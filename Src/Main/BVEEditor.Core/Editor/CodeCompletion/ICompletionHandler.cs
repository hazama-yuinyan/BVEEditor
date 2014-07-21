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
        /// <summary>
        /// The text editor.
        /// </summary>
        ITextEditor Editor{get;}

        /// <summary>
        /// The list which includes all completion items.
        /// It is worth noting that it will automatically filter the items according to the characters that are input afterwards.
        /// </summary>
        IList<ICompletionItem> ItemsCache{get; set;}

        /// <summary>
        /// The currently selected item.
        /// </summary>
        ICompletionItem SelectedCompletionItem{get; set;}

        /// <summary>
        /// Reference to the insight window handler.
        /// </summary>
        IInsightWindowHandler InsightWindowHandler{get;}

        /// <summary>
        /// The offset at which the current completion action was started.
        /// </summary>
        int StartOffset{get;}
    }
}
