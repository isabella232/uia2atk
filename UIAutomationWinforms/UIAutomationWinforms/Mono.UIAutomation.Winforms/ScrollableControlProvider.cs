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
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Winforms.Behaviors.ScrollableControl;
using AEIds = System.Windows.Automation.AutomationElementIdentifiers;

namespace Mono.UIAutomation.Winforms
{
	[MapsComponent (typeof (ScrollableControl))]
	internal class ScrollableControlProvider
		: FragmentRootControlProvider, IScrollBehaviorSubject
	{
#region Public Methods
		public ScrollableControlProvider (ScrollableControl control)
			: base (control)
		{
			scrollableControl = control;
		}
#endregion

#region IScrollBehaviorSubject Implementation
		public IScrollBehaviorObserver ScrollBehaviorObserver {
			get { return observer; }
		}

		public FragmentControlProvider GetScrollbarProvider (ScrollBar scrollbar)
		{
			return ProviderFactory.GetProvider (scrollbar)
				as FragmentControlProvider;
		}
#endregion

#region FragmentRootControlProvider Implementation

		protected override void InitializeChildControlStructure ()
		{
			base.InitializeChildControlStructure ();

			observer = new ScrollBehaviorObserver (
				this, scrollableControl.hscrollbar,
				scrollableControl.vscrollbar);
			observer.ScrollPatternSupportChanged += OnScrollPatternSupportChanged;
			observer.Initialize ();
			UpdateScrollBehavior ();
		}

		protected override void FinalizeChildControlStructure()
		{
			if (observer != null) {
				UpdateScrollBehavior (forceDisconnect: true);
				observer.ScrollPatternSupportChanged -= OnScrollPatternSupportChanged;
				observer.Terminate ();
				observer = null;
			}
			base.FinalizeChildControlStructure ();
		}

		protected override object GetProviderPropertyValue (int propertyId)
		{
			if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
				return ControlType.Pane.Id;

			return base.GetProviderPropertyValue (propertyId);
		}
#endregion

#region Private Methods
		private void OnScrollPatternSupportChanged (object o, EventArgs args)
		{
			UpdateScrollBehavior ();
		}	

		private void UpdateScrollBehavior (bool forceDisconnect = false)
		{
			if (observer != null && observer.SupportsScrollPattern)
				SetBehavior (ScrollPatternIdentifiers.Pattern,
				             new ScrollProviderBehavior (this));
			else
				SetBehavior (ScrollPatternIdentifiers.Pattern, null);
		}
#endregion

#region Private Fields
		private ScrollBehaviorObserver observer;
		private ScrollableControl scrollableControl;
#endregion
	}
}
