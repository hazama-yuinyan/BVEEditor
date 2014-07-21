// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.AvalonEdit.Search
{
    /// <summary>
    /// An <see cref="ISearchStrategy"/> implementation that finds strings with regular expressions.
    /// </summary>
	class RegexSearchStrategy : ISearchStrategy
	{
		readonly Regex SearchPattern;
		readonly bool MatchWholeWords;
		
		public RegexSearchStrategy(Regex searchPattern, bool matchWholeWords)
		{
			if(searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			
            SearchPattern = searchPattern;
			MatchWholeWords = matchWholeWords;
		}
		
		public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
		{
			int end_offset = offset + length;
			foreach(Match result in SearchPattern.Matches(document.Text)){
				int result_end_offset = result.Length + result.Index;
				if(offset > result.Index || end_offset < result_end_offset)
					continue;
				
                if(MatchWholeWords && (!IsWordBorder(document, result.Index) || !IsWordBorder(document, result_end_offset)))
					continue;
				
                yield return new SearchResult{StartOffset = result.Index, Length = result.Length, Data = result};
			}
		}
		
		static bool IsWordBorder(ITextSource document, int offset)
		{
			return TextUtilities.GetNextCaretPosition(document, offset - 1, LogicalDirection.Forward, CaretPositioningMode.WordBorder) == offset;
		}
		
		public ISearchResult FindNext(ITextSource document, int offset, int length)
		{
			return FindAll(document, offset, length).FirstOrDefault();
		}
		
		public bool Equals(ISearchStrategy other)
		{
			var strategy = other as RegexSearchStrategy;
			return strategy != null &&
				strategy.SearchPattern.ToString() == SearchPattern.ToString() &&
				strategy.SearchPattern.Options == SearchPattern.Options &&
				strategy.SearchPattern.RightToLeft == SearchPattern.RightToLeft;
		}
	}
	
	public class SearchResult : TextSegment, ISearchResult
	{
		public Match Data{get; set;}
		
		public string ReplaceWith(string replacement)
		{
			return Data.Result(replacement);
		}
	}
}
