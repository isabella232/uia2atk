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
//      Andres G. Aragoneses <aaragoneses@novell.com>
// 

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{

	public abstract class AtkTests : AtkTester
	{
		
		[Test]
		public void Label ()
		{
			Label (BasicWidgetType.Label);
		}
		
		protected void Label (BasicWidgetType type)
		{
			Atk.Object accessible = InterfaceText (type);

			PropertyRole (type, accessible);
			
			//a label always contains this state, not because it's multi_line, but because it can be multi_line
			States (accessible,
			  Atk.StateType.MultiLine,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Label numChildren");
			
			//TODO: check parent (it seems it only works for real objects)
			//Assert.IsNotNull (accessible.Parent, "Label parent");
		}
		
		[Test]
		public void Button ()
		{
			BasicWidgetType type = BasicWidgetType.NormalButton;
			Atk.Object accessible;

			string name = "test";
			accessible = GetAccessible (type, name, true);

			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Focusable,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

			InterfaceText (type);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Button numChildren");
			
			Parent (type, accessible);
			
			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);
		}

		[Test]
		public void Checkbox ()
		{
			EventMonitor.Start ();

			BasicWidgetType type = BasicWidgetType.CheckBox;
			Atk.Object accessible;
			
			InterfaceText (type);
			
			string name = "test";
			accessible = GetAccessible (type, name, true);
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Atk.StateSet stateSet = accessible.RefStateSet ();
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Enabled), "Checkbox Enabled");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Sensitive), "Checkbox Sensitive");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Focusable), "Checkbox Focusable");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Showing), "Checkbox Showing");
			Assert.IsTrue (stateSet.ContainsState (Atk.StateType.Visible), "Checkbox Visible");
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "CheckBox numChildren");
			Parent (type, accessible);

			EventCollection events = EventMonitor.Pause ();
			string eventsInXml = String.Format (" events in XML: {0}", Environment.NewLine + events.OriginalGrossXml);
			string evType = "object:state-changed:checked";
			EventCollection checkboxEvs = events.FindByRole (Atk.Role.CheckBox).FindWithDetail1 ("1");
			EventCollection typeEvs = checkboxEvs.FindByType (evType);
			
			Assert.AreEqual (1, typeEvs.Count, "bad number of checked events!" + eventsInXml);

			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);
		}
		
		[Test]
		public void RadioButtons ()
		{
			BasicWidgetType type = BasicWidgetType.RadioButton;
			Atk.Object accessible = null, accessible2 = null, accessible3 = null;
			string name = "test 01";

			accessible = GetAccessible (type, name, true);
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "RadioButton numChildren");

			Parent (type, accessible);
			
			//more than one radiobutton
			name = "test 02";
			accessible2 = GetAccessible (type, name, true);
			Atk.Action atkAction2 = CastToAtkInterface <Atk.Action> (accessible2);

			//the third radio button is disconnected from the previous ones
			name = "test 03";
			accessible3 = GetAccessible (type, name, true);
			Atk.Action atkAction3 = CastToAtkInterface <Atk.Action> (accessible3);

			InterfaceActionFor3RadioButtons (atkAction, accessible,
			                                 atkAction2, accessible2,
			                                 atkAction3, accessible3);

			Parent (type, accessible);
			Parent (type, accessible2);
			Parent (type, accessible3);

			RunInGuiThread (delegate () {
				accessible = InterfaceText (type, true);
			});
			
			//test with an image
			Atk.Image atkWithOutImage, atkWithImage;
			atkWithOutImage = CastToAtkInterface <Atk.Image> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithOutImage);
		}
		
		[Test]
		public void StatusBar () { RunInGuiThread (RealStatusBar); }
		public void RealStatusBar()
		{
			BasicWidgetType type = BasicWidgetType.StatusBar;

			Atk.Object accessible = InterfaceText (type, true);

			Assert.AreEqual (ValidNChildrenForASimpleStatusBar, accessible.NAccessibleChildren, "StatusBar numChildren");

			string name = "test";
			accessible = GetAccessible (type, name, true);

			PropertyRole (type, accessible);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			int x, y, width, height;
			atkComponent.GetExtents (out x, out y, out width, out height, Atk.CoordType.Screen);
			Assert.IsTrue (width > 0 && height > 0, "width and height must be > 0");

			Parent (type, accessible);
		}
		
		[Test]
		public void HScrollBar ()
		{
			BasicWidgetType type = BasicWidgetType.HScrollBar;
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			States (accessible,
				Atk.StateType.Horizontal,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			

			Assert.AreEqual (ValidNChildrenForAScrollBar, accessible.NAccessibleChildren, "HScrollBar numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);

			//Simple Set/Get
			GLib.Value glibValue = GLib.Value.Empty;
			atkValue.GetMaximumValue (ref glibValue);
			double value = (double) glibValue.Val;

			glibValue.Val = value - 1;
			atkValue.SetCurrentValue (glibValue);
			atkValue.GetCurrentValue (ref glibValue);
			Assert.AreEqual (value - 1,
			                 (double) glibValue.Val, "Set/Get values failed.");

			//SHOULD NOT THROW ANY EXCEPTION when Maximum + 1
			atkValue.GetCurrentValue (ref glibValue);
			double currentValue = (double) glibValue.Val;
			atkValue.GetMaximumValue (ref glibValue);
			glibValue = new GLib.Value ((double) glibValue.Val + 1);
			atkValue.SetCurrentValue (glibValue);
			atkValue.GetCurrentValue (ref glibValue);
			Assert.AreEqual (currentValue,
			                 (double) glibValue.Val, "Set/Get values failed. (Maximum + 1)");

			//SHOULD NOT THROW ANY EXCEPTION when Minimum - 1
			atkValue.GetCurrentValue (ref glibValue);
			currentValue = (double) glibValue.Val;
			atkValue.GetMinimumValue (ref glibValue);
			glibValue = new GLib.Value ((double) glibValue.Val - 1);
			atkValue.SetCurrentValue (glibValue);
			atkValue.GetCurrentValue (ref glibValue);
			Assert.AreEqual (currentValue,
			                 (double) glibValue.Val, "Set/Get values failed. (Maximum + 1)");
		}
		
		[Test]
		public void VScrollBar ()
		{
			BasicWidgetType type = BasicWidgetType.VScrollBar;
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			States (accessible,
				Atk.StateType.Vertical,
				Atk.StateType.Enabled,
				Atk.StateType.Sensitive,
				Atk.StateType.Showing,
				Atk.StateType.Visible);
			
			Assert.AreEqual (ValidNChildrenForAScrollBar, accessible.NAccessibleChildren, "VScrollBar numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
		}
		
 		[Test]
		public void ProgressBar ()
		{
			ProgressBar (BasicWidgetType.ProgressBar);
		}

		protected void ProgressBar (BasicWidgetType type)
		{
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			
			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Assert.AreEqual (0, accessible.NAccessibleChildren, "ProgressBar numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Parent (type, accessible);
		}
		
 		[Test]
		public void Spinner ()
		{
			BasicWidgetType type = BasicWidgetType.Spinner;
			Atk.Object accessible;
			string name = "test";

			accessible = GetAccessible (type, name, true);
			Atk.Value atkValue = CastToAtkInterface <Atk.Value> (accessible);
			Atk.Text atkText = CastToAtkInterface <Atk.Text> (accessible);

			InterfaceValue (type, atkValue, atkText);

			InterfaceValue (type, atkValue);

			PropertyRole (type, accessible);

			Assert.AreEqual (0, accessible.NAccessibleChildren, "Spinner numChildren");

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			
			InterfaceComponent (type, atkComponent);

			InterfaceEditableText (type, accessible);

			Atk.Action atkAction = CastToAtkInterface<Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);
		}
		
		[Test]
		public void TextBoxEntry ()
		{
			BasicWidgetType type = BasicWidgetType.TextBoxEntry;
			Atk.Object accessible = InterfaceText (type, true);
			
			string name = "Edit test#1";
			accessible = GetAccessible (type, name, true);

			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  Atk.StateType.Focusable,
			  Atk.StateType.SingleLine,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
			
//			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
//			InterfaceAction (type, atkAction, accessible);
//			
//			name = "Edit test#2";
//			accessible = GetAccessible (type, name, true);
//			Atk.Component atkComponent = CastToAtkInterface<Atk.Component> (accessible);
//
//			InterfaceComponent (type, atkComponent);
//			
//			PropertyRole (type, accessible);
//			
//			Assert.AreEqual (0, accessible.NAccessibleChildren, "TextBoxEntry numChildren");
		}

		[Test]
		public void TextBoxView ()
		{
			BasicWidgetType type = BasicWidgetType.TextBoxView;
			Atk.Object accessible = InterfaceText (type, false);
			
			string name = "Edit test#1";
			accessible = GetAccessible (type, name, true);

			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  Atk.StateType.Focusable,
			  Atk.StateType.MultiLine,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
		}

		[Test]
		public void MaskedTextBoxEntry ()
		{
			BasicWidgetType type = BasicWidgetType.TextBoxEntry;
			Atk.Object accessible = null;
			
			accessible = InterfaceText (type, true);
			
			string name = "Edit test#1";
			accessible = GetAccessible (type, name, true);
			
			States (accessible,
			  Atk.StateType.Editable, 
			  Atk.StateType.Enabled, 
			  Atk.StateType.Focusable,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible,
			  Atk.StateType.SingleLine);
		}
		
		[Test]
		public void MainMenuBar ()
		{
			BasicWidgetType type = BasicWidgetType.MainMenuBar;
			Atk.Object accessible = null;
			
			string menuName = "File!";
			string[] names = new string[] { menuName, "New", "Quit" };
			accessible = GetAccessible (type, names, true);

			Assert.IsNull (accessible.Name, "name of the menubar should be null, now it's:" + accessible.Name);
			
			States (accessible,
			  Atk.StateType.Enabled, 
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

			PropertyRole (type, accessible);

			Assert.AreEqual (1, accessible.NAccessibleChildren, "number of children; children roles:" + childrenRoles (accessible));

			for (int i = 0; i < accessible.NAccessibleChildren; i++) {
				Atk.Object parentMenuChild = accessible.RefAccessibleChild (i);
				Assert.IsNotNull (parentMenuChild, "menubar child#" + i + " should not be null");

				Assert.IsTrue ( //FIXME: check if it's possible to have a MenuItem alone (like a push button)
				  (parentMenuChild.Role == Atk.Role.Menu), "menubar children should have Menu role");
				Assert.AreEqual (names [0], parentMenuChild.Name, "name of the parentmenu is the same as its label");
			}

			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			//FIXME:
			//Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			//InterfaceSelection (atkSelection, names, accessible, type);
			
//TODO:
//			List <MenuLayout> menu = new List <MenuLayout> ();
//			menu.Add (new MenuLayout ("XFile", new MenuLayout ("Quit")));
//			menu.Add (new MenuLayout ("GimmeHelp", new MenuLayout ("About")));
		}
		
		[Test]
		public void ParentMenu () 
		{
			BasicWidgetType type = BasicWidgetType.ParentMenu;
			Atk.Object accessible = null;
			
			string menuName = "File!";
			string[] names = new string[] { menuName, "New", "Quit" };
			accessible = GetAccessible (type, names, true);
			
			Assert.AreEqual (menuName, accessible.Name, "name of the menu is the same as its label");

			PropertyRole (type, accessible);

			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Selectable, 
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing, 
			  Atk.StateType.Visible);
			
			Assert.AreEqual (names.Length - 1, accessible.NAccessibleChildren, "number of children; children roles:" + childrenRoles (accessible));
			
			for (int i = 0; i < accessible.NAccessibleChildren; i++) {
				Atk.Object menuChild = accessible.RefAccessibleChild (i);
				Assert.IsNotNull (menuChild, "menu child#0 should not be null");
				Assert.IsTrue (
				  ((menuChild.Role == Atk.Role.Menu) ||
				   (menuChild.Role == Atk.Role.MenuItem) ||
				   (menuChild.Role == Atk.Role.TearOffMenuItem) ||
				   (menuChild.Role == Atk.Role.Separator)), "valid roles for a child of a parentMenu");
				
				Assert.IsTrue (menuChild.NAccessibleChildren > 0 || (menuChild.Role != Atk.Role.Menu),
				   "only grandchildren allowed if parent is menu");

				Assert.AreEqual (menuChild.Name, names [i + 1], "name of the menu is the same as its label");
			}
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);

			names [0] = simpleTestText;
			accessible = GetAccessible (type, names, true);
			InterfaceText (type, true, accessible);
			
			//FIXME:
			//Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			//InterfaceSelection (atkSelection, names, accessible, type);
		}

		[Test]
		public void ComboBoxDropDownEntry ()
		{
			ComboBoxDropDownEntry (null);
		}
		
		protected void ComboBoxDropDownEntry (object widget)
		{
			BasicWidgetType type = BasicWidgetType.ComboBoxDropDownEntry;
			Atk.Object accessible;
			
			string [] names = new string[] { "First item", "Second Item", "Last Item" };
			if (widget != null)
				accessible = GetAccessible (type, names, widget);
			else
				accessible = GetAccessible (type, names);
			
			StatesComboBox (accessible);

			Assert.AreEqual (2, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));
			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names);

			Atk.Object entryChild = accessible.RefAccessibleChild (1);
			Assert.IsNotNull (entryChild, "ComboBox child#1 should not be null");
			Assert.AreEqual (entryChild.Role, Atk.Role.Text, "Role of 2nd child");
			Assert.IsNull (entryChild.Name, "textbox .Name should be null");
		}
		
		//it's safer to put this test the last, apparently Atk makes it unresponsive after dealing with
		//the widget, so we kill all with the method marked as [TestFixtureTearDown]
		[Test]
		public void ComboBoxDropDownList ()
		{
			ComboBoxDropDownList (null);
		}

		//it's safer to put this test the last, apparently Atk makes it unresponsive after dealing with
		//the widget, so we kill all with the method marked as [TestFixtureTearDown]
		public void ComboBoxDropDownList (object widget)
		{
			BasicWidgetType type = BasicWidgetType.ComboBoxDropDownList;
			Atk.Object accessible;
			
			string[] names = new string [] { "First item", "Second Item", "Last Item" };
			if (widget != null)
				accessible = GetAccessible (type, names, widget);
			else
				accessible = GetAccessible (type, names);
			
			StatesComboBox (accessible);

			Assert.AreEqual (1, accessible.NAccessibleChildren, "numChildren; children roles:" + childrenRoles (accessible));
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			
			Atk.Action atkAction = CastToAtkInterface <Atk.Action> (accessible);
			InterfaceAction (type, atkAction, accessible);
			
			Atk.Object menuChild = accessible.RefAccessibleChild (0);
			CheckComboBoxMenuChild (menuChild, names);

			//FIXME:
			//Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			//InterfaceSelection (atkSelection, names, accessible, type);
		}
		
		[Test]
		public void TabControl () { RunInGuiThread (RealTabControl); }
		public void RealTabControl ()
		{
			BasicWidgetType type = BasicWidgetType.TabControl;
			Atk.Object accessible = null;
			string [] names = new string [] { "Page1", "Page2" };

			RunInGuiThread (delegate () {
				accessible = GetAccessible (type, names, true);
			});
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);

			Atk.Selection atkSelection = CastToAtkInterface <Atk.Selection> (accessible);
			InterfaceSelection (atkSelection, names, accessible, type);
			
			Assert.AreEqual (names.Length, accessible.NAccessibleChildren, "TabControl numChildren");
			BasicWidgetType childType = BasicWidgetType.TabPage;
			Atk.Object child1 = accessible.RefAccessibleChild (0);
			PropertyRole (childType, child1);
			Atk.Text atkText = null;
			RunInGuiThread (delegate () {
				atkText = CastToAtkInterface<Atk.Text> (child1);
			});
			Assert.AreEqual (5, atkText.CharacterCount, "CharacterCount");
			Assert.AreEqual ("page1", atkText.GetText (0, 5), "GetText #1");
			Assert.AreEqual ("page1", atkText.GetText (0, -1), "GetText #2");
		}

		[Test]
		public void PictureBox ()
		{
			BasicWidgetType type = BasicWidgetType.PictureBox;
			Atk.Object accessible;

			string name = "test";
			accessible = GetAccessibleThatEmbedsAnImage (type, name, true);
			
			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);

			PropertyRole (type, accessible);
			
			Assert.AreEqual (0, accessible.NAccessibleChildren, "Button numChildren");

			Parent (type, accessible);

			Atk.Image atkWithoutImage, atkWithImage;
			atkWithImage = CastToAtkInterface <Atk.Image> (accessible);
			atkComponent = CastToAtkInterface<Atk.Component> (accessible);
			accessible = GetAccessibleThatEmbedsAnImage (type, name, false);
			atkWithoutImage = CastToAtkInterface <Atk.Image> (accessible);
			InterfaceImage (type, atkWithImage, atkComponent, atkWithoutImage);
		}

		//[Test]
		public void ListView ()
		{
			BasicWidgetType type = BasicWidgetType.ListView;
			Atk.Object accessible;

			string name = "<table><th><td>Title</td><td>Author</td><td>year</td></th>"+
				"<tr><td>Non-C#</td>"+
				"<tr><td>Programming Windows</td><td>Petzold, Charles</td><td>1998</td></tr>"+
				"<tr><td>Code: The Hidden Language of Computer Hardware and Software</td><td>Petzold, Charles</td><td>2000</td></tr>"+
				"<tr><td>Coding Techniques for Microsoft Visual Basic .NET</td><td>Connell, John</td><td>2001</td></tr>"+
				"</tr><tr><td>C#</td>"+
				"<tr><td>Programming Windows with C#</td><td>Petzold, Charles</td><td>2001</td></tr>"+
				"<tr><td>C# for Java Developers</td><td>Jones, Allen &amp; Freeman, Adam</td><td>2002</td></tr>"+
				"</tr></table>";
			accessible = GetAccessible (type, name, true);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);
			InterfaceComponent (type, atkComponent);
			InterfaceComponent (type, atkComponent);
			
			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Focusable,
			  Atk.StateType.ManagesDescendants,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);

			PropertyRole (type, accessible);

			Atk.Table atkTable = CastToAtkInterface<Atk.Table> (accessible);
			Assert.AreEqual (ValidNChildrenForAListView, accessible.NAccessibleChildren, "ListView numChildren");
			Atk.Object header = accessible.RefAccessibleChild (0);
			Assert.AreEqual (Atk.Role.TableColumnHeader, header.Role, "Child 0 role");
			Atk.Object child1 = FindObjectByName (accessible, "Programming Windows with C#");
			int child1Index = child1.IndexInParent;
			Assert.IsTrue (child1Index >= 0, "Child 1 index > 0");
			InterfaceText (child1, "Programming Windows with C#");
			Assert.IsNotNull (child1, "FindObjectByName #1");
			Assert.AreEqual (Atk.Role.TableCell, child1.Role, "Child 1 role");
			Atk.Object group = FindObjectByName (accessible, "C#");
			int groupIndex = group.IndexInParent;
			Assert.IsTrue (groupIndex >= 0, "Group index > 0");
			Assert.IsFalse (child1Index == groupIndex, "Child should have a different index from its group");

			InterfaceText (group, "C#");
			Relation (Atk.RelationType.NodeChildOf, child1, group);

			Assert.AreEqual (3, atkTable.NColumns, "Table NumColumns");
			Assert.AreEqual (1, atkTable.GetRowAtIndex (groupIndex), "GetRowAtIndex");
			Assert.AreEqual (0, atkTable.GetColumnAtIndex (groupIndex), "GetColumnAtIndex");
			Assert.AreEqual (group, atkTable.RefAt (1, 0), "ListView RefAt");
		}

		[Test]
		public void Window () { RunInGuiThread (RealWindow); }
		public void RealWindow ()
		{
			BasicWidgetType type = BasicWidgetType.Window;
			Atk.Object accessible;
			
			string name = "test";
			accessible = GetAccessible (type, name, true);
			
			Atk.Component atkComponent = CastToAtkInterface <Atk.Component> (accessible);

			InterfaceComponent (type, atkComponent);
			
			PropertyRole (type, accessible);
			Parent (type, accessible);

			States (accessible,
			  Atk.StateType.Enabled,
			  Atk.StateType.Sensitive,
			  Atk.StateType.Resizable,
			  Atk.StateType.Showing,
			  Atk.StateType.Visible);
		}
		

	}
}
