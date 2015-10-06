/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/19
 * Time: 15:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Media;
using BVEEditor.Editor.CodeCompletion;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// Interface for completion items.
    /// </summary>
	public interface ICompletionItem
	{
        /// <summary>
        /// The content to be inserted.
        /// </summary>
		string Text{get;}

        /// <summary>
        /// The description of this item. It may contain html tags.
        /// </summary>
		string Description{get;}

        /// <summary>
        /// The real description of this item. 
        /// </summary>
        string RealDescription{get;}

        /// <summary>
        /// The icon of this item.
        /// </summary>
		ImageSource Image{get;}

        /// <summary>
        /// The text range to be replaced.
        /// </summary>
        ISegment ReplacementRange{get;}
		
		/// <summary>
		/// Performs code completion for the item.
		/// </summary>
		void Insert(ITextEditor editor);
		
		/// <summary>
		/// Gets a priority value for the completion data item.
		/// When selecting items by their start characters, the item with the highest
		/// priority is selected first.
		/// </summary>
		double Priority{
			get;
		}
	}
	
	/// <summary>
	/// Completion item that supports complex content and description.
	/// </summary>
	public interface IFancyCompletionItem : ICompletionItem
	{
        /// <summary>
        /// The content. It can be various WPF objects.
        /// </summary>
		object Content{get;}

        /// <summary>
        /// The rich description of this item.
        /// </summary>
        object FancyDescription{get;}
	}
	
    /// <summary>
    /// Completion item that represents a code snippet.
    /// </summary>
	public interface ISnippetCompletionItem : ICompletionItem
	{
		string Keyword{get;}
	}
	
	public class DefaultCompletionItem : ICompletionItem
	{
		public string Text{get; private set;}
		public virtual string Description{get; set;}
        public virtual string RealDescription{get; set;}
		public virtual ImageSource Image{get; set;}
        public ISegment ReplacementRange{get; set;}
		
		public virtual double Priority{
            get{return 0;}
        }
		
		public DefaultCompletionItem(string text)
		{
			this.Text = text;
		}
		
		public virtual void Insert(ITextEditor editor)
		{
			editor.Document.Replace(ReplacementRange.Offset, ReplacementRange.Length, Text);
            // Because TextDocument.Replace automatically locate the caret immediately after the replaced text
            // we don't need to locate it manually
		}
	}
}
