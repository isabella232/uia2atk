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
// 

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms
{
	public class ButtonProvider : SimpleControlProvider, IInvokeProvider
	{
#region Private Members
		
		private Button button;
		
#endregion
		
#region Constructors
		
		public ButtonProvider (Button button) : base (button)
		{
			this.button = button;
		}
		
#endregion
		
#region Protected Methods

		protected override void InitializeEvents ()
		{
			base.InitializeEvents ();

			SetEvent (EventStrategyType.InvokedEvent, 
			          new InvokedEventStrategy (this, control));
		}

#endregion
		
#region IInvokeProvider Members
		
		public void Invoke ()
		{
			if (!button.Enabled)
				throw new ElementNotEnabledException ();
			
			// TODO: Make sure this runs on the right thread
			button.PerformClick ();
		}
		
#endregion
		
#region IRawElementProviderSimple Members
	
		public override object GetPatternProvider (int patternId)
		{
			if (patternId == InvokePatternIdentifiers.Pattern.Id)
				return this;
			
			return null;
		}
		
		public override object GetPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Button.Id;
			else if (propertyId == AutomationElementIdentifiers.AcceleratorKeyProperty.Id)
				throw new NotImplementedException ("AcceleratorKeyProperty");
			else if (propertyId == AutomationElementIdentifiers.BoundingRectangleProperty.Id)
				return Helper.RectangleToRect (button.Bounds);
			else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
				throw new NotImplementedException ("HelpTextProperty");
			else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
				return "button";
			else
				return base.GetPropertyValue (propertyId);
		}

#endregion
	}

}