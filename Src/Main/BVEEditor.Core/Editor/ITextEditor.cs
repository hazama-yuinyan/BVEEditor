/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/19
 * Time: 13:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Editor.LanguageBinding;
using BVEEditor.Workbench;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Editor
{
	/// <summary>
	/// Interface for text editors.
	/// </summary>
	public interface ITextEditor
	{
		/// <summary>
		/// Gets the document that is being edited.
		/// </summary>
		IDocument Document{get;}
		
		/// <summary>
		/// Gets an object that represents the caret inside this text editor.
		/// This property never returns null.
		/// </summary>
		ITextEditorCaret Caret{get;}
		
		/// <summary>
		/// Gets the set of options used in the text editor.
		/// This property never returns null.
		/// </summary>
		ITextEditorOptions Options{get;}

        ILanguageBinding Language{get;}
		
		/// <summary>
		/// Gets the start offset of the selection.
		/// </summary>
		int SelectionStart{get;}
		
		/// <summary>
		/// Gets the length of the selection.
		/// </summary>
		int SelectionLength{get;}
		
		/// <summary>
		/// Gets/Sets the selected text.
		/// </summary>
		string SelectedText{get; set;}

		/// <summary>
		/// Sets the selection.
		/// </summary>
		/// <param name="selectionStart">Start offset of the selection</param>
		/// <param name="selectionLength">Length of the selection</param>
		void Select(int selectionStart, int selectionLength);
		
		/// <summary>
		/// Is raised when the selection changes.
		/// </summary>
		event EventHandler SelectionChanged;

        /// <summary>
        /// 
        /// </summary>
        event TextCompositionEventHandler PreviewTextInput;

        /// <summary>
        /// 
        /// </summary>
        event KeyEventHandler PreviewKeyDown;

		/// <summary>
		/// Is raised before a key is pressed.
		/// </summary>
		event KeyEventHandler KeyDown;

        /// <summary>
        /// 
        /// </summary>
        event KeyEventHandler KeyUp;

		/// <summary>
		/// Sets the caret to the specified line/column and brings the caret into view.
		/// </summary>
		void JumpTo(int line, int column);
		
		FileName FileName{get;}
        UIElement EditorElement{get;}
		
		/// <summary>
		/// Gets the list of available code snippets.
		/// </summary>
		IEnumerable<ICompletionItem> GetSnippets();

        /// <summary>
        /// Gets the visual position of the caret.
        /// </summary>
        Rect GetCursorCoordinates();
		
		/// <summary>
		/// Gets the list of context action providers.
		/// </summary>
		//IList<IContextActionProvider> ContextActionProviders { get; }
	}
	
	public interface ITextEditorOptions : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the text used for one indentation level.
		/// </summary>
		string IndentationString { get; }
		
		/// <summary>
		/// Gets whether a '}' should automatically be inserted when a block is opened.
		/// </summary>
		bool AutoInsertBlockEnd { get; }
		
		/// <summary>
		/// Gets if tabs should be converted to spaces.
		/// </summary>
		bool ConvertTabsToSpaces { get; }
		
		/// <summary>
		/// Gets the size of an indentation level.
		/// </summary>
		int IndentationSize { get; }
		
		/// <summary>
		/// Gets the column of the vertical ruler (line that signifies the maximum line length
		/// defined by the coding style)
		/// This property returns a valid value even if the vertical ruler is set to be invisible.
		/// </summary>
		int VerticalRulerColumn { get; }
		
		/// <summary>
		/// Gets whether errors should be underlined.
		/// </summary>
		bool UnderlineErrors { get; }
		
		/// <summary>
		/// Gets the name of the currently used font.
		/// </summary>
		string FontFamily { get; }
	}
	
	/// <summary>
	/// Represents the caret in a text editor.
	/// </summary>
	public interface ITextEditorCaret
	{
		/// <summary>
		/// Gets/Sets the caret offset;
		/// </summary>
		int Offset { get; set; }

		/// <summary>
		/// Gets/Sets the caret line number.
		/// Line numbers are counted starting from 1.
		/// </summary>
		int Line { get; set; }

		/// <summary>
		/// Gets/Sets the caret column number.
		/// Column numbers are counted starting from 1.
		/// </summary>
		int Column { get; set; }

		/// <summary>
		/// Gets/sets the caret location.
		/// </summary>
		TextLocation Location { get; set; }

		/// <summary>
		/// Is raised whenever the location of the caret has changed.
		/// </summary>
		event EventHandler LocationChanged;
	}
}
