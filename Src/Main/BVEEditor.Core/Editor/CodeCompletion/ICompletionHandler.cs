using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// Interface for code completion handlers.
    /// </summary>
    public interface ICompletionHandler
    {
        ITextEditor Editor{get;}
        IList<ICompletionItem> ItemsCache{get; set;}
        ICompletionItem SelectedCompletionItem{get; set;}
        IInsightWindowHandler InsightWindowHandler{get;}
        int StartOffset{get;}
    }
}
