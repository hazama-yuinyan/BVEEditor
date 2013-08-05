/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/07/10
 * Time: 14:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// ViewContentViewModel is the view model for "tabs" in the document area of BVEEditor.
	/// A view content represents either a view onto a single file, or other content that opens like a document
	/// (e.g. the start page).
	/// </summary>
	public class ViewContentViewModel : PaneViewModel
	{
		ViewDocumentViewModel parent;
		
		/// <summary>
		/// Is raised when the value of the TabPageText property changes.
		/// </summary>
		event EventHandler TabPageTextChanged;
		
		string tab_page_text;
		/// <summary>
		/// The text on the tab page when more than one view content
		/// is attached to a single ViewDocument.
		/// </summary>
		string TabPageText{
			get{return tab_page_text;}
			set{
				if(tab_page_text != value){
					tab_page_text = value;
					if(TabPageTextChanged != null)
						TabPageTextChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public ViewDocumentViewModel ViewDocument{
			get{return parent;}
		}
		
		public ViewContentViewModel(ViewDocumentViewModel viewDocument)
		{
			parent = viewDocument;
		}
		
		/// <summary>
		/// Saves the content to the location <code>fileName</code>
		/// </summary>
		/// <remarks>
		/// When the user switches between multiple views editing the same file, a view
		/// change will trigger one view content to save that file into a memory stream
		/// and the other view content will load the file from that memory stream.
		/// </remarks>
		public virtual void Save(OpenedFile file, Stream stream)
		{
		}
		
		/// <summary>
		/// Load or reload the content of the specified file from the stream.
		/// </summary>
		/// <remarks>
		/// When the user switches between multiple views editing the same file, a view
		/// change will trigger one view content to save that file into a memory stream
		/// and the other view content will load the file from that memory stream.
		/// </remarks>
		public virtual void Load(OpenedFile file, Stream stream)
		{
		}
		
		/// <summary>
		/// Builds an <see cref="INavigationPoint"/> for the current position.
		/// </summary>
		public virtual INavigationPoint BuildNavPoint()
		{
			return null;
		}
		
		/// <summary>
		/// Gets if the view content is read-only (can be saved only when choosing another file name).
		/// </summary>
		public virtual bool IsReadOnly{
			get{return false;}
		}
		
		/// <summary>
		/// Gets if the view content is view-only (cannot be saved at all).
		/// </summary>
		public virtual bool IsViewOnly{
			get{return false;}
		}
	}
}
