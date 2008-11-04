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
//	Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms;
using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class ToolTipProviderTest
	{
		[SetUp]
		public void SetUp ()
		{
			TestHelper.SetUpEnvironment ();
			
			form = new Form ();
			form.Show ();
		}
		
		[TearDown]
		public void TearDown ()
		{
			TestHelper.TearDownEnvironment ();

			form.Close ();
			form_provider = null;
		}
		
		[Test]
		public void ControlPropertiesTest ()
		{
			Label label = new Label ();
			label.Size = new System.Drawing.Size (30, 30);
			form.Controls.Add (label);

			ToolTip tooltip = new ToolTip ();
			tooltip.SetToolTip (label, "This is a tooltip");
			tooltip.Show (null, label);
			tooltip.ShowAlways = true;

			IRawElementProviderSimple provider
				= ProviderFactory.GetProvider (tooltip);
			
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.ControlTypeProperty, ControlType.ToolTip.Id);
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.NameProperty, "This is a tooltip");
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.LabeledByProperty, null);
			TestHelper.TestAutomationProperty (
				provider, AutomationElementIdentifiers.LocalizedControlTypeProperty, "tool tip");
		}

#region private fields
		private MockBridge bridge;
		private Form form;
		private FormProvider form_provider;
#endregion
	}
}
