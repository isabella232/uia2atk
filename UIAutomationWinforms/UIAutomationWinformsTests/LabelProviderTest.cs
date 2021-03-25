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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using Mono.UIAutomation.Winforms;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class LabelProviderTest : BaseProviderTest
	{
#region Tests
		[Test]
		public void BasicPropertiesTest ()
		{
			// TODO: Add to form or something, need a parent
			//       for certain things to work :-(
			Form parentForm = new Form ();
			Label label = new Label ();
			parentForm.Controls.Add (label);
			
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (label);
			
			//FIXME: Update this test
//			TestProperty (provider,
//			              AutomationElementIdentifiers.AutomationIdProperty,
//			              label.GetHashCode ());
			
			TestProperty (provider,
			              AutomationElementIdentifiers.ControlTypeProperty,
			              ControlType.Text.Id);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.LocalizedControlTypeProperty,
			              "text");
		}

		[Test]
		public void ProviderPatternTest ()
		{
			Label label = new Label ();
			IRawElementProviderSimple provider =
				ProviderFactory.GetProvider (label);

			object rangeValueProvider =
				provider.GetPatternProvider (RangeValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (rangeValueProvider,
			               "RangeValuePattern should not be supported.");

			object tableItemProvider =
				provider.GetPatternProvider (TableItemPatternIdentifiers.Pattern.Id);
			Assert.IsNull (tableItemProvider,
			               "TableItemPattern should not be supported.");

			object textProvider =
				provider.GetPatternProvider (TextPatternIdentifiers.Pattern.Id);
			Assert.IsNull (textProvider,
			               "TextPattern should not be supported.");

			object valueProvider =
				provider.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id);
			Assert.IsNull (valueProvider,
			               "ValuePattern should not be supported.");
		}
		
		[Test]
		public void TextChangedEventTest ()
		{
			Label label = new Label ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (label);
			
			bridge.ResetEventLists ();
			
			label.Text = "first";
			
			Assert.AreEqual (1,
			                 bridge.AutomationEvents.Count,
			                 "event count");
			
			Assert.AreEqual (provider,
			                 bridge.AutomationEvents [0].provider,
			                 "provider");
			
			
			// TODO: args
		}
		
#endregion
		
#region BaseProviderTest Overrides

		protected override bool IsContentElement {
			get { return false; }
		}
		
		protected override Control GetControlInstance ()
		{
			return new Label ();
		}

		public override void LabeledByAndNamePropertyTest ()
		{
			TestLabeledByAndName (false, false, true, true, false);
			// TODO: Test name change
		}
		
		public override void IsKeyboardFocusablePropertyTest ()
		{
			Control control = GetControlInstance ();
			IRawElementProviderSimple provider = ProviderFactory.GetProvider (control);
			
			TestProperty (provider,
			              AutomationElementIdentifiers.IsKeyboardFocusableProperty,
			              false);
		}

#endregion
	}
}
