// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//	Mario Carrion <mcarrion@novell.com>
// 

using System;
using Mono.Unix;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using SWF = System.Windows.Forms;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Navigation;

namespace Mono.UIAutomation.Winforms
{
	internal delegate IRawElementProviderFragment ComponentProviderMapperHandler (Component component);

	public static class ProviderFactory
	{	
		#region Static Fields
		
		// NOTE: This may not be the best place to track this...however
		//       I forsee this factory class evolving into a builder
		//       class that takes raw providers and attaches provider
		//       behaviors depending on the control type, and maybe
		//       it makes sense for the builder to keep track of
		//       this mapping?
		private static Dictionary<Component, IRawElementProviderFragment>
			componentProviders;
		
		private static List<IRawElementProviderFragmentRoot> formProviders;

		private static Dictionary<Type, Type> providerComponentMap
			= new Dictionary<Type, Type> ();

		private static Dictionary<Type, ComponentProviderMapperHandler> componentProviderMappers
			= new Dictionary<Type, ComponentProviderMapperHandler> ();

		#endregion
	
		#region Static Methods

		static ProviderFactory ()
		{
			Catalog.Init (Globals.CatalogName, Globals.LocalePath);

			componentProviders =
				new Dictionary<Component,IRawElementProviderFragment> ();
			
			formProviders = new List<IRawElementProviderFragmentRoot> ();

			InitializeProviderHash ();
		}

		private static void InitializeProviderHash ()
		{
			foreach (Type t in typeof (ProviderFactory).Assembly.GetTypes ()) {
				object[] attrs = t.GetCustomAttributes (
					typeof (MapsComponentAttribute), false);

				foreach (Attribute attr in attrs) {
					MapsComponentAttribute mca
						= attr as MapsComponentAttribute;
					if (mca == null) {
						continue;
					}

					if (mca.ProvidesMapper) {
						MethodInfo mi = t.GetMethod ("RegisterComponentMappings",
						                             BindingFlags.Static
						                             | BindingFlags.Public,
						                             null, new Type[0], null);
						if (mi == null) {
							Console.WriteLine (
								"WARNING: {0} is a ProvidesMapper but does not implement RegisterComponentMappings.", t
							);
							continue;
						}

						// Allow the class to register it's mappings
						mi.Invoke (null, null);
						continue;
					}

					if (providerComponentMap.ContainsKey (mca.From)) {
						Console.WriteLine (
							"WARNING: Component map already contains a provider for {0}.  Ignoring.",
							mca.From
						);
						continue;
					}

					providerComponentMap.Add (mca.From, t);
				}
			}
		}
		
		#endregion
		
		#region Static Public Methods
		
		public static List<IRawElementProviderFragmentRoot> GetFormProviders () 
		{
			return formProviders;
		}
		
		public static IRawElementProviderFragment GetProvider (Component component)
		{
			return GetProvider (component, true, false);
		}
		
		public static IRawElementProviderFragment GetProvider (Component component,
		                                                       bool initialize)
		{
			return GetProvider (component, initialize, false);
		}

		public static IRawElementProviderFragment GetProvider (Component component,
		                                                       bool initialize,
		                                                       bool forceInitializeChildren)
		{
			if (component == null)
				return null;

			// First check if we've seen this component before
			IRawElementProviderFragment provider = FindProvider (component);
			if (provider != null)
				return provider;

			// Send a WndProc message to see if the control
			// implements it's own provider.
			if (component is SWF.Control
			    // Sending WndProc to a form is broken for some reason
			    && !(component is SWF.Form)) {

				SWF.Control control = component as SWF.Control;
				IRawElementProviderSimple simpleProvider;
				IntPtr result;

				result = SWF.NativeWindow.WndProc (control.Handle, SWF.Msg.WM_GETOBJECT,
				                                   IntPtr.Zero, IntPtr.Zero);
				if (result != IntPtr.Zero) {
					simpleProvider = AutomationInteropProvider
						.RetrieveAndDeleteProvider (result);

					// TODO: If simpleProvider isn't a
					// Fragment, wrap it with a fragment
					// wrapper to preserve navigation
					provider = simpleProvider as IRawElementProviderFragment;
				}
			}

			ComponentProviderMapperHandler handler = null;
			Type providerType = null;

			if (provider == null) {
				Type typeIter = component.GetType ();

				// Chain up the type hierarchy until we find
				// either a type or handler for mapping, or we
				// hit Control or Component.
				do {
					// First see if there's a mapping handler
			    		if (componentProviderMappers.TryGetValue (typeIter,
					                                          out handler))
						break;

					// Next, see if we have a type mapping
					if (providerComponentMap.TryGetValue (typeIter,
					                                      out providerType))
						break;

					typeIter = typeIter.BaseType;
				} while (typeIter != null
				         && typeIter != typeof (System.ComponentModel.Component)
				         && typeIter != typeof (SWF.Control));
			}
			
			if (handler != null) {
				provider = handler (component);
			}

			// Create the provider if we found a mapping type
			if (provider == null && providerType != null) {
				try {
					provider = (FragmentControlProvider)
						Activator.CreateInstance (providerType,
									  new object [] { component });
				} catch (MissingMethodException) {
					Console.WriteLine (
						"ERROR: Provider {0} does not have a valid single parameter constructor to handle {1}.",
						providerType, component.GetType ()
					);
					return null;
				}
			}

			if (provider != null) {
				// TODO: Abstract this out?
				if (component is SWF.Form) {
					formProviders.Add ((IRawElementProviderFragmentRoot) provider);
				}

				// TODO: Make tracking in dictionary optional
				componentProviders [component] = provider;
				if (provider is FragmentControlProvider) {
					FragmentControlProvider frag = (FragmentControlProvider) provider;
					if (initialize)
						frag.Initialize ();
					
					if (forceInitializeChildren)
						frag.InitializeChildControlStructure ();
				}
			} else {
				//FIXME: let's not throw while we are developing, a big WARNING will suffice
				//throw new NotImplementedException ("Provider not implemented for control " + component.GetType().Name);
				Console.WriteLine ("WARNING: Provider not implemented for control " + component.GetType());
				return null;
			}
			
			return provider;
		}

		public static void ReleaseProvider (Component component)
		{
			IRawElementProviderFragment provider;
			if (componentProviders.TryGetValue (component, 
			                                    out provider) == true) {
				componentProviders.Remove (component);
				((FragmentControlProvider) provider).Terminate ();
				if (provider is FormProvider)
					formProviders.Remove ((FormProvider) provider);
			}
		}
		
		public static IRawElementProviderFragment FindProvider (Component component)
		{
			IRawElementProviderFragment provider;
			
			if (component == null)
				return null;
			else if (componentProviders.TryGetValue (component, out provider))
				return provider;

			return null;
		}

		internal static void RegisterComponentProviderMapper (System.Type componentType,
		                                                      ComponentProviderMapperHandler handler)
		{
			componentProviderMappers.Add (componentType, handler);
		}
		
		#endregion
	}
}
