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
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Forms;

using Mono.UIAutomation.Winforms;
using Mono.UIAutomation.Bridge;

using NUnit.Framework;

namespace MonoTests.Mono.UIAutomation.Winforms
{
	[TestFixture]
	public class WindowProviderTest : BaseProviderTest
	{
		
#region IWindowProvider Tests
		
		[Test]
		public void MinimizableTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsTrue (pattern.Minimizable, "Initialize to false");
				f.MinimizeBox = false;
				Assert.IsFalse (pattern.Minimizable, "Set to true");
				f.MinimizeBox = true;
				Assert.IsTrue (pattern.Minimizable, "Set to false");
			}
		}
		
		[Test]
		public void MaximizableTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsTrue (pattern.Maximizable, "Initialize to false");
				f.MaximizeBox = false;
				Assert.IsFalse (pattern.Maximizable, "Set to true");
				f.MaximizeBox = true;
				Assert.IsTrue (pattern.Maximizable, "Set to false");
			}
		}
		
		[Test]
		public void IsTopmostTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsFalse (pattern.IsTopmost, "Initialize to false");
				f.TopMost = true;
				Assert.IsTrue (pattern.IsTopmost, "Set to true");
				f.TopMost = false;
				Assert.IsFalse (pattern.IsTopmost, "Set to false");
			}
		}
		
		[Test]
		public void IsModalTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				Assert.IsFalse (pattern.IsModal, "Form should initialize to not modal");
				
				// Run modal dialog in separate thread
				Thread t = new Thread (new ParameterizedThreadStart (delegate {
					f.ShowDialog ();
				}));
				t.Start ();
				
				// Wait for dialog to appear
				Thread.Sleep (500); // TODO: Fragile
				
				Assert.IsTrue (pattern.IsModal, "ShowDialog should be modal");
				
				f.Close ();
				t.Join ();
				
				f.Show ();
				// Wait for form to appear
				Thread.Sleep (500); // TODO: Fragile
				
				Assert.IsFalse (pattern.IsModal, "Show should not be modal");
				f.Close ();
			}
		}
		
		[Test]
		public void CloseTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				f.Show ();
				
				Assert.AreEqual (WindowInteractionState.Running,
				                 pattern.InteractionState,
				                 "Interaction state while running normally");
				
				bool formClosed = false;
				bool formClosingChecked = false;
				f.Closed += delegate (Object sender, EventArgs e) {
					formClosed = true;
				};
				f.Closing += delegate (Object sender, CancelEventArgs e) {
					Assert.AreEqual (WindowInteractionState.Closing,
					                 pattern.InteractionState,
					                 "Interaction state while closing");
					formClosingChecked = true;
				};
				
				Console.WriteLine ("provider close 0: "+bridge.StructureChangedEvents.Count);
				bridge.ResetEventLists ();
				Console.WriteLine ("provider close 1: "+bridge.StructureChangedEvents.Count);
				pattern.Close ();
				Console.WriteLine ("provider close 2: "+bridge.StructureChangedEvents.Count);
				
				Assert.IsTrue (formClosed, "Form closed event didn't fire.");
				Assert.IsTrue (formClosingChecked, "Interaction state while closing never confirmed.");
				
				Assert.AreEqual (1, bridge.StructureChangedEvents.Count, "event count");
				Assert.AreSame (provider, bridge.StructureChangedEvents [0].provider, "event provider");
				Assert.AreEqual (StructureChangeType.ChildRemoved, bridge.StructureChangedEvents [0].e.StructureChangeType, "event change type");
				
				Application.DoEvents ();
			}
		}
		
		[Test]
		public void SetVisualStateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				//Application.DoEvents ();
					
				Assert.AreEqual (FormWindowState.Normal, f.WindowState, "Form should initially be 'normal'");
				
				pattern.SetVisualState (WindowVisualState.Maximized);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Maximized, f.WindowState, "Form should maximize");
				
				pattern.SetVisualState (WindowVisualState.Minimized);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Minimized, f.WindowState, "Form should minimize");
				
				pattern.SetVisualState (WindowVisualState.Normal);
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (FormWindowState.Normal, f.WindowState, "Form should return to 'normal'");
			}
		}
		
		[Test]
		public void VisualStateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment) ProviderFactory.GetProvider (f);
				IWindowProvider pattern = (IWindowProvider) provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				//Application.DoEvents ();
				
				Assert.AreEqual (WindowVisualState.Normal, pattern.VisualState, "Provider should initially be 'normal'");
				
				f.WindowState = FormWindowState.Maximized;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Maximized, pattern.VisualState, "Provider should maximize");
				
				f.WindowState = FormWindowState.Minimized;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Minimized, pattern.VisualState, "Provider should minimize");
				
				f.WindowState = FormWindowState.Normal;
				//System.Threading.Thread.Sleep (1000);
				//Application.DoEvents ();
				//System.Threading.Thread.Sleep (1000);
				Assert.AreEqual (WindowVisualState.Normal, pattern.VisualState, "Provider should return to 'normal'");
			}
		}
		
#endregion
		
#region ITransformProvider Tests

		[Test]
		public void CanMoveTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				
				Assert.IsTrue (transform.CanMove,
				               "True by default");
				f.WindowState = FormWindowState.Maximized;
				Assert.IsFalse (transform.CanMove,
				                "Maximized");
				f.WindowState = FormWindowState.Minimized;
				Assert.IsFalse (transform.CanMove,
				                "Minimized");
				f.WindowState = FormWindowState.Normal;
				Assert.IsTrue (transform.CanMove,
				               "Normal");
			}
		}

		[Test]
		public void CanResizeTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				f.Show ();
				
				Assert.IsTrue (transform.CanResize,
				               "True by default");
				
				f.FormBorderStyle = FormBorderStyle.Fixed3D;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.Fixed3D");
				f.FormBorderStyle = FormBorderStyle.FixedDialog;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.FixedDialog");
				f.FormBorderStyle = FormBorderStyle.FixedSingle;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.FixedSingle");
				f.FormBorderStyle = FormBorderStyle.FixedToolWindow;
				Assert.IsFalse (transform.CanResize,
				                "FormBorderStyle.FixedToolWindow");
				
				f.FormBorderStyle = FormBorderStyle.None;
				Assert.IsTrue (transform.CanResize,
				                "FormBorderStyle.None");
				f.FormBorderStyle = FormBorderStyle.Sizable;
				Assert.IsTrue (transform.CanResize,
				                "FormBorderStyle.Sizable");
				f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				Assert.IsTrue (transform.CanResize,
				                "FormBorderStyle.SizableToolWindow");
			}
		}

		[Test]
		public void CanRotateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				Assert.IsFalse (transform.CanRotate,
				                "Should always be false");
			}
		}

		[Test]
		public void MoveTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				//f.Show ();
				
				transform.Move (15, 20);
				Assert.AreEqual (15, f.Location.X, "X, default form");
				Assert.AreEqual (20, f.Location.Y, "Y, default form");
				
				f.WindowState = FormWindowState.Maximized;
				VerifyMoveFail (f, transform, FormWindowState.Maximized);
				f.WindowState = FormWindowState.Minimized;
				VerifyMoveFail (f, transform, FormWindowState.Minimized);
				
				f.WindowState = FormWindowState.Normal;
				
				transform.Move (150, 100);
				Assert.AreEqual (150, f.Location.X, "X, normal form");
				Assert.AreEqual (100, f.Location.Y, "Y, normal form");
			}
		}
		
		private void VerifyMoveFail (Form f, ITransformProvider transform, FormWindowState state)
		{
			f.WindowState = state;
			try {
				transform.Move (10, 10);
				Assert.Fail ("Expected InvalidOperationException");
			} catch (InvalidOperationException) {
				// Expected
			} catch (Exception e){
				Assert.Fail ("Expected InvalidOperationException, instead got this exception: " + e.Message);
			}
		}

		[Test]
		public void ResizeTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				f.Show ();
				
				VerifyResizeFail (f, transform, FormBorderStyle.Fixed3D);
				VerifyResizeFail (f, transform, FormBorderStyle.FixedDialog);
				VerifyResizeFail (f, transform, FormBorderStyle.FixedSingle);
				VerifyResizeFail (f, transform, FormBorderStyle.FixedToolWindow);
				
				f.FormBorderStyle = FormBorderStyle.None;
				transform.Resize (100, 200);
				Assert.AreEqual (100, f.Width, "Width");
				Assert.AreEqual (200, f.Height, "Height");
				
				// Problematic...getting 110 for width!
				/*f.FormBorderStyle = FormBorderStyle.Sizable;
				transform.Resize (35.7, 10);
				Assert.AreEqual (36, f.Width, "Width");
				Assert.AreEqual (10, f.Height, "Height");*/
				
				f.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				transform.Resize (1234, 500.2);
				Assert.AreEqual (1234, f.Width, "Width");
				Assert.AreEqual (500, f.Height, "Height");
			}
		}
		
		private void VerifyResizeFail (Form f, ITransformProvider transform, FormBorderStyle style)
		{
			f.FormBorderStyle = style;
			try {
				transform.Resize (10, 10);
				Assert.Fail ("Expected InvalidOperationException");
			} catch (InvalidOperationException) {
				// Expected
			} catch (Exception e){
				Assert.Fail ("Expected InvalidOperationException, instead got this exception: " + e.Message);
			}
		}

		[Test]
		public void RotateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragment provider = (IRawElementProviderFragment)
					ProviderFactory.GetProvider (f);
				ITransformProvider transform = (ITransformProvider)
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				
				try {
					transform.Rotate (1);
					Assert.Fail ("Expected InvalidOperationException");
				} catch (InvalidOperationException) {
					// Expected
				} catch (Exception e){
					Assert.Fail ("Expected InvalidOperationException, instead got this exception: " + e.Message);
				}
			}
		}
#endregion
		
#region IRawElementProviderFragmentRoot Tests
		
		[Test]
		[Ignore ("Not implemented")]
		public void HostRawElementProviderTest ()
		{
			;
		}
		
		[Test]
		public void ProviderOptionsTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
				Assert.AreEqual (ProviderOptions.ServerSideProvider,
				                 provider.ProviderOptions,
				                 "ProviderOptions");
			}
		}
		
		[Test]
		public void GetPatternProviderTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			
				object window =
					provider.GetPatternProvider (WindowPatternIdentifiers.Pattern.Id);
				Assert.IsNotNull (window);
				Assert.IsTrue (window is IWindowProvider,
				               "IWindowProvider");
				
				object transform =
					provider.GetPatternProvider (TransformPatternIdentifiers.Pattern.Id);
				Assert.IsNotNull (transform);
				Assert.IsTrue (transform is ITransformProvider,
				               "ITransformProvider");
			}
		}
		
		/*
		[Test]
		[Ignore ("Not implemented")]
		public void GetPropertyValueTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void BoundingRectangleTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			}
		}
		*/
		
		[Test]
		public void FragmentRootTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
				Assert.AreEqual (provider,
				                 provider.FragmentRoot);
			}
		}
		
		[Test]
		public void GetEmbeddedFragmentRootsTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
				Assert.IsNull (provider.GetEmbeddedFragmentRoots ());
			}
		}
		
		/*
		[Test]
		[Ignore ("Not implemented")]
		public void GetRuntimeIdTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void NavigateTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void SetFocusTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void ElementProviderFromPointTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			}
		}
		
		[Test]
		[Ignore ("Not implemented")]
		public void GetFocusTest ()
		{
			using (Form f = new Form ()) {
				IRawElementProviderFragmentRoot provider =
					(IRawElementProviderFragmentRoot) ProviderFactory.GetProvider (f);
			}
		}
		*/

#endregion
		
#region BaseProviderTest Overrides
		
		protected override Control GetControlInstance ()
		{
			return new Form ();
		}
		
#endregion
		
	}
}
