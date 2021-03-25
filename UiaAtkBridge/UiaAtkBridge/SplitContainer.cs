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
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com) 
// 
// Authors:
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class SplitContainer : ComponentParentAdapter , Atk.IValueImplementor
	{
		private IRangeValueProvider rangeValueProvider;
		private OrientationType orientation;
		
		public SplitContainer (IRawElementProviderSimple provider) : base (provider)
		{
			Role = Atk.Role.SplitPane;
			rangeValueProvider = (IRangeValueProvider)provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			object o = provider.GetPropertyValue (AutomationElementIdentifiers.OrientationProperty.Id);
			if (o is OrientationType)
				orientation = (OrientationType)o;
			else {
				IDockProvider dockProvider = (IDockProvider)provider.GetPatternProvider (DockPatternIdentifiers.Pattern.Id);
				if (dockProvider != null) {
					orientation = (dockProvider.DockPosition == DockPosition.Top || dockProvider.DockPosition == DockPosition.Bottom)?
						OrientationType.Horizontal:
						OrientationType.Vertical;
				} else {
					Log.Warn ("SplitContainer: Couldn't get orientation for splitter.  Does not support DockProvider.");
					orientation = OrientationType.Horizontal;
				}
			}
		}

		public void GetMinimumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Minimum: 0);
		}

		public void GetMaximumValue (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.Maximum: 100);
		}

		public void GetMinimumIncrement (ref GLib.Value value)
		{
			value = new GLib.Value (rangeValueProvider != null? rangeValueProvider.SmallChange: 1);
		}

		public void GetCurrentValue (ref GLib.Value value)
		{
			if (rangeValueProvider != null) {
				value = new GLib.Value (rangeValueProvider.Value);
				return;
			}
		}

		public bool SetCurrentValue (GLib.Value value)
		{
			double v = (double)value.Val;
			return SetCurrentValue (v);
		}

		private bool SetCurrentValue (double v)
		{
			if (rangeValueProvider != null) {
				if (v > rangeValueProvider.Maximum)
					return false;
				if (v < rangeValueProvider.Minimum)
					v = rangeValueProvider.Minimum;
				
				try {
					rangeValueProvider.SetValue (v);
				} catch (ArgumentOutOfRangeException e) {
					Log.Debug (e);
					return false;
				} catch (ElementNotEnabledException e) {
					Log.Debug (e);
					return false;
				}

				return true;
			}
			return false;
		}

		public void GetValueAndText (out double value, out string text)
		{
			value = (rangeValueProvider != null? rangeValueProvider.Value: 0);
			text = "";
		}

		public Atk.Range Range {
			get {
				double minimum = (rangeValueProvider != null? rangeValueProvider.Minimum: 0);
				double maximum = (rangeValueProvider != null? rangeValueProvider.Maximum: 0);
				return new Atk.Range (minimum, maximum, "");
			}
		}

		public double Increment {
			get {
				return 0;
			}
		}

		public GLib.SList SubRanges {
			get {
				return null;
			}
		}

		public double Value {
			set {
				SetCurrentValue (value);
			}
		}

		protected override Atk.StateSet OnRefStateSet ()
		{
			Atk.StateSet states = base.OnRefStateSet ();

			if (states.ContainsState (Atk.StateType.Defunct))
				return states;

			// Selectable added by atk if parent has Atk.Selection
			states.RemoveState (Atk.StateType.Selectable);
			// A horizontal line splits the pane vertically and vice versa
			states.AddState (orientation == OrientationType.Vertical? Atk.StateType.Horizontal: Atk.StateType.Vertical);
			return states;
		}

		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
		}

		public override void RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			if (e.Property == RangeValuePatternIdentifiers.ValueProperty) {
				Notify ("accessible-value");
			}
		}
	}
}
