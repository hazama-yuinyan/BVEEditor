/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/20
 * Time: 13:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Editor
{
	/// <summary>
	/// Indentation and formatting strategy.
	/// </summary>
	public interface IFormattingStrategy
	{
		/// <summary>
		/// This method formats a specific line after <code>charTyped</code> is pressed.
		/// </summary>
        /// <remarks>
        /// This method is supposed to be called whenever a new character is typed in.
        /// </remarks>
		void FormatLine(ITextEditor editor, char charTyped);
		
		/// <summary>
		/// This method sets the indentation level in a specific line
		/// </summary>
		void IndentLine(ITextEditor editor, IDocumentLine line);
		
		/// <summary>
		/// This method sets the indentation in a range of lines.
		/// </summary>
		void IndentLines(ITextEditor editor, int beginLine, int endLine);
		
		/// <summary>
		/// This method surrounds the selected text with a comment.
		/// </summary>
		void SurroundSelectionWithComment(ITextEditor editor);
	}
	
	public class DefaultFormattingStrategy : IFormattingStrategy
	{
		internal static readonly DefaultFormattingStrategy DefaultInstance = new DefaultFormattingStrategy();
		
		public virtual void FormatLine(ITextEditor editor, char charTyped)
		{
		}
		
		public virtual void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			IDocument document = editor.Document;
			int line_num = line.LineNumber;
			if(line_num > 1){
				IDocumentLine previous_line = document.GetLineByNumber(line_num - 1);
				string indentation = DocumentUtilities.GetWhitespaceAfter(document, previous_line.Offset);
				// copy indentation to line
				string new_indentation = DocumentUtilities.GetWhitespaceAfter(document, line.Offset);
				document.Replace(line.Offset, new_indentation.Length, indentation);
			}
		}
		
		public virtual void IndentLines(ITextEditor editor, int begin, int end)
		{
			using(editor.Document.OpenUndoGroup()){
				for(int i = begin; i <= end; i++)
					IndentLine(editor, editor.Document.GetLineByNumber(i));
			}
		}
		
		public virtual void SurroundSelectionWithComment(ITextEditor editor)
		{
		}
		
		/// <summary>
		/// Default implementation for single line comments.
		/// </summary>
		protected void SurroundSelectionWithSingleLineComment(ITextEditor editor, string comment)
		{
			IDocument document = editor.Document;
			using(document.OpenUndoGroup()){
				TextLocation start_pos = document.GetLocation(editor.SelectionStart);
				TextLocation end_pos = document.GetLocation(editor.SelectionStart + editor.SelectionLength);
				
				// endLine is one above endPosition if no characters are selected on the last line (e.g. line selection from the margin)
				int end_line = (end_pos.Column == 1 && end_pos.Line > start_pos.Line) ? end_pos.Line - 1 : end_pos.Line;
				
				List<IDocumentLine> lines = new List<IDocumentLine>();
				bool remove_comment = true;
				
				for(int i = start_pos.Line; i <= end_line; i++){
					lines.Add(editor.Document.GetLineByNumber(i));
					if(!document.GetText(lines[i - start_pos.Line]).Trim().StartsWith(comment, StringComparison.Ordinal))
						remove_comment = false;
				}
				
				foreach(IDocumentLine line in lines){
					if(remove_comment)
						document.Remove(line.Offset + document.GetText(line).IndexOf(comment, StringComparison.Ordinal), comment.Length);
					else
						document.Insert(line.Offset, comment, AnchorMovementType.BeforeInsertion);
				}
			}
		}
		
		/// <summary>
		/// Default implementation for multiline comments.
		/// </summary>
		protected void SurroundSelectionWithBlockComment(ITextEditor editor, string blockStart, string blockEnd)
		{
			using(editor.Document.OpenUndoGroup()){
				int start_offset = editor.SelectionStart;
				int end_offset = editor.SelectionStart + editor.SelectionLength;
				
				if(editor.SelectionLength == 0){
					IDocumentLine line = editor.Document.GetLineByOffset(editor.SelectionStart);
					start_offset = line.Offset;
					end_offset = line.Offset + line.Length;
				}
				
				BlockCommentRegion region = FindSelectedCommentRegion(editor, blockStart, blockEnd);
				
				if(region != null){
					editor.Document.Remove(region.EndOffset, region.CommentEnd.Length);
					editor.Document.Remove(region.StartOffset, region.CommentStart.Length);
				}else{
					editor.Document.Insert(end_offset, blockEnd);
					editor.Document.Insert(start_offset, blockStart);
				}
			}
		}
		
		public static BlockCommentRegion FindSelectedCommentRegion(ITextEditor editor, string commentStart, string commentEnd)
		{
			IDocument document = editor.Document;
			
			if(document.TextLength == 0)
				return null;
			
			// Find start of comment in selected text.
			
			int comment_end_offset = -1;
			string selected_text = editor.SelectedText;
			
			int comment_start_offset = selected_text.IndexOf(commentStart);
			if(comment_start_offset >= 0)
				comment_start_offset += editor.SelectionStart;

			// Find end of comment in selected text.
			
			if(comment_start_offset >= 0)
				comment_end_offset = selected_text.IndexOf(commentEnd, comment_start_offset + commentStart.Length - editor.SelectionStart);
			else
				comment_end_offset = selected_text.IndexOf(commentEnd);
			
			if(comment_end_offset >= 0)
				comment_end_offset += editor.SelectionStart;
			
			// Find start of comment before or partially inside the
			// selected text.
			
			int comment_end_before_start_offset = -1;
			if(comment_start_offset == -1){
				int offset = editor.SelectionStart + editor.SelectionLength + commentStart.Length - 1;
				if(offset > document.TextLength)
					offset = document.TextLength;
				
				string text = document.GetText(0, offset);
				comment_start_offset = text.LastIndexOf(commentStart);
				if(comment_start_offset >= 0){
					// Find end of comment before comment start.
					comment_end_before_start_offset = text.IndexOf(commentEnd, comment_start_offset, editor.SelectionStart - comment_start_offset);
					if(comment_end_before_start_offset > comment_start_offset)
						comment_start_offset = -1;
				}
			}
			
			// Find end of comment after or partially after the
			// selected text.
			
			if(comment_end_offset == -1){
				int offset = editor.SelectionStart + 1 - commentEnd.Length;
				if(offset < 0)
					offset = editor.SelectionStart;
				
				string text = document.GetText(offset, document.TextLength - offset);
				comment_end_offset = text.IndexOf(commentEnd);
				if(comment_end_offset >= 0)
					comment_end_offset += offset;
			}
			
			if(comment_start_offset != -1 && comment_end_offset != -1)
				return new BlockCommentRegion(commentStart, commentEnd, comment_start_offset, comment_end_offset);
			
			return null;
		}
	}
	
	public class BlockCommentRegion
	{
		public string CommentStart{get; private set;}
		public string CommentEnd{get; private set;}
		public int StartOffset{get; private set;}
		public int EndOffset{get; private set;}
		
		/// <summary>
		/// The end offset is the offset where the comment end string starts from.
		/// </summary>
		public BlockCommentRegion(string commentStart, string commentEnd, int startOffset, int endOffset)
		{
			this.CommentStart = commentStart;
			this.CommentEnd = commentEnd;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if(CommentStart != null)
                    hashCode += 1000000007 * CommentStart.GetHashCode();
				
                if(CommentEnd != null)
                    hashCode += 1000000009 * CommentEnd.GetHashCode();
				
                hashCode += 1000000021 * StartOffset.GetHashCode();
				hashCode += 1000000033 * EndOffset.GetHashCode();
			}
			return hashCode;
		}
		
		public override bool Equals(object obj)
		{
			BlockCommentRegion other = obj as BlockCommentRegion;
			if(other == null)
                return false;
			
            return this.CommentStart == other.CommentStart &&
				this.CommentEnd == other.CommentEnd &&
				this.StartOffset == other.StartOffset &&
				this.EndOffset == other.EndOffset;
		}
	}
}
