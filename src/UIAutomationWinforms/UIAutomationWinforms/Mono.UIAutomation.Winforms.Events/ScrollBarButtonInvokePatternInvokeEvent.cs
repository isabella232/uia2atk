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
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

namespace Mono.UIAutomation.Winforms.Events
{

	internal class ScrollBarButtonInvokePatternInvokeEvent
		: InvokePatternInvokedEvent
	{
		
		#region Constructors
		
		public ScrollBarButtonInvokePatternInvokeEvent (ScrollBarProvider.ScrollBarButtonProvider provider) 
			: base (provider)
		{
		}
				
		#endregion

		#region IConnectable Overriders

		public override void Connect (Control control)
		{		
			try {
				Helper.AddPrivateEvent (typeof (ScrollBar), 
				                        (ScrollBar) control, 
				                        "UIAScroll",
				                        this, 
				                        "OnButtonClicked");
			} catch (NotSupportedException) {}			
		}

		public override void Disconnect (Control control)
		{
			try {
				Helper.RemovePrivateEvent (typeof (ScrollBar), 
				                           (ScrollBar) control, 
				                           "UIAScroll",
				                           this, 
				                           "OnButtonClicked");	
			} catch (NotSupportedException) {}
		}

		#endregion

		#region Private Methods
		
#pragma warning disable 169
		
		private void OnButtonClicked (object sender, ScrollEventArgs args)
		{
			ScrollBarProvider.ScrollBarButtonProvider provider 
				= (ScrollBarProvider.ScrollBarButtonProvider) Provider;
			
			if ((args.Type == ScrollEventType.LargeDecrement
			     && provider.Orientation == ScrollBarProvider.ScrollBarButtonOrientation.LargeBack)
			    || (args.Type == ScrollEventType.LargeIncrement 
			        && provider.Orientation == ScrollBarProvider.ScrollBarButtonOrientation.LargeForward)
			    || (args.Type == ScrollEventType.SmallDecrement
			        && provider.Orientation == ScrollBarProvider.ScrollBarButtonOrientation.SmallBack)
			    || (args.Type == ScrollEventType.SmallIncrement
			        && provider.Orientation == ScrollBarProvider.ScrollBarButtonOrientation.SmallForward))
				InvokeEvent ();
		}
		
#pragma warning restore 169
		
		#endregion
	}
}
