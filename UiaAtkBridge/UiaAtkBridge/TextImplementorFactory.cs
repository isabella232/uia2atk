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
//      Brad Taylor <brad@getcoded.net>
// 

using System;
using System.Drawing;
using Mono.UIAutomation.Bridge;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using CG = System.Collections.Generic;
using System.Windows.Automation.Provider;

namespace UiaAtkBridge
{
	internal static class TextImplementorFactory
	{
#region Public Methods
		public static ITextImplementor GetImplementor (Adapter adapter,
		                                               IRawElementProviderSimple prov)
		{
			ITextProvider textProvider
				= prov.GetPatternProvider (TextPatternIdentifiers.Pattern.Id)
					as ITextProvider;
			if (textProvider != null)
				return new TextProviderTextImplementor (adapter, textProvider);

			IValueProvider valueProvider
				= prov.GetPatternProvider (ValuePatternIdentifiers.Pattern.Id)
					as IValueProvider;
			if (valueProvider != null)
				return new ValueProviderTextImplementor (adapter, valueProvider);
			
			return new NamePropertyTextImplementor (adapter, prov);
		}
#endregion
	}
}
