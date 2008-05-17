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
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
//      Mario Carrion <mcarrion@novell.com>
// 

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace Mono.UIAutomation.Winforms
{
	public abstract class SimpleControlProvider : IRawElementProviderSimple
	{
#region Protected Fields
		
		protected Control control;
		
#endregion
		
#region Private Fields
		private Dictionary<EventStrategyType, IEventStrategy> events;
#endregion
		
#region Constructors
		
		public SimpleControlProvider (Control control)
		{
			this.control = control;
			
			events = new Dictionary<EventStrategyType,IEventStrategy> ();

			SetEventStrategy (EventStrategyType.IsOffscreenProperty, 
			                  new IsOffscreenPropertyEventStrategy (this, control));
			SetEventStrategy (EventStrategyType.IsEnabledProperty, 
			                  new IsEnabledPropertyEventStrategy (this, control));
			SetEventStrategy (EventStrategyType.NameProperty,
			                  new NamePropertyEventStrategy (this, control));
			SetEventStrategy (EventStrategyType.HasKeyboardFocusProperty,
			                  new HasKeyboardFocusPropertyEventStrategy (this, control));
			SetEventStrategy (EventStrategyType.BoundingRectangleProperty,
			                  new BoundingRectanglePropertyEventStrategy (this, control));
		}
		
#endregion
		
#region Protected
		protected virtual bool GetIsContentElementProperty () 
		{
			return true;
		}
		
		protected virtual bool GetIsControlElementProperty () 
		{
			return true;
		}
		
		protected virtual object GetLabeledByProperty ()
		{
			return null;
		}
		
		protected virtual object GetNameProperty ()
		{
			return control.Text;
		}
		
		protected abstract int GetControlTypeProperty ();
		
		protected void SetEventStrategy (EventStrategyType type, IEventStrategy strategy)
		{
			IEventStrategy value;
			
			if (events.TryGetValue (type, out value) == true) {			
				value.Disconnect ();
				events.Remove (type);
			}

			if (strategy != null) {
			    events [type] = strategy;
				strategy.Connect ();
			}
		}
#endregion
		
#region IRawElementProviderSimple Members
	
		public abstract object GetPatternProvider (int patternId);
		
		public virtual object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
				return control.GetHashCode (); // TODO: Ensure uniqueness
			else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
				return control.Enabled;
			else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
				return GetNameProperty ();
			else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
				return control.CanFocus;
			else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
				return control.Visible;
			else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
				return control.Focused;
			else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
				return GetIsControlElementProperty ();
			else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
				return GetIsContentElementProperty ();
			else
				return null;
		}

		public virtual IRawElementProviderSimple HostRawElementProvider {
			get {
				return AutomationInteropProvider.HostProviderFromHandle (control.TopLevelControl.Handle);
			}
		}
		
		public virtual ProviderOptions ProviderOptions {
			get {
				return ProviderOptions.ServerSideProvider;
			}
		}

#endregion
	}
}
