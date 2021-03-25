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
//      Andrés G. Aragoneses <aaragoneses@novell.com>
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Drawing;
using Mono.UIAutomation.Bridge;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using CG = System.Collections.Generic;
using System.Windows.Automation.Provider;
using Mono.UIAutomation.Services;

namespace UiaAtkBridge
{
	internal abstract class BaseTextImplementor : ITextImplementor
	{
#region Public Properties
		public abstract string Text {
			get;
		}

		public int Length {
			get { return Text.Length; }
		}

		public virtual Atk.Attribute [] DefaultAttributes {
			get { return new Atk.Attribute [0]; }
		}
#endregion

#region Public Methods
		public BaseTextImplementor (Adapter resource)
		{
			this.resource = resource;

			caretProvider
				= resource.Provider.GetPatternProvider (CaretPatternIdentifiers.Pattern.Id)
					as ICaretProvider;
			caretOffset = -1;
		}
		
		public virtual int NSelections { get { return 0; } }

		public virtual string GetSelection (int selectionNum, out int startOffset, out int endOffset)
		{
			startOffset = selectionStartOffset;
			endOffset = selectionEndOffset;
			return null;
		}
		
		public virtual bool AddSelection (int startOffset,  int endOffset)
		{
			selectionStartOffset = 0;
			selectionEndOffset = 0;
			return false;
		}

		public virtual bool RemoveSelection (int selectionNum)
		{
			return false;
		}

		public virtual bool SetSelection (int selectionNum, int startOffset,  int endOffset)
		{
			return false;
		}

		public int CaretOffset { 
			get {
				if (caretProvider == null)
					return caretOffset;

				return caretProvider.CaretOffset;
			}
		}

		public bool SetCaretOffSet (int offset)
		{
			if (caretProvider != null) {
				if (caretProvider.SetCaretOffset (offset))
					caretOffset = offset;
				// gail always returns true; tracking it
				// because of tests
			} else
				caretOffset = offset;

			return true;
		}

		public string GetTextAfterOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string text = Text;

			startOffset = 0;
			endOffset = 0;
			try {
				switch (boundaryType){
				case Atk.TextBoundary.Char:
					
					endOffset = startOffset = offset + 1;
					if (startOffset > text.Length)
						endOffset = startOffset = text.Length;
					else if (endOffset + 1 <= text.Length)
						endOffset++;
	
					return ReturnTextWrtOffset (startOffset);
					
				case Atk.TextBoundary.LineEnd:
					ForwardToNextSeparator (newLineSeparators, offset, out startOffset, out endOffset);
					endOffset = ForwardToNextSeparator (newLineSeparators, endOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.LineStart:
					startOffset = ForwardToNextSeparator (newLineSeparators, offset, false);
					endOffset = ForwardToNextSeparator (newLineSeparators, startOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.WordEnd:
					ForwardToNextSeparator (wordSeparators, offset, out startOffset, out endOffset);
					endOffset = ForwardToNextSeparator (wordSeparators, endOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.WordStart:
					startOffset = ForwardToNextSeparator (wordSeparators, offset, false);
					endOffset = ForwardToNextSeparator (wordSeparators, startOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.SentenceEnd:
					ForwardToNextSeparator (sentenceSeparators, offset, out startOffset, out endOffset, true);
					endOffset = ForwardToNextSeparator (sentenceSeparators, endOffset, true);
					int testStartOffset, nextStartOffset, testEndOffset, nextEndOffset;
					ForwardToNextSeparator(softSentenceSeparators, startOffset, out testStartOffset, out nextStartOffset);
					if (testStartOffset == startOffset)
						startOffset = nextStartOffset;
					ForwardToNextSeparator(softSentenceSeparators, startOffset, out testEndOffset, out nextEndOffset);
					if (testEndOffset == endOffset)
						endOffset = nextEndOffset;
					
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.SentenceStart:
					startOffset = ForwardToNextSeparator (sentenceSeparators, offset, false);
					endOffset = ForwardToNextSeparator (sentenceSeparators, startOffset, false);
					
					endOffset = ForwardToNextSeparator (wordSeparators, endOffset, false);
					
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				default:
					throw GetNotSupportedBoundary (boundaryType);
				}
			} catch (System.IndexOutOfRangeException) {
				return String.Empty;
			} finally {
				selectionStartOffset = startOffset;
				selectionEndOffset = endOffset;
			}
		}
		
		public string GetTextAtOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			string text = Text;

			startOffset = 0;
			endOffset = 0;
			try {
				switch (boundaryType){
	
				case Atk.TextBoundary.WordEnd:
					startOffset = BackwardToNextSeparator (wordSeparators, offset, false);
					endOffset = ForwardToNextSeparator (wordSeparators, offset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.WordStart:
					startOffset = BackwardToNextSeparator (wordSeparators, offset, true);
					endOffset = ForwardToNextSeparator (wordSeparators, offset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.LineEnd:
					startOffset = BackwardToNextSeparator (newLineSeparators, offset, false);
					endOffset = ForwardToNextSeparator (newLineSeparators, offset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.LineStart:
					startOffset = BackwardToNextSeparator (newLineSeparators, offset, true);
					endOffset = ForwardToNextSeparator (newLineSeparators, offset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.SentenceEnd:
					startOffset = BackwardToNextSeparator (sentenceSeparators, offset, false);
					endOffset = ForwardToNextSeparator (sentenceSeparators, offset, true);
	
					int testEndOffset, nextEndOffset;
					ForwardToNextSeparator(softSentenceSeparators, startOffset, out testEndOffset, out nextEndOffset);
					if (testEndOffset == endOffset)
						endOffset = nextEndOffset;
					
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.SentenceStart:
					startOffset = BackwardToNextSeparator (sentenceSeparators, offset, true);
					endOffset = ForwardToNextSeparator (sentenceSeparators, offset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.Char:
					startOffset = offset;
					if (startOffset < 0) {
						startOffset = text.Length;
						endOffset = text.Length;
					}
					else if (offset >= text.Length)
						endOffset = offset;
					else
						endOffset = offset + 1;
					return ReturnTextWrtOffset (offset);
					
				default:
					throw GetNotSupportedBoundary (boundaryType);
				}
			} catch (System.IndexOutOfRangeException) {
				return String.Empty;
			} finally {
				selectionStartOffset = startOffset;
				selectionEndOffset = endOffset;
			}
		}

		public string GetTextBeforeOffset (int offset, Atk.TextBoundary boundaryType, out int startOffset, out int endOffset)
		{
			startOffset = 0;
			endOffset = 0;
			try {
				switch (boundaryType){
	
				case Atk.TextBoundary.WordEnd:
					endOffset = BackwardToNextSeparator (wordSeparators, offset, false);
					startOffset = BackwardToNextSeparator (wordSeparators, endOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.WordStart:
					BackwardToNextSeparator (wordSeparators, offset, out endOffset, out startOffset);
					startOffset = BackwardToNextSeparator (wordSeparators, startOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.LineEnd:
					endOffset = BackwardToNextSeparator (newLineSeparators, offset, false);
					startOffset = BackwardToNextSeparator (newLineSeparators, endOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.LineStart:
					BackwardToNextSeparator (newLineSeparators, offset, out endOffset, out startOffset);
					startOffset = BackwardToNextSeparator (newLineSeparators, startOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.Char:
					startOffset = offset - 1;
					endOffset = offset;
					return ReturnTextWrtOffset (startOffset);
	
				case Atk.TextBoundary.SentenceEnd:
					endOffset = BackwardToNextSeparator (sentenceSeparators, offset, false);
					startOffset = BackwardToNextSeparator (sentenceSeparators, endOffset, false);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				case Atk.TextBoundary.SentenceStart:
					BackwardToNextSeparator (sentenceSeparators, offset, out endOffset, out startOffset);
					startOffset = BackwardToNextSeparator (sentenceSeparators, startOffset, true);
					return ReturnTextWrtOffset (startOffset, endOffset);
					
				default:
					throw GetNotSupportedBoundary (boundaryType);
				}
			} catch (System.IndexOutOfRangeException) {
				return String.Empty;
			} finally {
				selectionStartOffset = startOffset;
				selectionEndOffset = endOffset;
			}
		}

		public string GetStringAtOffset (int offset, Atk.TextGranularity granularity, out int startOffset, out int endOffset)
		{
			Atk.TextBoundary boundaryType;

			switch (granularity) {
			case Atk.TextGranularity.Char:
				boundaryType = Atk.TextBoundary.Char;
				break;
			case Atk.TextGranularity.Word:
				boundaryType = Atk.TextBoundary.WordStart;
				break;
			case Atk.TextGranularity.Sentence:
				boundaryType = Atk.TextBoundary.SentenceStart;
				break;
			case Atk.TextGranularity.Line:
			case Atk.TextGranularity.Paragraph:
				boundaryType = Atk.TextBoundary.LineStart;
				break;
			default:
				throw new NotSupportedException ("Granularity not supported: " + granularity.ToString ());
			}

			return GetTextAtOffset (offset, boundaryType, out startOffset, out endOffset);
		}

		public string GetText (int startOffset, int endOffset)
		{
			if (cachedText != null && cachedTextStart == startOffset && cachedTextEnd == endOffset)
				return cachedText;
			string text = Text;
			if ((endOffset == -1) || (endOffset > text.Length))
				endOffset = text.Length;
			if (endOffset < startOffset)
				return String.Empty;
			return text.Substring (startOffset, endOffset - startOffset);
		}

		public char GetCharacterAtOffset (int offset)
		{
			string text = Text;
			if ((offset >= text.Length) || (offset < 0) || (String.IsNullOrEmpty (text)))
				return '\0';
			return text [offset];
		}

		public void GetCharacterExtents (int offset, out int x, out int y, out int width, out int height, Atk.CoordType coords)
		{
			Atk.TextRectangle rect;
			GetRangeExtents (offset, offset, coords, out rect);
			x = rect.X;
			y = rect.Y;
			width = rect.Width;
			height = rect.Height;
		}

		public void GetRangeExtents (int startOffset, int endOffset, Atk.CoordType coordType, out Atk.TextRectangle rect)
		{
			int length = Length;
			System.Windows.Rect bounds = resource.BoundingRectangle;
			rect.X = (int)(bounds.X + (length > 0 ? (bounds.Width * startOffset) / length : 0));
			rect.Y = (int)bounds.Y;
			rect.Height = (int)bounds.Height;
			rect.Width = (int)(length > 0 ? bounds.Width * (endOffset - startOffset) / length : 0);
			if (coordType == Atk.CoordType.Window)
				resource.ConvertCoords (ref rect.X, ref rect.Y, false);
		}

		public Atk.TextRange GetBoundedRanges (Atk.TextRectangle rect, Atk.CoordType coordType, Atk.TextClipType xClipType, Atk.TextClipType yClipType)
		{
			//TODO
			Log.Warn ("GetBoundedRanges not implemented");
			return new Atk.TextRange ();
		}

		public int GetOffsetAtPoint (int x, int y, Atk.CoordType coords)
		{
			//TODO
			Log.Warn ("GetOffsetAtPoint not implemented");
			return -1;
		}

		public virtual Atk.Attribute [] GetRunAttributes (int offset, out int startOffset, out int endOffset)
		{
			startOffset = endOffset = -1;
			return new Atk.Attribute [0];
		}

		public bool HandleSimpleChange (ref string oldText, ref int caretOffset)
		{
			return HandleSimpleChange (ref oldText, ref caretOffset, true);
		}

		public bool HandleSimpleChange (ref string oldText, ref int caretOffset, bool updateCaret)
		{
			string text = Text;

			if (text == oldText)
				return true;
			int oldLength = oldText.Length;
			int newLength = Length;
			int offset;
			if (newLength > oldLength) {
				if (IsAddition (text, oldText, out offset)) {
					EmitTextChanged (Atk.TextChangedDetail.Insert, offset, newLength - oldLength);
					if (updateCaret) {
						caretOffset = offset + (newLength - oldLength);
						resource.EmitSignal ("text_caret_moved", caretOffset);
					}
					oldText = text;
					return true;
				}
			}
			else if (oldLength > newLength) {
				if (IsAddition (oldText, text, out offset)) {
					// Atk-bridge expects the text not to
					// have been deleted yet
					EmitTextChanged (Atk.TextChangedDetail.Delete, offset, oldLength - newLength, oldText);
					if (updateCaret) {
						caretOffset = offset;
						resource.EmitSignal ("text_caret_moved", caretOffset);
					}
					oldText = text;
					return true;
				}
			}
			return false;
		}

		public bool RaiseAutomationPropertyChangedEvent (AutomationPropertyChangedEventArgs e)
		{
			UpdateCaretToValidValue ();
			
			if (e.Property.Id == ValuePatternIdentifiers.ValueProperty.Id) {
				// Don't fire spurious events if the text hasn't changed
				string oldText = (string) e.OldValue;
				if (HandleSimpleChange (ref oldText,
				                        ref caretOffset, 
				                        caretProvider == null))
					return false;

				if (caretProvider == null)
					caretOffset = Length;

				return false;
			}
			
			return false;
		}

		public bool RaiseAutomationEvent (AutomationEvent eventId, AutomationEventArgs e)
		{
			UpdateCaretToValidValue ();

			if (eventId == TextPatternIdentifiers.CaretMovedEvent) {
				int newCaretOffset = caretProvider.CaretOffset;
				if (newCaretOffset != caretOffset) {
					caretOffset = newCaretOffset;
					resource.EmitSignal ("text_caret_moved", caretOffset);
				}
				return true;
			}
				
			return false;
		}

		public void EmitTextChanged (Atk.TextChangedDetail detail, int position, int length)
		{
			EmitTextChanged (detail, position, length, null);
		}

		public void EmitTextChanged (Atk.TextChangedDetail detail, int position, int length, string curText)
		{
			if (curText == null)
				curText = Text;
			GLib.Timeout.Add (0, new GLib.TimeoutHandler (delegate {
				cachedText = curText.Substring (position, length);
				cachedTextStart = position;
				cachedTextEnd = position + length;
				GLib.Signal.Emit (resource, "text_changed::" + detail.ToString ().ToLower (),
					position, length);
				cachedText = null;
				return false;
			}));
		}
#endregion

#region Protected Fields

		protected ICaretProvider CaretProvider {
			get { return caretProvider; }
		}
		
		protected Adapter resource = null;
		protected int selectionStartOffset = 0;
		protected int selectionEndOffset = 0;
#endregion

#region Private Methods
		private string ReturnTextWrtOffset (int startOffset, int endOffset)
		{
			//TODO: optimize?
			return Text.Substring (startOffset, endOffset - startOffset);
		}
		
		// endOffset == startOffset + 1
		private string ReturnTextWrtOffset (int startOffset)
		{
			//TODO: optimize?
			if ((startOffset < 0) || (startOffset > Length))
				return String.Empty;
			return new String (new char[] { GetCharacterAtOffset (startOffset) });
		}
		
		private void ForwardToNextSeparator (char[] seps, int startOffset, out int stopEarlyOffset, out int stopLateOffset)
		{
			ForwardToNextSeparator (seps, startOffset, out stopEarlyOffset, out stopLateOffset, false);
		}
		
		private void ForwardToNextSeparator (char[] seps, int startOffset, 
		                                     out int stopEarlyOffset, out int stopLateOffset, 
		                                     bool findNonSeparators)
		{
			string text = Text;
			int retOffset = startOffset;
			bool anyNonSeparator = false;
			while (true) {
				bool isSep = CharEqualsAny (text [retOffset], seps);
				if (!isSep)
					anyNonSeparator = true;
				
				if (!isSep || (findNonSeparators && !anyNonSeparator))
					retOffset++;
				else
					break;
			}

			stopEarlyOffset = retOffset;
			while (retOffset < text.Length && CharEqualsAny (text [retOffset], seps))
				retOffset++;
			stopLateOffset = retOffset;
		}
		
		private int ForwardToNextSeparator (char[] seps, int startOffset, bool stopEarly)
		{
			string text = Text;
			int retOffset = startOffset;
			if (retOffset >= text.Length)
				return text.Length;
			
			while (!CharEqualsAny (text [retOffset], seps))
			{
				if (retOffset == text.Length - 1)
					return text.Length;
				retOffset++;
			}
			if (stopEarly)
				return retOffset;
			
			while ((retOffset < text.Length) && CharEqualsAny (text [retOffset], seps))
				retOffset++;
			return retOffset;
		}

		private void BackwardToNextSeparator (char[] seps, int startOffset, 
		                                      out int stopEarlyOffset, out int stopLateOffset)
		{
			if (startOffset <= 1){
				stopEarlyOffset = 0;
				stopLateOffset = 0;
				return;
			}
			
			int retOffset = startOffset - 1;
			string text = Text;
			
			while (!CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			stopEarlyOffset = retOffset + 1;
			if (retOffset < 0) {
				stopLateOffset = 0;
				return;
			}
			
			while (CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}
			
			stopLateOffset = retOffset + 1;
			return;
		}
		
		private int BackwardToNextSeparator (char[] seps, int startOffset, bool stopEarly)
		{
			if (startOffset <= 1)
				return 0;
			
			int retOffset = startOffset - 1;
			string text = Text;
			
			while (!CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}

			if (stopEarly)
				return retOffset + 1;
			else if (retOffset < 0)
				return 0;
			
			while (CharEqualsAny (text [retOffset], seps)) {
				retOffset--;
				if (retOffset < 0)
					break;
			}
			
			return retOffset + 1;
		}
		
		private bool CharEqualsAny (char boilerPlate, char [] candidates)
		{
			foreach (char candidate in candidates)
				if (boilerPlate == candidate)
					return true;
			return false;
		}
		
		private NotSupportedException GetNotSupportedBoundary (Atk.TextBoundary bType)
		{
			return new NotSupportedException (
				String.Format ("The value {0} is not supported as a Atk.TextBoundary type.",
					bType));
		}

		private bool IsAddition (string super, string sub, out int offset)
		{
			int startOff = 0;
			int subLength = sub.Length;
			while (startOff < subLength && super [startOff] == sub [startOff])
				startOff++;
			if (startOff == subLength) {
				offset = startOff;
				return true;
			}
			int endOff = subLength - 1;
			int diff = super.Length - subLength;
			while (endOff >= startOff && sub [endOff] == super [endOff + diff])
				endOff--;
			offset = startOff;
			return (startOff > endOff);
		}

		private void UpdateCaretToValidValue ()
		{
			if (caretOffset == -1)
				caretOffset = (caretProvider != null ? caretProvider.CaretOffset : Length);
		}
#endregion

#region Private Fields
		
		//TODO: use regexp?
		private static char [] wordSeparators = new char [] { ' ', '\n', '\r', '.', '\t' };
		private static char [] newLineSeparators = new char [] { '\n', '\r' };
		private static char [] sentenceSeparators = new char [] { '\n', '\r', '.' };
		private static char [] softSentenceSeparators = new char [] { '.', ':'};

		private ICaretProvider caretProvider;
		private int caretOffset;
		private string cachedText = null;
		private int cachedTextStart = 0;
		private int cachedTextEnd = 0;
#endregion
	}
}
