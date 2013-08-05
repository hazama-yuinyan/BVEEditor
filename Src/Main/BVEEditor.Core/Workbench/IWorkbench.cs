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
		/// The title shown in the title bar.
		/// </summary>
		string Title {
			get;
			set;
		}
		
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
		
		IList<ViewDocumentViewModel> Documents{
			get;
		}
		
		/// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
		ViewContentViewModel ActiveViewContent {
			get; set;
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
		/// Inserts a new <see cref="ViewDocumentViewModel"/> object in the workspace and switches to the new view.
		/// </summary>
		void AddDocument(ViewDocumentViewModel document);
		
		/// <summary>
		/// Inserts a new <see cref="ViewDocumentViewModel"/> object in the workspace.
		/// </summary>
		void ShowDocument(ViewDocumentViewModel document, bool switchToOpenedView);
		
		/// <summary>
		/// Activates the specified pad.
		/// </summary>
		void ActivatePad(PadDescriptor content);
		
		/// <summary>
		/// Returns a pad from a specific type.
		/// </summary>
		PadViewModel GetPad(Type type);
		
		/// <summary>
		/// Closes the specified ViewDocument. It also prompts the user whether it should save the content.
		/// </summary>
		/// <param name="document"></param>
		//void Close(ViewDocumentViewModel document);
		
		/// <summary>
		/// Saves the content in the ViewDocument.
		/// </summary>
		/// <param name="document"></param>
		/// <param name="saveAsFlag"></param>
		void Save(ViewDocumentViewModel document, bool saveAsFlag = false);
		
		/// <summary>
		/// Opens the ViewDocument.
		/// </summary>
		/// <param name="document"></param>
		void Open(ViewDocumentViewModel document);
		
		/// <summary>
		/// Makes a new ViewDocument.
		/// </summary>
		void New();
		
		ICommand NewCommand{
			get;
		}
		
		ICommand OpenCommand{
			get;
		}
		
		/// <summary>
		/// Closes all views inside the workbench.
		/// </summary>
		void CloseAllViews();
		
		/// <summary>
		/// 	Closes all views related to current solution.
		/// </summary>
		/// <returns>
		/// 	True if all views were closed properly, false if closing was aborted.
		/// </returns>
		bool CloseAllSolutionViews(bool force);
		
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
