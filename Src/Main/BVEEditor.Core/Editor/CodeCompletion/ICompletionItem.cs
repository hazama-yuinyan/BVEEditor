﻿/*
 * Created by SharpDevelop.
 * User: HAZAMA
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
	public interface ICompletionItem
	{
		string Text{get;}
		string Description{get;}
		ImageSource Image{get;}
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
		new object Description{get;}
	}
	
	public interface ISnippetCompletionItem : ICompletionItem
	{
		string Keyword{get;}
	}
	
	public class DefaultCompletionItem : ICompletionItem
	{
		public string Text{get; private set;}
		public virtual string Description{get; set;}
		public virtual ImageSource Image{get; set;}
        public ISegment ReplacementRange{get; set;}
		
		public virtual double Priority{get{return 0;}}
		
		public DefaultCompletionItem(string text)
		{
			this.Text = text;
		}
		
		public virtual void Insert(ITextEditor editor)
		{
			editor.Document.Replace(ReplacementRange.Offset, ReplacementRange.Length, Text);
			editor.Caret.Offset = ReplacementRange.EndOffset;
		}
	}
}
