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
//      Calvin Gaisford <cgaisford@novell.com>
// 

using System;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using SWF = System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

using NUnit.Framework;

namespace UiaAtkBridgeTest
{
	public abstract class BridgeTester : AtkTests
	{
		protected SWF.GroupBox gb1 = new SWF.GroupBox ();
		protected SWF.GroupBox gb2 = new SWF.GroupBox ();
		protected SWF.RadioButton rad1 = new SWF.RadioButton ();
		protected SWF.RadioButton rad2 = new SWF.RadioButton ();
		protected SWF.RadioButton rad3 = new SWF.RadioButton ();
		protected SWF.RadioButton rad4 = new SWF.RadioButton ();
		protected SWF.RadioButton radWithImage = new SWF.RadioButton ();
		protected List<SWF.RadioButton> radios = new List<SWF.RadioButton> ();
		protected int currentRadio = -1;
		protected SWF.ListBox lb1 = new SWF.ListBox ();
		protected SWF.CheckedListBox clb1 = new SWF.CheckedListBox ();
		protected SWF.ComboBox cbDD = new SWF.ComboBox ();
		protected SWF.ComboBox cbSim = new SWF.ComboBox ();
		protected SWF.ComboBox cbDDL = new SWF.ComboBox ();
		protected SWF.Label lab1 = new SWF.Label ();
		protected SWF.LinkLabel linklab1 = new SWF.LinkLabel ();
		protected SWF.Button butWithoutImage = new SWF.Button ();
		protected SWF.Button butWithImage = new SWF.Button ();
		protected SWF.CheckBox chkWithoutImage = new SWF.CheckBox ();
		protected SWF.CheckBox chkWithImage = new SWF.CheckBox ();
		protected SWF.StatusBar sb1 = new SWF.StatusBar ();
		protected SWF.StatusStrip ss1 = new SWF.StatusStrip ();
		protected SWF.ProgressBar pb1 = new SWF.ProgressBar ();
		protected SWF.NumericUpDown nud1 = new SWF.NumericUpDown ();
		protected SWF.DomainUpDown dud1 = new SWF.DomainUpDown ();
		protected SWF.Form form = new SWF.Form ();
		protected SWF.MenuStrip menuStrip1 = new SWF.MenuStrip ();
		protected SWF.ContextMenuStrip contextMenu = new SWF.ContextMenuStrip ();
		protected SWF.ContextMenu contextMenuDep = new SWF.ContextMenu ();
		protected SWF.Panel panel1 = new SWF.Panel ();
		protected SWF.PictureBox pboxWithoutImage = new SWF.PictureBox ();
		protected SWF.PictureBox pboxWithImage = new SWF.PictureBox ();
		protected SWF.TextBox tbx1 = new SWF.TextBox ();
		protected SWF.TextBox tbx2 = new SWF.TextBox ();
		protected SWF.ToolStrip toolStrip = new SWF.ToolStrip ();
		protected SWF.ToolStripButton toolStripButton = new SWF.ToolStripButton ("TestTSB");
		protected SWF.ToolStripComboBox toolStripComboBoxSim = new SWF.ToolStripComboBox ();
		protected SWF.ToolStripComboBox toolStripComboBoxDDL = new SWF.ToolStripComboBox ();
		protected SWF.ToolStripComboBox toolStripComboBoxDD = new SWF.ToolStripComboBox ();
		protected SWF.ToolStripTextBox toolStripTextBox1 = new SWF.ToolStripTextBox ();
		protected SWF.ToolStripTextBox toolStripTextBox2 = new SWF.ToolStripTextBox ();
		protected SWF.ToolStripLabel tsl1 = new SWF.ToolStripLabel ();
		protected SWF.ToolStripProgressBar tspb1 = new SWF.ToolStripProgressBar ();
		protected SWF.ListView lv1 = new SWF.ListView ();
		protected SWF.ToolStripDropDownButton tsddb = new SWF.ToolStripDropDownButton ();
		protected SWF.ToolStripSplitButton tssb = new SWF.ToolStripSplitButton ();
		protected SWF.ToolBar toolBar = new SWF.ToolBar ();
		protected SWF.ToolBarButton toolBarButton1 = new SWF.ToolBarButton ("TestPushButton");
		protected SWF.ToolBarButton toolBarButton2 = new SWF.ToolBarButton ("TestDropDownButton");
		protected SWF.ToolBarButton toolBarButtonWithImage = new SWF.ToolBarButton ("TestIMG");
		protected SWF.TabControl tabControl = new SWF.TabControl ();
		protected SWF.TreeView treeView = new SWF.TreeView ();
		protected SWF.DateTimePicker dateTimePicker = new SWF.DateTimePicker ();
		protected SWF.SplitContainer splitContainer = new SWF.SplitContainer ();
		protected SWF.RichTextBox richTextBox = new SWF.RichTextBox ();
		protected SWF.TrackBar trackBar = new SWF.TrackBar ();
		protected SWF.FlowLayoutPanel flp = new SWF.FlowLayoutPanel ();
		protected SWF.TableLayoutPanel tlp = new SWF.TableLayoutPanel ();
		protected SWF.MonthCalendar monthCalendar = new SWF.MonthCalendar ();
		protected SWF.ContainerControl containerControl = new SWF.ContainerControl ();
		protected SWF.DataGrid datagrid = new SWF.DataGrid ();
		protected SWF.DataGridView datagridView = new SWF.DataGridView ();
		protected SWF.MaskedTextBox maskedTextBox = new SWF.MaskedTextBox ();
		protected SWF.PropertyGrid pgrid = new SWF.PropertyGrid ();

		protected int lastClickedLink = -1;

		public static void InitializeA11y ()
		{
			//same effect as Application.Run() (the important bit is this causes a call to ApplicationStarts() ):
			AutomationInteropProvider.RaiseAutomationEvent (null, null);
		}
		
		[TestFixtureSetUp]
		public virtual void BridgeTesterInit ()
		{
			InitializeA11y ();
			
			form.Show ();

			string uiaQaPath = Misc.LookForParentDir ("*.gif");
			if (uiaQaPath == null)
				throw new Exception ("Path for images not found");
			string imgPath = System.IO.Path.Combine (uiaQaPath, "opensuse60x38.gif");

			SWF.ImageList imageList = new SWF.ImageList ();
			imageList.Images.Add (Image.FromFile (imgPath));
			toolBar.ImageList = imageList;
			
			butWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			butWithImage.AutoSize = true;

			chkWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			butWithImage.AutoSize = true;

			radWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			radWithImage.AutoSize = true;
			
			pboxWithImage.Image = System.Drawing.Image.FromFile (imgPath);
			pboxWithImage.AutoSize = true;

			cbDDL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cbDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
			cbSim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			tbx1.Multiline = false;
			toolStripTextBox1.Multiline = false;
			tbx2.Multiline = true;
			toolStripTextBox2.Multiline = true;
			toolStripComboBoxSim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			toolStripComboBoxDDL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			toolStripComboBoxDD.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;

			toolStrip.Items.Add (toolStripButton);
			toolStrip.Items.Add (toolStripComboBoxSim);
			toolStrip.Items.Add (toolStripComboBoxDDL);
			toolStrip.Items.Add (toolStripComboBoxDD);
			toolStrip.Items.Add (tsl1);
			toolStrip.Items.Add (tspb1);
			toolStrip.Items.Add (tsddb);
			toolStrip.Items.Add (tssb);
			toolStrip.Items.Add (toolStripTextBox1);
			toolStrip.Items.Add (toolStripTextBox2);
			form.Controls.Add (toolStrip);

			toolBar.Buttons.Add (toolBarButton1);
			toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.PushButton;
			toolBar.Buttons.Add (toolBarButton2);
			toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
			toolBar.Buttons.Add (toolBarButtonWithImage);
			toolBarButtonWithImage.ImageIndex = 0;
			form.Controls.Add (toolBar);

			pgrid.CommandsVisibleIfAvailable = true;
			pgrid.Dock = SWF.DockStyle.Top;
			pgrid.Text = "PGrid TestText";
			pgrid.SelectedObject = lab1;
			form.Size = new Size (800, 600);
			form.Controls.Add (pgrid);

			linklab1.Links [0].Visited = true;
			linklab1.Text = "openSUSE:www.opensuse.org \n\n webmail:gmail.novell.com";
			linklab1.Links.Add (9, 16, "www.opensuse.org");
			linklab1.Links.Add (35, 16, "gmail.novell.com");
			linklab1.LinkClicked += LinkLabelClicked;
			linklab1.Links [0].Enabled = false;

			//TODO: test also contextmenus not attached to widgets
			lab1.ContextMenuStrip = contextMenu;

			butWithoutImage.ContextMenu = contextMenuDep;
			
			gb1.Controls.Add (rad1);
			gb1.Controls.Add (rad2);
			gb2.Controls.Add (rad3);
			gb2.Controls.Add (rad4);
			form.Controls.Add (gb1);
			form.Controls.Add (gb2);
			form.Controls.Add (lb1);
			form.Controls.Add (clb1);
			form.Controls.Add (cbDDL);
			form.Controls.Add (cbDD);
			form.Controls.Add (cbSim);
			lab1.Text = "test";
			form.Controls.Add (lab1);
			form.Controls.Add (linklab1);
			form.Controls.Add (butWithoutImage);
			form.Controls.Add (butWithImage);
			form.Controls.Add (chkWithoutImage);
			form.Controls.Add (chkWithImage);
			form.Controls.Add (sb1);
			form.Controls.Add (ss1);
			form.Controls.Add(menuStrip1);
			form.MainMenuStrip = menuStrip1;
			form.Controls.Add (pb1);
			form.Controls.Add (nud1);
			form.Controls.Add (dud1);
			form.Controls.Add (panel1);
			form.Controls.Add (pboxWithoutImage);
			form.Controls.Add (pboxWithImage);
			form.Controls.Add (tbx1);
			form.Controls.Add (tbx2);
			form.Controls.Add (dateTimePicker);
			form.Controls.Add (splitContainer);
			form.Controls.Add (richTextBox);
			form.Controls.Add (trackBar);
			form.Controls.Add (monthCalendar);
			form.Controls.Add (maskedTextBox);
			// TODO: Move following lines to the end of ListView test to test view switching
			lv1.View = SWF.View.Details;
			lv1.ShowGroups = true;
			form.Controls.Add (lv1);
			form.Controls.Add (treeView);
			form.Controls.Add (radWithImage);
			form.Controls.Add (datagrid);
			datagridView.AllowUserToAddRows = false;
			form.Controls.Add (datagridView);
			rad1.Text = "rad1";
			rad2.Text = "rad2";
			rad3.Text = "rad3";
			rad4.Text = "rad4";
			radios.Add (rad1);
			radios.Add (rad2);
			radios.Add (rad3);
			radios.Add (rad4);
			form.Controls.Add (tabControl);
			form.Controls.Add (flp);
			form.Controls.Add (tlp);
			form.Controls.Add (containerControl);
			form.Text = "MainWindow";
			SWF.Application.EnableVisualStyles ();
		}
		
		private SWF.RadioButton GiveMeARadio (string name) {
			if (currentRadio == 3) {
				currentRadio = -1;
			}
			
			currentRadio++;
			radios [currentRadio].Name = name;
			return radios [currentRadio];
		}
		
		public override void RunInGuiThread (System.Action d)
		{
			d ();
			GlibSync ();
		}

		public override Atk.Object GetTopLevelRootItem ()
		{
			return UiaAtkBridge.TopLevelRootItem.Instance;
		}

		public override bool IsBGO561414Addressed ()
		{
			return true;
		}

		public override bool IsBGO567991Addressed ()
		{
			return true;
		}

		public override bool IsBGO574674Addressed ()
		{
			return true;
		}

		public override bool IsBGO580460Addressed ()
		{
			return true;
		}

		public override bool IsBGO580452Addressed ()
		{
			return true;
		}

		public override void CloseContextMenu (Atk.Object accessible)
		{
			var comp = mappings [accessible];
			if (comp is SWF.ContextMenuStrip)
				((SWF.ContextMenuStrip)mappings [accessible]).Close ();
			else if (comp is SWF.ContextMenu)
				((SWF.ContextMenu)mappings [accessible]).Dispose ();
		}

		protected override bool AllowsEmptyingSelectionOnComboBoxes { 
			get { return false; }
		}
		
		public override bool HasComboBoxSimpleLayout {
			get { return true; }
		}
		
		protected override bool TextBoxCaretInitiallyAtEnd { 
			get { return false; }
		}

		protected override bool TextBoxHasScrollBar { 
			get { return true; }
		}

		protected override bool SupportsLabeledBy (out string labelName)
		{
			labelName = lab1.Text;
			return true;
		}

		private static Dictionary <Atk.Object, System.ComponentModel.Component> mappings = 
			new Dictionary<Atk.Object, System.ComponentModel.Component> ();
		
		public override void DisableWidget (Atk.Object accessible)
		{
			System.ComponentModel.Component comp = mappings [accessible];

			if (comp is SWF.Control)
				((SWF.Control)comp).Enabled = false;
			else if (comp is SWF.ToolStripItem)
				((SWF.ToolStripItem)comp).Enabled = false;
			else if (comp is SWF.ToolBarButton)
				((SWF.ToolBarButton)comp).Enabled = false;
			else
				throw new NotSupportedException ();
		}

		public override void EnableWidget (Atk.Object accessible)
		{
			System.ComponentModel.Component comp = mappings [accessible];

			if (comp is SWF.Control)
				((SWF.Control)comp).Enabled = true;
			else if (comp is SWF.ToolStripItem)
				((SWF.ToolStripItem)comp).Enabled = true;
			else if (comp is SWF.ToolBarButton)
				((SWF.ToolBarButton)comp).Enabled = true;
			else
				throw new NotSupportedException ();
		}
		
		public override void SetReadOnly (BasicWidgetType type, Atk.Object accessible, bool readOnly)
		{
			System.ComponentModel.Component comp = null;

			if (!mappings.TryGetValue (accessible, out comp)) {
				// Is a fake provider
				if (type == BasicWidgetType.ListView) {
					lv1.LabelEdit = !readOnly;
				} else
					throw new NotSupportedException ();
			} else {
				if (comp is SWF.UpDownBase)
					((SWF.UpDownBase)comp).ReadOnly = readOnly;
				else if (comp is SWF.TextBox)
					((SWF.TextBox)comp).ReadOnly = readOnly;
				else if (comp is SWF.RichTextBox)
					((SWF.RichTextBox)comp).ReadOnly = readOnly;
				else if (comp is SWF.ToolStripTextBox)
					((SWF.ToolStripTextBox)comp).ReadOnly = readOnly;
				else
					throw new NotSupportedException ();
			}
		}

		public override Atk.Object ActivateAdditionalForm (string name)
		{
			SWF.Form f = new SWF.Form ();
			f.Text = name;
			SWF.Button button = new SWF.Button ();
			f.Controls.Add (button);
			f.Show ();
			button.Focus ();
			return GetAdapterForWidget (f);
		}

		public override void RemoveAdditionalForm (Atk.Object obj)
		{
			if (obj != null) {
				SWF.Form f = (SWF.Form)mappings [obj];
				f.Close ();
			}
			butWithImage.Focus ();
		}
		public override object CastToAtkInterface (Type t, Atk.Object accessible)
		{
			if (t == typeof (Atk.IAction)) {
				if (!(accessible is Atk.IActionImplementor))
					return null;
				return Atk.ActionAdapter.GetObject (accessible.Handle, false);
			} else if (t == typeof (Atk.IComponent)) {
				if (!(accessible is Atk.IComponentImplementor))
					return null;
				return Atk.ComponentAdapter.GetObject (accessible.Handle, false);
			} else if (t == typeof (Atk.IEditableText)) {
				if (!(accessible is Atk.IEditableTextImplementor))
					return null;
				return Atk.EditableTextAdapter.GetObject (accessible.Handle, false);
			} else if (t == typeof (Atk.IImage)) {
				if (!(accessible is Atk.IImageImplementor))
					return null;
				return Atk.ImageAdapter.GetObject (accessible.Handle, false);
			} else if (t == typeof (Atk.ITable)) {
				if (!(accessible is Atk.ITableImplementor))
					return null;
				return Atk.TableAdapter.GetObject (accessible.Handle, false);
			} else if (t == typeof (Atk.IText)) {
				if (!(accessible is Atk.ITextImplementor))
					return null;
				return Atk.TextAdapter.GetObject (accessible.Handle, false);
			} else if (t == typeof (Atk.ISelection)) {
				if (!(accessible is Atk.ISelectionImplementor))
					return null;
				return Atk.SelectionAdapter.GetObject (accessible.Handle, false);
			} else if (t == typeof (Atk.IValue)) {
				if (!(accessible is Atk.IValueImplementor))
					return null;
				return Atk.ValueAdapter.GetObject (accessible.Handle, false);
			}
			throw new NotImplementedException ("Couldn't cast to interface " + t.Name);
		}
		
		public override I CastToAtkInterface <I> (Atk.Object accessible)
		{
			try {
				if (typeof (I) == typeof (Atk.IComponent)) {
					return new Atk.ComponentAdapter ((Atk.IComponentImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.IText)) {
					return new Atk.TextAdapter ((Atk.ITextImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.IAction)) {
					return new Atk.ActionAdapter ((Atk.IActionImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.ITable)) {
					return new Atk.TableAdapter ((Atk.ITableImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.IValue)) {
					return new Atk.ValueAdapter ((Atk.IValueImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.IImage)) {
					return new Atk.ImageAdapter ((Atk.IImageImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.ISelection)) {
					return new Atk.SelectionAdapter ((Atk.ISelectionImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.IEditableText)) {
					return new Atk.EditableTextAdapter ((Atk.IEditableTextImplementor)accessible) as I;
				} else if (typeof (I) == typeof (Atk.IStreamableContent)) {
					return new Atk.StreamableContentAdapter ((Atk.IStreamableContentImplementor)accessible) as I;
				}
				throw new NotImplementedException ("Couldn't cast to interface " +
				typeof (I).Name);
			} catch (InvalidCastException) {
				return null;
			}
		}

		public override Atk.Object GetAccessible (BasicWidgetType type)
		{
			if (type != BasicWidgetType.ToolBar)
				throw new Exception ("Use another GetAccessible overload for this widget type");

			return GetAdapterForWidget (toolBar);
		}
		
		public override Atk.Object GetAccessibleThatEmbedsAnImage (BasicWidgetType type, string name, bool real)
		{
			return GetAccessible (type, name, null, real, true);
		}

		public static IRawElementProviderSimple ProviderFactoryGetProvider (System.ComponentModel.Component widget)
		{
			return ProviderFactoryGetProvider (widget, true, false);
		}

		public static IRawElementProviderSimple ProviderFactoryGetProvider (System.ComponentModel.Component widget,
			bool initialize,
			bool forceInitializeChildren)
		{
			const string UIA_WINFORMS_ASSEMBLY = 
			  "UIAutomationWinforms, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812";
			MethodInfo getProviderMethod;
			Assembly mwf_providers = null;
			IRawElementProviderSimple provider = null;
			try {
				mwf_providers = Assembly.Load (UIA_WINFORMS_ASSEMBLY);
			} catch { }
			
			if (mwf_providers == null)
				throw new Exception ("Warning: Couldn't locate UIAutomationWinforms");

			const string UIA_WINFORMS_TYPE     = "Mono.UIAutomation.Winforms.ProviderFactory";
			const string UIA_WINFORMS_METHOD   = "GetProvider";
			try {
				Type global_type = mwf_providers.GetType (UIA_WINFORMS_TYPE, false);
				if (global_type != null) {
					getProviderMethod = global_type.GetMethod (UIA_WINFORMS_METHOD, 
						new System.Type [] {
							typeof (System.ComponentModel.Component),
							typeof (bool),
							typeof (bool)});
					if (getProviderMethod != null)
						provider = (IRawElementProviderSimple) getProviderMethod.Invoke (null, new object [] {widget, initialize, forceInitializeChildren});
					else
						throw new Exception (String.Format ("Method {0} not found in type {1}.",
						                                    UIA_WINFORMS_METHOD, UIA_WINFORMS_TYPE));
				}
				else
					throw new Exception (String.Format ("Type {0} not found in assembly {1}.",
					                                    UIA_WINFORMS_TYPE, UIA_WINFORMS_ASSEMBLY));
			} catch (Exception ex) {
				throw new Exception ("Error setting up UIA: " + ex);
			}
			return provider;
		}

		public static Atk.Object GetAdapterForWidget (System.ComponentModel.Component widget)
		{
			if (widget == null)
				throw new ArgumentNullException ("widget");
			
			IRawElementProviderSimple provider = ProviderFactoryGetProvider (widget);
			Assert.IsNotNull (provider, "ProviderFactory returned null for this widget");
			Atk.Object acc = GetAdapterForProvider (provider);
			mappings [acc] = widget;
			
//NOTE: this code fragment below may be useful for discovering child items which don't have a test for themselves alone
//			System.Reflection.PropertyInfo itemsProp = 
//			  new List<System.Reflection.PropertyInfo> (widget.GetType ().GetProperties ()).
//			  Find (delegate (System.Reflection.PropertyInfo p) {
//				return p.Name == "Items";
//			  });
//			if (itemsProp != null) {
//				object value = itemsProp.GetValue (widget, null);
//				if (value is ICollection)
//					foreach (object o in (ICollection)value) 
//						if (o is System.ComponentModel.Component)
//							GetAdapterForWidget ((System.ComponentModel.Component)o, false);
//			}
			return acc;
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names)
		{
			return GetAccessible (type, names, -1, null, true);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names, int selected, object widget)
		{
			return GetAccessible (type, names, selected, widget, true);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names, object widget)
		{
			return GetAccessible (type, names, -1, widget, true);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string name, object widget)
		{
			return GetAccessible (type, name, widget, true, false);
		}

		public override Atk.Object GetAccessible (BasicWidgetType type, string [] names, bool real)
		{
			return GetAccessible (type, names, -1, null, real);
		}
		
		public Atk.Object GetAccessible (BasicWidgetType type, string [] names, int selected, object widget, bool real)
		{
			Atk.Object accessible = null;
			
			switch (type) {
			case BasicWidgetType.ListBox:
			case BasicWidgetType.VScrollBar:
			case BasicWidgetType.HScrollBar:
				SWF.ListBox listBox = new SWF.ListBox ();
				if (real)
					listBox = lb1;
				listBox.Items.Clear ();
				listBox.ScrollAlwaysVisible = (type == BasicWidgetType.VScrollBar);
				listBox.HorizontalScrollbar = (type == BasicWidgetType.HScrollBar);
				foreach (string item in names)
					listBox.Items.Add (item);
			
				if (real)
					accessible = GetAdapterForWidget (listBox);
				else
					accessible = new UiaAtkBridge.List ((IRawElementProviderFragmentRoot) 
					                                    ProviderFactoryGetProvider (listBox, true, true));
				break;

			case BasicWidgetType.CheckedListBox:
				SWF.CheckedListBox clistBox = new SWF.CheckedListBox ();
				if (real)
					clistBox = clb1;
				clistBox.Items.Clear ();
				foreach (string item in names)
					clistBox.Items.Add (item);
			
				if (real)
					accessible = GetAdapterForWidget (clistBox);
				else
					accessible = new UiaAtkBridge.List ((IRawElementProviderFragmentRoot) 
					                                    ProviderFactoryGetProvider (clistBox, true, true));
				break;

			case BasicWidgetType.ListView:
				lv1.Items.Clear ();
				lv1.Scrollable = false;
				foreach (string item in names)
					lv1.Items.Add (item);
				accessible = GetAdapterForWidget (lv1);
				break;

			case BasicWidgetType.DomainUpDown:
				dud1.Items.Clear ();
				foreach (string item in names)
					dud1.Items.Add (item);
			
				accessible = GetAdapterForWidget (dud1);
				break;

			case BasicWidgetType.TabControl:
				tabControl.TabPages.Clear ();
				foreach (string item in names)
					tabControl.TabPages.Add (item);
			
				accessible = GetAdapterForWidget (tabControl);
				break;

			case BasicWidgetType.ComboBoxSimple:
			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxDropDownEntry:
				if (!real)
					throw new NotSupportedException ("You, clown, we're gonna deprecate un-real support");

				System.ComponentModel.Component comp = null;
				if (widget != null) {
					comp = (System.ComponentModel.Component)widget;
				} else {
					if (type == BasicWidgetType.ComboBoxDropDownEntry)
						comp = cbDD;
					else if (type == BasicWidgetType.ComboBoxDropDownList)
						comp = cbDDL;
					else if (type == BasicWidgetType.ComboBoxSimple)
						comp = cbSim;
				}
				
				if (selected != -1) {
					//not implemented yet
					if (widget != null || type != BasicWidgetType.ComboBoxDropDownList)
						throw new NotImplementedException ();
					
					//in this case, we need to select an option *before* the combobox is shown/included
					//in the form so we cannot use the cbDDL
					comp = new SWF.ComboBox ();
					((SWF.ComboBox)comp).DropDownStyle = SWF.ComboBoxStyle.DropDownList;
				}
				
				if (comp is SWF.ComboBox) {
					SWF.ComboBox normalCombo = (SWF.ComboBox)comp;
					normalCombo.Items.Clear();
					foreach (string item in names)
						normalCombo.Items.Add (item);
				} else if (comp is SWF.ToolStripComboBox) {
					SWF.ToolStripComboBox stripCombo = (SWF.ToolStripComboBox)comp;
					stripCombo.Items.Clear();
					foreach (string item in names)
						stripCombo.Items.Add (item);
				} else
					throw new NotSupportedException ("This kind of ComboBox is not supported: " + comp.GetType ().Name);

				if (selected != -1) {
					((SWF.ComboBox)comp).SelectedIndex = selected;
					form.Controls.Add ((SWF.ComboBox)comp);
				}

				accessible = GetAdapterForWidget (comp);
				break;

			case BasicWidgetType.ToolStripSplitButton:
				tssb.Text = names [0];
				for (int i = 1; i < names.Length; i++) {
					SWF.ToolStripMenuItem item
					  = new SWF.ToolStripMenuItem ();
					item.Text = names [i];
					tssb.DropDownItems.Add (item);
				}
				accessible = GetAdapterForWidget (tssb);
				break;

			case BasicWidgetType.ToolStripDropDownButton:
				while (tsddb.DropDownItems.Count > 0)
					tsddb.DropDownItems.Remove (tsddb.DropDownItems [0]);
				
				foreach (string name in names) {
					SWF.ToolStripMenuItem item
					  = new SWF.ToolStripMenuItem ();
					item.Text = name;
					tsddb.DropDownItems.Add (item);
				}
				accessible = GetAdapterForWidget (tsddb);
				break;
			case BasicWidgetType.DateTimePicker:
				accessible = GetAdapterForWidget (dateTimePicker);
				break;
			default:
				throw new NotImplementedException ("This AtkTester overload doesn't handle this type of widget: " +
					type.ToString ());
			}

			GlibSync ();
			return accessible;
		}
		

		public override Atk.Object GetAccessible (BasicWidgetType type, string name)
		{
			return GetAccessible (type, name, null);
		}
		
		public override Atk.Object GetAccessible (BasicWidgetType type, string name, bool real)
		{
			return GetAccessible (type, name, null, real, false);
		}
		
		private Atk.Object GetAccessible (BasicWidgetType type, string name, object widget, bool real, bool embeddedImage)
		{
			Atk.Object accessible = null;
			string[] names = null;
			XmlDocument xml;
			XmlElement root;

			switch (type) {
			case BasicWidgetType.Label:
				SWF.Label lab = new SWF.Label ();
				if (real)
					lab = lab1;
				lab.Text = name;
				if (real)
					accessible = GetAdapterForWidget (lab);
				else
					accessible = new UiaAtkBridge.TextLabel (ProviderFactoryGetProvider (lab, true, true));
				break;
				
			case BasicWidgetType.NormalButton:
				SWF.Button but = new SWF.Button ();
				if (real)
					but = (embeddedImage ? butWithImage : butWithoutImage);
				but.Text = name.Replace ("_", "&"); // Gtk uses '_' to underline, SWF uses '&'
				if (!real)
					throw new NotSupportedException ("We don't support unreal anymore in tests");
				accessible = GetAdapterForWidget (but);
				break;

			case BasicWidgetType.ToolBarDropDownButton:
				if (!real)
					throw new NotSupportedException ("No unreal support for ToolbarButton");

				toolBarButton2.Text = name;
				accessible = GetAdapterForWidget (toolBarButton2);
				break;
				
			case BasicWidgetType.ToolBarPushButton:
				if (!real)
					throw new NotSupportedException ("No unreal support for ToolbarButton");

				SWF.ToolBarButton theButton = (embeddedImage) ? toolBarButtonWithImage : toolBarButton1;
				theButton.Text = name;
				accessible = GetAdapterForWidget (theButton);
				break;

			case BasicWidgetType.ToolStripButton:
				if (!real)
					throw new NotSupportedException ("No unreal support for ToolbarButton");

				toolStripButton.Text = name;
				accessible = GetAdapterForWidget (toolStripButton);
				break;
				
			case BasicWidgetType.Window:
				SWF.Form frm = new SWF.Form ();
				if (real)
					frm = form;
				frm.Name = name;
				if (real)
					accessible = GetAdapterForWidget (frm);
				else
					accessible = new UiaAtkBridge.Window (ProviderFactoryGetProvider (frm, true, true));
				break;
				
			case BasicWidgetType.CheckBox:
				SWF.CheckBox chk = new SWF.CheckBox ();
				if (real)
					chk = (embeddedImage ? chkWithImage : chkWithoutImage);
				chk.Text = name.Replace ("_", "&"); // Gtk uses '_' to underline, SWF uses '&'
				if (real)
					accessible = GetAdapterForWidget (chk);
				else
					accessible = new UiaAtkBridge.CheckBoxButton (ProviderFactoryGetProvider (chk, true, true));
				break;
				
			case BasicWidgetType.RadioButton:
				// the way to group radioButtons is dependent on their parent control
				SWF.RadioButton radio = 
					(embeddedImage ? radWithImage : GiveMeARadio (name));
				radio.Text = name.Replace ("_", "&"); // Gtk uses '_' to underline, SWF uses '&'
				if (real)
					accessible = GetAdapterForWidget (radio);
				else
					throw new NotSupportedException ("No un-real support for this");
				break;
				
			case BasicWidgetType.StatusBar:
				SWF.StatusBar sb = new SWF.StatusBar ();
				if (real)
					sb = sb1;
				sb.Text = name;
				if (real)
					accessible = GetAdapterForWidget (sb);
				else
					accessible = new UiaAtkBridge.TextContainer (ProviderFactoryGetProvider (sb, true, true));
				break;

			case BasicWidgetType.HScrollBar:
				names = new string [] { "First item", "Second Item", "Last Item", "A really, really long item that's here to try to ensure that we have a scrollbar, assuming that it's even possible to have a scrollbar just by having a relaly, really long item and we don't also have to perform some other function which I'm not aware of, like display the form on the screen" };
				accessible = GetAccessible (type, names, real);
				for (int i = accessible.NAccessibleChildren - 1; i >= 0; i--)
				{
					Atk.Object child = accessible.RefAccessibleChild (i);
					if (child.Role == Atk.Role.ScrollBar && child.RefStateSet().ContainsState(Atk.StateType.Horizontal))
					{
						accessible = child;
						break;
					}
				}
				if (accessible.Role != Atk.Role.ScrollBar)
					return null;
				break;

			case BasicWidgetType.VScrollBar:
				names = new string [100];
				for (int i = 0; i < 100; i++)
					names [i] = i.ToString();
				accessible = GetAccessible (type, names, real);
				for (int i = accessible.NAccessibleChildren - 1; i >= 0; i--)
				{
					Atk.Object child = accessible.RefAccessibleChild (i);
					if (child.Role == Atk.Role.ScrollBar && child.RefStateSet().ContainsState(Atk.StateType.Vertical))
					{
						accessible = child;
						break;
					}
				}
				if (accessible.Role != Atk.Role.ScrollBar)
					return null;
				break;

			case BasicWidgetType.ProgressBar:
				SWF.ProgressBar pb = new SWF.ProgressBar ();
				if (real) {
					pb = pb1;
					accessible = GetAdapterForWidget (pb);
				} else {
					accessible = new UiaAtkBridge.ProgressBar (ProviderFactoryGetProvider (pb, true, true));
				}
				break;

			case BasicWidgetType.Spinner:
				SWF.NumericUpDown nud = new SWF.NumericUpDown();
				if (real)
					nud = nud1;
				nud.Minimum = 0;
				nud.Maximum = 100;
				nud.Value = 50;
				if (real)
					accessible = GetAdapterForWidget (nud);
				else
					accessible = new UiaAtkBridge.SpinnerWithValue (ProviderFactoryGetProvider (nud, true, true));
				break;

			case BasicWidgetType.TextBoxEntry:
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");
				
				SWF.TextBox tbxEntry = tbx1;
				SWF.ToolStripTextBox tstbxEntry = null;

				if ((widget != null) && (widget is SWF.ToolStripTextBox)) {
					tstbxEntry = (SWF.ToolStripTextBox)widget;
					accessible = GetAdapterForWidget (tstbxEntry);
					tstbxEntry.Text = name;
				} else {
					accessible = GetAdapterForWidget (tbxEntry);
					tbx1.Text = name;
				}
				break;

			case BasicWidgetType.TextBoxView:
				if (!real)
					throw new NotSupportedException ("Not unreal support for TextBox");
				
				SWF.TextBox tbxView = tbx2;
				SWF.ToolStripTextBox tstbxView = null;
				if ((widget != null) && (widget is SWF.ToolStripTextBox)) {
					tstbxView = (SWF.ToolStripTextBox)widget;					
					accessible = GetAdapterForWidget (tstbxView);
					tstbxView.Text = name;
				} else {
					accessible = GetAdapterForWidget (tbxView);
					tbx2.Text = name;
					tbx2.ScrollBars = SWF.ScrollBars.Both;
				}
				break;

			case BasicWidgetType.RichTextBox:
				accessible = GetAdapterForWidget (richTextBox);
				richTextBox.Text = name;
				break;

			case BasicWidgetType.PictureBox:
				SWF.PictureBox pbox = new SWF.PictureBox ();
				if (real)
					pbox = (embeddedImage ? pboxWithImage: pboxWithoutImage);
				if (real)
					accessible = GetAdapterForWidget (pbox);
				else
					accessible = new UiaAtkBridge.Image (ProviderFactoryGetProvider (pbox, true, true));
				break;

			case BasicWidgetType.ToolStripLabel:
				tsl1.Text = name;
				accessible = GetAdapterForWidget (tsl1);
				break;

			case BasicWidgetType.ListView:
				xml = new XmlDocument ();
				xml.LoadXml (name);
				lv1.Groups.Clear ();
				lv1.Items.Clear ();
				lv1.Columns.Clear ();
				foreach (XmlElement th in xml.GetElementsByTagName ("th"))
					foreach (XmlElement td in th.GetElementsByTagName ("td"))
						lv1.Columns.Add (new SWF.ColumnHeader (td.InnerText));
				if (lv1.Columns.Count == 0)
					// TODO: Allow more than one column
					lv1.Columns.Add (new SWF.ColumnHeader ());
				root = xml.DocumentElement;
				for (XmlNode node = root.FirstChild; node != null; node = node.NextSibling) {
					if (node.Name == "tr") {
						bool group = false;
						for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
							if (child.Name == "tr")
								group = true;
						if (group)
							GetListViewGroup (node);
						else
							lv1.Items.Add (GetListViewItem (node));
					}
				}
				lv1.MultiSelect = false;
				accessible = GetAdapterForWidget (lv1);
				break;

			case BasicWidgetType.DataGridView:
				xml = new XmlDocument ();
				xml.LoadXml (name);
				datagridView.Rows.Clear ();
				datagridView.Columns.Clear ();
				// Columns
				foreach (XmlElement th in xml.GetElementsByTagName ("th")) {
					foreach (XmlElement td in th.GetElementsByTagName ("td"))
						datagridView.Columns.Add (GetDataGridViewColumnFromInnerText (td.InnerText));
				}
				// Rows
				foreach (XmlElement th in xml.GetElementsByTagName ("tr")) {
					int column = 0;					
					SWF.DataGridViewRow row = new SWF.DataGridViewRow ();
					foreach (XmlElement td in th.GetElementsByTagName ("td")) {
						row.Cells.Add (GetDataGridViewCellFromInnerText (td.InnerText, column));
						column++;
					}
					datagridView.Rows.Add (row);
				}
				accessible = GetAdapterForWidget (datagridView);
				break;
				
			case BasicWidgetType.TreeView:
				treeView.Scrollable = false;
				xml = new XmlDocument ();
				xml.LoadXml (name);
				treeView.BeginUpdate ();
				treeView.Nodes.Clear ();
				root = xml.DocumentElement;
				for (XmlNode node = root.FirstChild; node != null; node = node.NextSibling)
					if (node.Name == "tr")
						treeView.Nodes.Add (GetTreeNode (node));
				treeView.EndUpdate ();
				accessible = GetAdapterForWidget (treeView);
				break;
				
			case BasicWidgetType.ToolStripProgressBar:
				accessible = GetAdapterForWidget (tspb1);
				break;
			case BasicWidgetType.ContainerPanel://In the future we may return something different in Pane
				accessible = GetAdapterForWidget (panel1);
				break;
			case BasicWidgetType.ErrorProvider:
				if (!real)
					throw new NotSupportedException ("We don't support unreal anymore in tests");
					
				// the way to group radioButtons is dependent on their parent control
				SWF.ErrorProvider errorProvider = new SWF.ErrorProvider ();
				errorProvider.SetError (butWithImage, "Error message");
				accessible = GetAdapterForWidget (errorProvider);
				break;
			case BasicWidgetType.HSplitContainer:
				accessible = GetAdapterForWidget (splitContainer);
				break;
			case BasicWidgetType.VTrackBar:
				trackBar.Orientation = SWF.Orientation.Vertical;
				trackBar.Minimum = 0;
				trackBar.Maximum = 100;
				accessible = GetAdapterForWidget (trackBar);
				break;
			case BasicWidgetType.MonthCalendar:
				accessible = GetAdapterForWidget (monthCalendar);
				break;
			case BasicWidgetType.ListBox:
			case BasicWidgetType.CheckedListBox:
			case BasicWidgetType.ParentMenu:
			case BasicWidgetType.ComboBoxDropDownEntry:
			case BasicWidgetType.ComboBoxDropDownList:
			case BasicWidgetType.ComboBoxSimple:
				throw new NotSupportedException ("You have to use the GetObject overload that receives a name array");
			default:
				throw new NotImplementedException ("The widget finder backend still hasn't got support for " +
					type.ToString ());
			}

			GlibSync ();
			return accessible;
		}

		public override Atk.Object GetAccessible (
		  BasicWidgetType type, List <MenuLayout> menu)
		{
			System.ComponentModel.Component widget;

			SWF.ToolStripItemCollection menuItems = null;
			SWF.MenuItem.MenuItemCollection menuItemsD = null;
			
			if (type == BasicWidgetType.ContextMenu)
				menuItems = contextMenu.Items;
			else if (type == BasicWidgetType.ContextMenuDeprecated)
				menuItemsD = contextMenuDep.MenuItems;
			else
				menuItems = menuStrip1.Items;

			//cleanup
			//FIXME: use 'menuItems.Clear ()' when BNC#446783 is fixed
			if (menuItems != null) {
				while (menuItems.Count > 0)
					menuItems.Remove (menuItems [0]);

				widget = AddRecursively (menuItems, menu, type);
			} else {
				menuItemsD.Clear ();

				AddRecursively (menuItemsD, menu);

				widget = contextMenuDep;
				contextMenuDep.Show (butWithoutImage, new Point (0, 0));
			}
			
			if (type == BasicWidgetType.MainMenuBar)
				widget = menuStrip1;
			else if (type == BasicWidgetType.ContextMenu) {
				//TODO: use contextMenu.Show (form, 0, 0) as well (when we don't attach it to any control)
				contextMenu.Show (lab1, 0, 0);
				widget = contextMenu;
			}
			GlibSync ();
			return GetAdapterForWidget (widget);
		}

		private void AddRecursively (SWF.MenuItem.MenuItemCollection subcol, List <MenuLayout> menus)
		{
			if (menus.Count <= 0)
				return;
			
			List <SWF.MenuItem> list = new List <SWF.MenuItem> ();
			foreach (MenuLayout menu in menus) {
				SWF.MenuItem mi = new SWF.MenuItem (menu.Label);
				
				AddRecursively (mi.MenuItems, menu.SubMenus);
				
				list.Add (mi);
			}

			subcol.AddRange (list.ToArray ());
		}
		
		private Component AddRecursively (SWF.ToolStripItemCollection subcol, List <MenuLayout> menus, BasicWidgetType type)
		{
			Component ret = null, ret_aux;
			if (menus.Count <= 0)
				return ret;
			
			List <SWF.ToolStripItem> list = new List <SWF.ToolStripItem> ();
			foreach (MenuLayout menu in menus) {
				SWF.ToolStripItem tsi;
				if (menu is MenuSeparator) {
					tsi = new SWF.ToolStripSeparator ();
					ret = tsi;
				} else {
					SWF.ToolStripMenuItem tsmi = new SWF.ToolStripMenuItem ();
					tsi = tsmi;
					tsmi.Text = menu.Label;
					ret_aux = AddRecursively (tsmi.DropDownItems, menu.SubMenus, type);
					if (ret == null) {
						if ((tsmi.DropDownItems.Count > 0) && (type == BasicWidgetType.ParentMenu))
							ret = tsmi;
						else if ((tsmi.DropDownItems.Count == 0) && (type == BasicWidgetType.ChildMenu))
							ret = tsmi;
						else
							ret = ret_aux;
					}
				}
				
				list.Add (tsi);
			}
			subcol.AddRange (list.ToArray ());
			return ret;
		}
		
		private void GetListViewGroup (XmlNode node)
		{
			XmlElement tr = node as XmlElement;
			if (tr == null)
				return;
			SWF.ListViewGroup group = new SWF.ListViewGroup (tr.FirstChild.InnerText);
			lv1.Groups.Add (group);
			for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling) {
				if (child.Name == "tr") {
					SWF.ListViewItem item = GetListViewItem (child);
					lv1.Items.Add (item);
					item.Group = group;
				}
			}
		}

		private SWF.ListViewItem GetListViewItem (XmlNode node)
		{
			SWF.ListViewItem item = null;
			for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling)
				if (child.Name == "td")
					if (item == null)
						item = new SWF.ListViewItem (child.InnerText);
					else
						item.SubItems.Add (child.InnerText);
			return item;
		}

		private SWF.TreeNode GetTreeNode (XmlNode node)
		{
			bool childAdded = false;
			SWF.TreeNode treeNode = new SWF.TreeNode ();
			for (XmlNode child = node.FirstChild; child != null; child = child.NextSibling) {
				if (child.Name == "tr") {
					treeNode.Nodes.Add (GetTreeNode (child));
				} else if (child.Name == "td") {
					if (!childAdded) {
						treeNode.Text = child.InnerText;
						childAdded = true;
					} else {
						Console.WriteLine ("Warning: Couldn't add " + child.InnerText + ": already got td for this row");
					}
				}
			}
			return treeNode;
		}

		private static Atk.Object GetAdapterForProvider (IRawElementProviderSimple provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

#pragma warning disable 618
			object adapter = UiaAtkBridge.AutomationBridge.GetAdapterForProvider (provider);
#pragma warning restore 618
			Assert.IsNotNull (adapter, 
			                  "Object retreived from AutomationBridge.GetAdapterForProvider should not be null");
			Atk.Object atkObj = adapter as Atk.Object;
			Assert.IsNotNull (atkObj, 
			                  "Object retreived from AutomationBridge.GetAdapterForProvider is not Atk.Object");
			return atkObj;
		}

		public void LinkLabelClicked (object source, SWF.LinkLabelLinkClickedEventArgs e)
		{
			lastClickedLink = linklab1.Links.IndexOf (e.Link);
		}

		private SWF.DataGridViewColumn GetDataGridViewColumnFromInnerText (string innerText)
		{
			// We should get the following format:
			// columntype|columnheader|columnvalue0,columnvalue1,..
			SWF.DataGridViewColumn column = null;

			string []tokens = innerText.Split ('|');
			if (tokens [0] == "checkbox")
				column = new SWF.DataGridViewCheckBoxColumn ();
			else if (tokens [0] == "link")
				column = new SWF.DataGridViewLinkColumn ();
			else if (tokens [0] == "image")
				column = new SWF.DataGridViewImageColumn ();
			else if (tokens [0] == "button") {
				SWF.DataGridViewButtonColumn buttoncolumn = new SWF.DataGridViewButtonColumn ();
				buttoncolumn.Text = tokens [2];
				column = buttoncolumn;
			} else if (tokens [0] == "combobox") {
				SWF.DataGridViewComboBoxColumn comboboxColumn = new SWF.DataGridViewComboBoxColumn ();
				string []values = tokens [2].Split (',');
				foreach (string val in values)
					comboboxColumn.Items.Add (val);
				column = comboboxColumn;
			} else // "textbox"
				column = new SWF.DataGridViewTextBoxColumn ();

			column.Width = 50;
			column.HeaderText = tokens [1];

			return column;
		}

		private SWF.DataGridViewCell GetDataGridViewCellFromInnerText (string innerText, int column)
		{
			SWF.DataGridViewCell cell = null;

			if (datagridView.Columns [column].GetType () == typeof (SWF.DataGridViewCheckBoxColumn))
				cell = new SWF.DataGridViewCheckBoxCell ();
			else if (datagridView.Columns [column].GetType () == typeof (SWF.DataGridViewLinkColumn))
				cell = new SWF.DataGridViewLinkCell ();
			else if (datagridView.Columns [column].GetType () == typeof (SWF.DataGridViewImageColumn))
				cell = new SWF.DataGridViewImageCell ();
			else if (datagridView.Columns [column].GetType () == typeof (SWF.DataGridViewComboBoxColumn))
				cell = new SWF.DataGridViewComboBoxCell ();
			else if (datagridView.Columns [column].GetType () == typeof (SWF.DataGridViewButtonColumn))
				cell = new SWF.DataGridViewButtonCell ();
			else // SWF.DataGridViewTextBoxColumn
				cell = new SWF.DataGridViewTextBoxCell ();			

			cell.Value = innerText;

			return cell;
		}

		protected override int ValidNumberOfActionsForAButton { get { return 1; } }
		protected override int ValidNChildrenForAListView { get { return 22; } }
		protected override bool TreeViewHasHeader { get { return false; } }
		protected override int ValidNChildrenForASimpleStatusBar { get { return 0; } }
		protected override int ValidNChildrenForAScrollBar { get { return 0; } }

		[TestFixtureTearDown]
		public void TearDown ()
		{
			form.Close ();
			
			// We shouldn't need to do this, since the disposal of all forms
			// should terminate the bridge, however, be sure to uncomment
			// it if you're getting deadlocks and you don't want to deal
			// with them for now!! (but if you get them, bug us, since
			// it's a bug... probably related to the one fixed in r122955)
			//BridgeTearDown ();

			EventMonitor.Stop ();
		}

		public static void BridgeTearDown ()
		{
			UiaAtkBridge.AutomationBridge.Quit ();
		}
	}
}
