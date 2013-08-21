﻿/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 06/30/2013
 * Time: 23:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;

namespace BVEEditor.Editor.AvalonEdit
{
	/// <summary>
	/// Implements AvalonEdit's <see cref="IIndentationStrategy"/> by forwarding calls
	/// to a <see cref="IFormattingStrategy"/>.
	/// </summary>
	public class IndentationStrategyAdapter : IIndentationStrategy
	{
		readonly ITextEditor editor;
		readonly IFormattingStrategy formattingStrategy;
		
		public IndentationStrategyAdapter(ITextEditor editor, IFormattingStrategy formattingStrategy)
		{
			if(editor == null)
				throw new ArgumentNullException("editor");
			
			if(formattingStrategy == null)
				throw new ArgumentNullException("formattingStrategy");
			
			this.editor = editor;
			this.formattingStrategy = formattingStrategy;
		}
		
		public virtual void IndentLine(TextDocument document, DocumentLine line)
		{
			if(line == null)
				throw new ArgumentNullException("line");
			
			formattingStrategy.IndentLine(editor, editor.Document.GetLineByNumber(line.LineNumber));
		}
		
		public virtual void IndentLines(TextDocument document, int beginLine, int endLine)
		{
			formattingStrategy.IndentLines(editor, beginLine, endLine);
		}
	}
}