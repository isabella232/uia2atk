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
using System.Runtime.InteropServices;
using System.Xml;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	
	[TestFixture]
	public class GailTester : AtkTests {

		static GailTestApp.MovingThread guiThread = null;
		
		static GailTester ()
		{
			try {
				GLib.Global.ProgramName = "nunit-console";
				guiThread = new GailTestApp.MovingThread ();
				guiThread.NotMainLoopDeleg = Gtk.Application.Init;
				guiThread.Start ();
				guiThread.JoinUntilSuspendedByMainLoop ();
	
				GailTestApp.MainClass.StartRemotely (guiThread);
				System.Threading.Thread.Sleep (1000);
				EventMonitor.Start ();
				System.Threading.Thread.Sleep (1000);
			}
			catch (Exception e) {
				Console.WriteLine ("Exception at tests initialization: " + e);
				throw;
			}
		}

		bool alreadyInGuiThread = false;
		
		public override void RunInGuiThread (System.Action d)
		{
			if (alreadyInGuiThread) {
				Console.WriteLine ();
				Console.WriteLine ("WARNING: You called RunInGuiThread nestedly.");
				d ();
				return;
			}
			alreadyInGuiThread = true;
			
			try {
				System.Threading.EventWaitHandle h = guiThread.CallDelegInMainLoop (d);
				h.WaitOne ();
				h.Close ();
				if (guiThread.ExceptionHappened != null)
					throw guiThread.ExceptionHappened;
			}
			finally {
				alreadyInGuiThread = false;
			}
			System.Threading.Thread.Sleep (2000);
		}

		private Dictionary <Atk.Object, Gtk.Widget> mappings = new Dictionary<Atk.Object, Gtk.Widget> ();

		public override void DisableWidget (Atk.Object accessible)
		{
			RunInGuiThread (delegate () {
				mappings [accessible].Sensitive = false;
			});
		}

		[DllImport("libgobject-2.0.so.0")]
		static extern void g_object_set_property (IntPtr obj, IntPtr name, ref GLib.Value val);

		public override void SetReadOnly (BasicWidgetType type, Atk.Object accessible, bool readOnly)
		{
			RunInGuiThread (delegate () {
				GLib.Value val = new GLib.Value (!readOnly);
				IntPtr native_name = GLib.Marshaller.StringToPtrGStrdup ("editable");
				g_object_set_property (mappings [accessible].Handle, native_name, ref val);
				GLib.Marshaller.Free (native_name);
			});
		}

		// This is a hack.  For some reason, our Tree Cell instances in
		// our Gtk.TreeView don't support AtkAction.  I suspect this
		// has something to do with the way the TreeView is
		// initialized, but I don't have the time to research this any
		// further.
		protected override void ExpandTreeView (Atk.Object obj)
		{
			RunInGuiThread (delegate {
				Gtk.TreeView widget = GailTestApp.MainClass.GiveMeARealTreeView ();
				widget.ExpandAll ();
			});
		}

		protected override void CollapseTreeView (Atk.Object obj)
		{
			RunInGuiThread (delegate {
				Gtk.TreeView widget = GailTestApp.MainClass.GiveMeARealTreeView ();
				widget.CollapseAll ();
			});
		}

		public override Atk.Object ActivateAdditionalForm (string name)
		{
			Gtk.Window win = new Gtk.Window (name);
			Gtk.Button button = new Gtk.Button ("test");
			win.Add (button);
			RunInGuiThread (delegate () {
				win.Show ();
				button.GrabFocus ();
			});
			mappings [win.Accessible] = win;
			return win.Accessible;
		}

		public override void RemoveAdditionalForm (Atk.Object obj)
		{
			RunInGuiThread (delegate () {
				Gtk.Window win = (Gtk.Window)mappings [obj];
				win.Hide ();
				GailTestApp.MainClass.GiveMeARealButton (true).GrabFocus ();
			});
		}

		public override void EnableWidget (Atk.Object accessible)
		{
			RunInGuiThread (delegate () {
				mappings [accessible].Sensitive = true;
			});
		}

		public override bool IsBGO561414Addressed ()
		{
			return false;
		}

		public override bool IsBGO567991Addressed ()
		{
			return false;
		}

		public override bool IsBGO574674Addressed ()
		{
			return false;
		}

		protected override bool SupportsLabeledBy (out string labelName)
		{
			labelName = null;
			return false;
		}

		public override bool IsBGO580460Addressed ()
		{
			return false;
		}

		public override bool IsBGO580452Addressed ()
		{
			return false;
		}

		public override void CloseContextMenu (Atk.Object accessible) {
			RunInGuiThread (delegate () {
				((Gtk.Menu)mappings [accessible]).Popdown ();
			});
		}

		protected override bool AllowsEmptyingSelectionOnComboBoxes { 
			get { return true; }
		}

		public override bool HasComboBoxSimpleLayout {
			get { return false; }
		}
		
		protected override bool TextBoxCaretInitiallyAtEnd { 
			get { return true; }
		}

		protected override bool TextBoxHasScrollBar { 
			get { return false; }
		}

		public override Atk.Object GetAccessible (BasicWidgetType type)
		{
			if (type != BasicWidgetType.ToolBar)
				throw new Exception ("Use another GetAccessible overload for this widget type");

			Gtk.Widget ret = GailTestApp.MainClass.GiveMeARealToolBar ();
			mappings [ret.Accessible] = ret;
			return ret.Accessible;
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string text)
		{
			return GetAccessible (type, text, true);
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string text, bool real)
		{
			return GetAccessible (type, text, null, real, false);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string text, object widget)
		{
			return GetAccessible (type, text, null, true, false);
		}

		public override Atk.Object GetAccessible (
		  BasicWidgetType type, List <MenuLayout> menu)
		{
			Gtk.MenuShell menushell;
			if (type != BasicWidgetType.ContextMenu)
				menushell = GailTestApp.MainClass.GiveMeARealMenuBar ();
			else
				menushell = new Gtk.Menu ();

			Gtk.Widget ret = null;
			RunInGuiThread (delegate {

				//cleanup
				if (menushell is Gtk.MenuBar)
					while (menushell.Children.Length > 0)
						menushell.Remove (menushell.Children [0]);

				ret = AddRecursively (menushell, menu, type);

				if (type == BasicWidgetType.ContextMenu) {
					((Gtk.Menu)ret).Popup (null, null, 
						delegate (Gtk.Menu themenu, out int x, out int y, out bool push_in) {
							GailTestApp.MainClass.GiveMeARealWindow ().GetPosition (out x, out y);
							push_in = false;
						}, 0, Gtk.Global.CurrentEventTime);
				}
			});
			
			mappings [ret.Accessible] = ret;
			return ret.Accessible;
		}

		private Gtk.Widget AddRecursively (Gtk.MenuShell shell, List <MenuLayout> menus, BasicWidgetType type) {
			Gtk.Widget toTest = null;
			AddRecursivelyAux (shell, menus, type, ref toTest, 0);
			if ((type == BasicWidgetType.MainMenuBar) ||
			    (type == BasicWidgetType.ContextMenu))
				return shell;
			return toTest;
		}
		
		private bool AddRecursivelyAux (Gtk.MenuShell shell, List <MenuLayout> menus, BasicWidgetType type, ref Gtk.Widget widget, int level)
		{
			if (menus.Count <= 0)
				return false;

			foreach (MenuLayout menu in menus) {
				Gtk.MenuItem menuitem = null;
				if (menu is MenuSeparator) {
					menuitem = new Gtk.SeparatorMenuItem ();
					if (type == BasicWidgetType.ChildMenuSeparator)
						widget = menuitem;
				} else {
					menuitem = new Gtk.MenuItem (menu.Label);
		
					Gtk.Menu menushell = new Gtk.Menu ();
					if (AddRecursivelyAux (menushell, menu.SubMenus, type, ref widget, level + 1)) {
						menuitem.Submenu = menushell;
						if ((widget == null) && (level == 0))
							widget = menuitem;
					} else {
						if (widget == null && type == BasicWidgetType.ChildMenu)
							widget = menuitem;
					}
				}
				
				shell.Append (menuitem);
				menuitem.Show ();
			}
			shell.ShowAll ();
			return true;
		}

		public override object CastToAtkInterface (Type t, Atk.Object accessible)
		{
			try {
				if (t == typeof (Atk.IAction)) {
					return Atk.ActionAdapter.GetObject (accessible.Handle, false);
				} else if (t == typeof (Atk.IText)) {
					return Atk.TextAdapter.GetObject (accessible.Handle, false);
				} else if (t == typeof (Atk.IComponent)) {
					return Atk.ComponentAdapter.GetObject (accessible.Handle, false);
				} else if (t == typeof (Atk.IEditableText)) {
					return Atk.EditableTextAdapter.GetObject (accessible.Handle, false);
				} else if (t == typeof (Atk.IImage)) {
					return Atk.ImageAdapter.GetObject (accessible.Handle, false);
				} else if (t == typeof (Atk.ITable)) {
					return Atk.TableAdapter.GetObject (accessible.Handle, false);
				} else if (t == typeof (Atk.ISelection)) {
					return Atk.SelectionAdapter.GetObject (accessible.Handle, false);
				} else if (t == typeof (Atk.IValue)) {
					return Atk.ValueAdapter.GetObject (accessible.Handle, false);
				} else {
					throw new NotImplementedException ("Couldn't cast to interface " + t.Name);
				}
			} catch (ArgumentException) {
				return null;
			}
		}
		
		public override I CastToAtkInterface <I> (Atk.Object accessible)
		{	
			try {
				if (typeof (I) == typeof (Atk.IAction)) {
					return Atk.ActionAdapter.GetObject (accessible.Handle, false) as I;
				} else if (typeof (I) == typeof (Atk.IText)) {
					return Atk.TextAdapter.GetObject (accessible.Handle, false) as I;
				} else if (typeof (I) == typeof (Atk.IComponent)) {
					return Atk.ComponentAdapter.GetObject (accessible.Handle, false) as I;
				} else if (typeof (I) == typeof (Atk.IEditableText)) {
					return Atk.EditableTextAdapter.GetObject (accessible.Handle, false) as I;
				} else if (typeof (I) == typeof (Atk.IImage)) {
					return Atk.ImageAdapter.GetObject (accessible.Handle, false) as I;
				} else if (typeof (I) == typeof (Atk.ITable)) {
					return Atk.TableAdapter.GetObject (accessible.Handle, false) as I;
				} else if (typeof (I) == typeof (Atk.ISelection)) {
					return Atk.SelectionAdapter.GetObject (accessible.Handle, false) as I;
				} else if (typeof (I) == typeof (Atk.IValue)) {
					return Atk.ValueAdapter.GetObject (accessible.Handle, false) as I;
				}
				throw new NotImplementedException ("Couldn't cast to interface " +
				  typeof (I).Name);
			} catch (ArgumentException) {
				return null;
			}
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] name)
		{
			return GetAccessible (type, name, -1, null, true);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] name, bool real)
		{
			return GetAccessible (type, name, -1, null, real);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] name, object widget)
		{
			return GetAccessible (type, name, -1, widget, true);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] name, int selected, object widget)
		{
			return GetAccessible (type, name, selected, widget, true);
		}

		private Atk.Object GetAccessible (BasicWidgetType type, string [] name, int selected, object widget, bool real)
		{
			Gtk.Widget gwidget = null;
			Atk.Object accessible = null;
			//this is because of this:
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
//Gtk-CRITICAL **: gtk_combo_box_append_text: assertion `GTK_IS_LIST_STORE (combo_box->priv->model)' failed
			if (!real)
				throw new NotSupportedException ("We cannot add items to a non-real ComboBox because of some GtkCritical");
			
			switch (type) {
			case BasicWidgetType.ComboBoxSimple:

				if (selected != -1)
					throw new NotImplementedException ();

				string treeViewStructure = "<table>";
				foreach (string item in name)
					treeViewStructure += "<tr><td>" + item + "</td></tr>";
				treeViewStructure += "</table>";
				return GetAccessible (BasicWidgetType.TreeView, treeViewStructure, true);

			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxDropDownEntry:

				if (!real)
					throw new NotSupportedException ("You, clown, we're gonna deprecate un-real support");

				if (type == BasicWidgetType.ComboBoxDropDownList)
					widget = GailTestApp.MainClass.GiveMeARealComboBox ();
				else
					widget = GailTestApp.MainClass.GiveMeARealComboBoxEntry ();

				RunInGuiThread (delegate {
					//FIXME: update this line when this bug is fixed: http://bugzilla.gnome.org/show_bug.cgi?id=324899
					((Gtk.ListStore)((Gtk.ComboBox) widget).Model).Clear ();

					foreach (string text in name) 
						((Gtk.ComboBoxText)widget).AppendText (text);

					if (selected != -1)
						((Gtk.ComboBox)widget).Active = selected;
				});
				
				break;
				
			case BasicWidgetType.TabControl:
				gwidget = new Gtk.Notebook ();
				// real not implemented yet
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealNotebook ();
				Gtk.Notebook notebook = (Gtk.Notebook)widget;
				if (!real) {
					foreach (string text in name)
						notebook.AppendPage (new Gtk.Label (text), new Gtk.Label (text));
					gwidget.ShowAll ();
				}
				break;
				
			default:
				throw new NotSupportedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}

			gwidget = widget as Gtk.Widget;
			accessible = gwidget.Accessible;
			mappings [accessible] = gwidget;
			
			return accessible;
		}

		public override Atk.Object GetAccessibleThatEmbedsAnImage (
		  BasicWidgetType type, string text, bool real)
		{
			return GetAccessible (type, text, null, real, true);
		}
		
		private void AddToTreeStore (Gtk.TreeStore store, Gtk.TreeIter [] iters, int i, XmlNode node)
		{
			XmlElement tr = node as XmlElement;
			if (tr == null)
				return;
			if (i > 0)
				iters [i] = store.AppendNode (iters [i - 1]);
			else
				iters [i] = store.AppendNode ();
			int j = 0;
			for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
				if (child.Name == "tr")
					AddToTreeStore (store, iters, i + 1, child);
				else if (child.Name == "td")
				store.SetValue (iters [i], j++, child.InnerText);
		}

		private Atk.Object GetAccessible (BasicWidgetType type, string text, object preWidget, bool real, bool embeddedImage)
		{
			Atk.Object accessible = null;
			Gtk.Widget widget = null;
			Gtk.Adjustment adj = new Gtk.Adjustment (50, 0, 100, 1, 10, 20);
			switch (type) {
			case BasicWidgetType.Label:
				widget = new Gtk.Label ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealLabel ();
				((Gtk.Label)widget).Text = text;
				break;
			case BasicWidgetType.NormalButton:
				widget = new Gtk.Button ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealButton (embeddedImage);
				((Gtk.Button)widget).Label = text;
				break;
			case BasicWidgetType.Window:
				widget = new Gtk.Window (text);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealWindow ();
				break;
			case BasicWidgetType.CheckBox:
				widget = new Gtk.CheckButton ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealCheckBox (embeddedImage);
				((Gtk.CheckButton)widget).Label = text;
				break;
			case BasicWidgetType.RadioButton:
				if (!real)
					throw new NotSupportedException ("We cannot use non-real radio buttons because of some wierd behaviour");
				
				widget = GailTestApp.MainClass.GiveMeARealRadioButton (embeddedImage);
				RunInGuiThread (delegate {
					((Gtk.CheckButton)widget).Label = text;
				});
				break;
			case BasicWidgetType.StatusBar:
				widget = new Gtk.Statusbar ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealStatusbar ();
				((Gtk.Statusbar)widget).Push (0, text);
				break;
			case BasicWidgetType.TextBoxEntry:
				if (!real)
					throw new NotSupportedException ();
				
				widget = GailTestApp.MainClass.GiveMeARealEntry (true);
				RunInGuiThread (delegate {
					((Gtk.Entry)widget).Text = text;
				});
				break;
			case BasicWidgetType.RichTextBox:
			case BasicWidgetType.TextBoxView:
				if (!real)
					throw new NotSupportedException ();
				
				widget = GailTestApp.MainClass.GiveMeARealTextView ();
				RunInGuiThread (delegate {
					((Gtk.TextView)widget).Buffer.Text = text;
				});
				break;
			case BasicWidgetType.PasswordCharTextBoxEntry:
				if (!real)
					throw new NotSupportedException ();
				
				widget = GailTestApp.MainClass.GiveMeARealEntry (false);
				RunInGuiThread (delegate {
					((Gtk.Entry)widget).Text = text;
				});
				break;
			case BasicWidgetType.HScrollBar:
				widget = new Gtk.HScrollbar (adj);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealHScrollbar ();
				break;
			case BasicWidgetType.VScrollBar:
				widget = new Gtk.VScrollbar (adj);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealVScrollbar ();
				break;
			case BasicWidgetType.ProgressBar:
				widget = new Gtk.ProgressBar ();
				((Gtk.ProgressBar)widget).Fraction = (adj.Value - adj.Lower) / (adj.Upper - adj.Lower);
				((Gtk.ProgressBar)widget).PulseStep = adj.StepIncrement;
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealProgressBar ();
				break;
			case BasicWidgetType.Spinner:
				widget = new Gtk.SpinButton (adj, 1, 2);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealSpinButton ();
				break;
			case BasicWidgetType.PictureBox:
				widget = new Gtk.Image ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealImage (embeddedImage);
				break;
			case BasicWidgetType.ListView:
			case BasicWidgetType.TreeView:
				Gtk.TreeStore store = null;
				List<string> columnNames = new List<String> ();
				XmlDocument xml = new XmlDocument ();
				xml.LoadXml (text);
				foreach (XmlElement th in xml.GetElementsByTagName ("th"))
					foreach (XmlElement td in th.GetElementsByTagName ("td"))
						columnNames.Add (td.InnerText);
					if (columnNames.Count == 0)
						columnNames.Add (string.Empty);
				if (columnNames.Count == 1)
					store = new Gtk.TreeStore (typeof (string));
				else if (columnNames.Count == 2)
					store = new Gtk.TreeStore (typeof (string), typeof (string));
				else if (columnNames.Count == 3)
					store = new Gtk.TreeStore (typeof (string), typeof (string), typeof (string));
				else
					Assert.Fail ("This test only supports 1-3 columns; got " + columnNames.Count);
				Gtk.TreeIter[] iters = new Gtk.TreeIter [8];
				XmlElement root = xml.DocumentElement;
				for (XmlNode node = root.FirstChild; node != null; node = node.NextSibling)
					if (node.Name == "tr")
						AddToTreeStore (store, iters, 0, node);
				widget = new Gtk.TreeView (store);
				if (real) {
					widget = GailTestApp.MainClass.GiveMeARealTreeView ();
					RunInGuiThread (delegate {
						((Gtk.TreeView)widget).Model = store;
					});
				}

				RunInGuiThread (delegate {
					Gtk.TreeView treeView = (Gtk.TreeView)widget;
					int i;
					for (i = treeView.Columns.Length - 1; i >= 0; i--)
						treeView.RemoveColumn (treeView.Columns [i]);
					i = 0;
					foreach (string columnName in columnNames) {
						Gtk.TreeViewColumn col = new Gtk.TreeViewColumn ();
						col.Title = columnName;
						treeView.AppendColumn (col);
						Gtk.CellRendererText cell = new Gtk.CellRendererText ();
						col.PackStart (cell, true);
						col.AddAttribute (cell, "text", i++);
					}
					((Gtk.TreeView)widget).HeadersClickable = text.Contains ("<th>");
					((Gtk.TreeView)widget).HeadersVisible = ((Gtk.TreeView)widget).HeadersClickable;
				});
				break;
			case BasicWidgetType.ContainerPanel:
				widget = new Gtk.Frame ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealFrame ();
				break;
			case BasicWidgetType.HSplitContainer:
				widget = new Gtk.HPaned ();
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealHPaned ();
				break;
			case BasicWidgetType.VTrackBar:
				widget = new Gtk.VScale (adj);
				if (real)
					widget = GailTestApp.MainClass.GiveMeARealVScale ();
				break;
			case BasicWidgetType.ComboBoxDropDownEntry:
			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxSimple:
				throw new NotSupportedException ("You have to use the GetObject overload that receives a name array");
			default:
				throw new NotImplementedException ("The widget finder backend still hasn't got support for " +
					type.ToString ());
			}
			
			accessible = widget.Accessible;
			mappings [accessible] = widget;
			
			return accessible;
		}
		
		protected override int ValidNumberOfActionsForAButton { get { return 3; } }
		protected override int ValidNChildrenForAListView { get { return 24; } }
		protected override bool TreeViewHasHeader { get { return true; } }
		protected override int ValidNChildrenForASimpleStatusBar { get { return 1; } }
		protected override int ValidNChildrenForAScrollBar { get { return 0; } }

		public override Atk.Object GetTopLevelRootItem () {
			return GailTestApp.MainClass.GiveMeARealWindow ().Accessible.Parent;
		}
		
		[TestFixtureTearDown]
		public void End () 
		{
			try {
				EventMonitor.Stop ();
				GailTestApp.MainClass.Kill (guiThread);
			
				//hack: let's wait for the Gtk.Application env to finish gracefully and then we abort the thread
				System.Threading.Thread.Sleep (1000);

				guiThread.Dispose ();
			}
			catch (Exception e) {
				Console.WriteLine ("Exception occurred in TestFixtureDown:" + e);
				throw;
			}
		}

		
		
	}
}
