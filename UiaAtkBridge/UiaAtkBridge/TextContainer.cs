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
//      Mike Gorse <mgorse@novell.com>
// 

using System;
using System.Windows.Automation;
using Mono.UIAutomation.Services;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{

	public class TextContainer : ComponentParentAdapter , Atk.ITextImplementor
	{
		private ITextImplementor textExpert = null;
		
		public TextContainer (IRawElementProviderSimple provider): base (provider)
		{
			Role = Atk.Role.Statusbar;

			textExpert = TextImplementorFactory.GetImplementor (this, provider);
		}
		
		public int CaretOffset {
			get {
				return 0;
			}
		}

		public Atk.Attribute [] DefaultAttributes {
			get {
				return textExpert.DefaultAttributes;
			}
		}

		public int CharacterCount {
			get {
				return textExpert.Length;
			}
		}

		public int NSelections {
			get {
				return -1;
			}
		}
		
		public override void RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			if (eventId == SelectionItemPatternIdentifiers.ElementSelectedEvent)
				NotifyStateChange (Atk.StateType.Selected, true);
			else if (eventId == SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent)
				NotifyStateChange (Atk.StateType.Selected, false);
			else
				base.RaiseAutomationEvent (eventId, e);
		}

		public string GetText (int startOffset, int endOffset)
		{
			return textExpert.GetText (startOffset, endOffset);
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAfterOffset (offset, boundaryType, out startOffset, out endOffset);
			return ret;
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
			return ret;
		}
		
		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string ret = 
				textExpert.GetTextBeforeOffset (offset, boundaryType, out startOffset, out endOffset);
			return ret;
		}
		
		public string GetStringAtOffset (int offset, Atk.TextGranularity granularity, out int startOffset, out int endOffset)
		{
			return textExpert.GetStringAtOffset (offset, granularity, out startOffset, out endOffset);
		}

		public Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			return textExpert.GetRunAttributes (offset, out startOffset, out endOffset);
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			textExpert.GetCharacterExtents (offset, out x, out y, out width, out height, coords);
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			return textExpert.GetOffsetAtPoint (x, y, coords);
		}

		public string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			return textExpert.GetSelection (selectionNum, out startOffset, out endOffset);
		}

		public bool AddSelection (int startOffset, int endOffset)
		{
			return textExpert.AddSelection (startOffset, endOffset);
		}

		public bool RemoveSelection (int selectionNum)
		{
			return textExpert.RemoveSelection (selectionNum);
		}

		public bool SetSelection (int selectionNum, int startOffset, int endOffset)
		{
			return textExpert.SetSelection (selectionNum, startOffset, endOffset);
		}
		
		public char GetCharacterAtOffset (int offset)
		{
			return textExpert.GetCharacterAtOffset (offset);
		}

		public bool SetCaretOffset (int offset)
		{
			return false;
		}

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			textExpert.GetRangeExtents (startOffset, endOffset, coordType, out rect);
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			return textExpert.GetBoundedRanges (rect, coordType, xClipType, yClipType);
		}
		
		public override void RaiseStructureChangedEvent (object childProvider, StructureChangedEventArgs e)
		{
			// TODO
			Log.Warn ("TextContainer: RaiseStructureChangedEvent not implemented.");
		}

		protected override void UpdateNameProperty (string newName, bool fromCtor)
		{
			base.UpdateNameProperty (newName, fromCtor);

			// If we're being called from the ctor, textExpert
			// won't be initialized yet so bail
			if (fromCtor)
				return;

			// First delete all text, then insert the new text
			textExpert.EmitTextChanged (Atk.TextChangedDetail.Delete, 0, textExpert.Length);

			textExpert.EmitTextChanged (Atk.TextChangedDetail.Insert, 0,
						 newName == null ? 0 : newName.Length);

			EmitVisibleDataChanged ();
		}
	}

	public class TextContainerWithGrid : TextContainer, Atk.ITableImplementor
	{
		private TableImplementorHelper tableExpert = null;
		
		public TextContainerWithGrid (IRawElementProviderSimple provider) : base (provider)
		{
			tableExpert = new TableImplementorHelper (this);
		}
		
		public Atk.Object RefAt (int row, int column)
		{
			return tableExpert.RefAt (row, column);
		}

		public int GetIndexAt (int row, int column)
		{
			return tableExpert.GetIndexAt (row, column);
		}

		public int GetColumnAtIndex (int index)
		{
			return tableExpert.GetColumnAtIndex (index);
		}

		public int GetRowAtIndex (int index)
		{
			return tableExpert.GetRowAtIndex (index);
		}

		public int NColumns { get { return tableExpert.NColumns; } }
		public int NRows { get { return tableExpert.NRows; } }
			
		public int GetColumnExtentAt (int row, int column)
		{
			return tableExpert.GetColumnExtentAt (row, column);
		}

		public int GetRowExtentAt (int row, int column)
		{
			return tableExpert.GetRowExtentAt (row, column);
		}

		public Atk.Object Caption
		{
			get { return tableExpert.Caption; } set { tableExpert.Caption = value; }
		}

		public string GetColumnDescription (int column)
		{
			return string.Empty;
		}

		public Atk.Object GetColumnHeader (int column)
		{
			return null;
		}

		public string GetRowDescription (int row)
		{
			return String.Empty;
		}

		public Atk.Object GetRowHeader (int row)
		{
			return null;
		}

		public Atk.Object Summary
		{
			get { return tableExpert.Summary; } set { tableExpert.Summary = value; }
		}


		public void SetColumnDescription (int column, string description)
		{
			tableExpert.SetColumnDescription (column, description);
		}

		public void SetColumnHeader (int column, Atk.Object header)
		{
			tableExpert.SetColumnHeader (column, header);
		}

		public void SetRowDescription (int row, string description)
		{
			tableExpert.SetRowDescription (row, description);
		}

		public void SetRowHeader (int row, Atk.Object header)
		{
			tableExpert.SetRowHeader (row, header);
		}

		public int [] SelectedColumns {
			get { return tableExpert.SelectedColumns; }
		}

		public int [] SelectedRows {
			get { return tableExpert.SelectedRows; }
		}

		// TODO: Remove next methods when atk-sharp is fixed (BNC#512477)
		public int GetSelectedColumns (out int selected)
		{
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedColumns (out int [] selected)
		{
			return tableExpert.GetSelectedColumns (out selected);
		}

		public int GetSelectedRows (out int selected)
		{
			return tableExpert.GetSelectedRows (out selected);
		}

		public int GetSelectedRows (out int [] selected)
		{
			return tableExpert.GetSelectedRows (out selected);
		}

		public bool IsColumnSelected (int column)
		{
			return tableExpert.IsColumnSelected (column);
		}

		public bool IsRowSelected (int row)
		{
			return tableExpert.IsRowSelected (row);
		}
		public bool IsSelected (int row, int column)
		{
			return tableExpert.IsSelected (row, column);
		}

		public bool AddRowSelection (int row)
		{
			return tableExpert.AddRowSelection (row);
		}

		public bool RemoveRowSelection (int row)
		{
			return tableExpert.RemoveRowSelection (row);
		}

		public bool AddColumnSelection (int column)
		{
			return tableExpert.AddColumnSelection (column);
		}

		public bool RemoveColumnSelection (int column)
		{
			return tableExpert.RemoveColumnSelection (column);
		}
	}
}
