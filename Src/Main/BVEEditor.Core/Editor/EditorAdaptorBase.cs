/*
 * Created by SharpDevelop.
 * User: HAZAMA
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
	/// Base class for editor adaptors.
	/// </summary>
	public abstract class EditorAdaptorBase : DependencyObject
	{
		/// <summary>
		/// Gets the document that is being edited.
		/// </summary>
		public abstract IDocument Document{get;}
		
        /// <summary>
        /// Gets the offset of the caret.
        /// </summary>
        public abstract int CaretOffset{get;}
		
		/// <summary>
		/// Gets the content text.
		/// </summary>
		public abstract string Text{get;}
		
		public abstract UIElement UIElement{get;}
		
		/// <summary>
		/// Is raised when the selection changes.
		/// </summary>
		public abstract event EventHandler SelectionChanged;

        /// <summary>
        /// 
        /// </summary>
        public abstract event KeyEventHandler PreviewKeyDown;
		
		/// <summary>
		/// Is raised before a key is pressed.
		/// </summary>
		public abstract event KeyEventHandler KeyDown;

        /// <summary>
        /// 
        /// </summary>
        public abstract event KeyEventHandler KeyUp;

        /// <summary>
        /// 
        /// </summary>
        public abstract event TextCompositionEventHandler PreviewTextInput;

        public abstract Rect GetVisualPosition();
        public abstract bool IsSameLine(int charIndex1, int charIndex2);
        public abstract void Focus();
	}
}
