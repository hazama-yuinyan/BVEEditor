// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering
{
	sealed class TextViewCachedElements : IDisposable
	{
		TextFormatter formatter;
		Dictionary<string, TextLine> nonPrintableCharacterTexts;
		
		public TextLine GetTextForNonPrintableCharacter(string text, ITextRunConstructionContext context)
		{
			if(nonPrintableCharacterTexts == null)
				nonPrintableCharacterTexts = new Dictionary<string, TextLine>();
			
            TextLine text_line;
			if(!nonPrintableCharacterTexts.TryGetValue(text, out text_line)){
				var p = new VisualLineElementTextRunProperties(context.GlobalTextRunProperties);
				p.SetForegroundBrush(context.TextView.NonPrintableCharacterBrush);
				if(formatter == null)
					formatter = TextFormatterFactory.Create(context.TextView);
				
                text_line = FormattedTextElement.PrepareText(formatter, text, p);
				nonPrintableCharacterTexts[text] = text_line;
			}
			return text_line;
		}
		
		public void Dispose()
		{
			if(nonPrintableCharacterTexts != null){
				foreach(TextLine line in nonPrintableCharacterTexts.Values)
					line.Dispose();
			}

			if(formatter != null)
				formatter.Dispose();
		}
	}
}
