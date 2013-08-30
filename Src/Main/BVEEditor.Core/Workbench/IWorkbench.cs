/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/17
 * Time: 14:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// This is the basic interface to the workspace.
	/// </summary>
	public interface IWorkbench : IMementoCapable
	{
		/// <summary>
		/// Gets/Sets whether the window is displayed in full-screen mode.
		/// </summary>
		//bool FullScreen { get; set; }
		
		/// <summary>
		/// A collection in which all opened view contents (including all secondary view contents) are saved.
		/// </summary>
		ICollection<ViewContentViewModel> ViewContentCollection {
			get;
		}
		
		/// <summary>
		/// A collection in which all opened primary view contents are saved.
		/// </summary>
		ICollection<ViewContentViewModel> PrimaryViewContents {
			get;
		}
		
		/// <summary>
		/// A collection in which all active pad items are saved.
		/// </summary>
		IList<PadViewModel> Pads{
			get;
		}
		
		IList<ViewDocumentViewModel> ViewDocuments{
			get;
		}
		
		/// <summary>
		/// Is called, when the active view content has changed.
		/// </summary>
		//event EventHandler ActiveViewContentChanged;
		
		/// <summary>
		/// The active content, depending on where the focus currently is.
		/// If a document is currently active, this will be equal to ActiveViewContent,
		/// if a pad has the focus, this property will return the IPadContent instance.
		/// </summary>
		ViewDocumentViewModel ActiveDocument {
			get;
		}

        /// <summary>
        /// Gets the recently opened file/project list.
        /// </summary>
        IRecentOpen RecentOpen{
            get;
        }

        /// <summary>
        /// Creates a new <see cref="BVEEditor.Workbench.ViewDocumentViewModel"/> with the specified file path.
        /// </summary>
        /// <param name="filePath">
        /// The file from which the ViewDocument loads its content.
        /// If it is null, then that means that it should create a ViewDocument with empty content.
        /// </param>
        void CreateViewDocumentViewModel(string filePath);

        /// <summary>
        /// Attempts to save the ViewDocument and shows the save-as dialog if needed.
        /// </summary>
        /// <param name="document">The ViewDocument to be saved</param>
        /// <param name="doQuickSave">Flag hinting that it should show the save-as dialog</param>
        IEnumerable<IResult> SaveDocument(ViewDocumentViewModel document, bool doQuickSave);
		
		/// <summary>
		/// Is called, when the active content has changed.
		/// </summary>
		//event EventHandler ActiveContentChanged;
		
		/// <summary>
		/// Gets whether BVEEditor is the active application in Windows.
		/// </summary>
		/*bool IsActiveWindow {
			get;
		}*/
		
		/// <summary>
		/// Gets/Sets the name of the current layout configuration.
		/// Setting this property causes the current layout to be saved, and the specified layout to be loaded.
		/// </summary>
		//string CurrentLayoutConfiguration { get; set; }
		
		/// <summary>
		/// Is called, when a workbench view was opened
		/// </summary>
		/// <example>
		/// WorkbenchSingleton.WorkbenchCreated += delegate {
		/// 	WorkbenchSingleton.Workbench.ViewOpened += ...;
		/// };
		/// </example>
		//event EventHandler<ViewContentEvent> ViewOpened;
		
		/// <summary>
		/// Is called, when a workbench view was closed
		/// </summary>
		//event EventHandler<ViewContentEvent> ViewClosed;
	}
}
