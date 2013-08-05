/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/27
 * Time: 15:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	sealed class FileServiceOpenedFile : OpenedFile
	{
		readonly IFileService file_service;
		List<ViewContentViewModel> registered_views = new List<ViewContentViewModel>();
		//FileChangeWatcher file_change_watcher;
		
		protected override void ChangeFileName(FileName newValue)
		{
			file_service.OpenedFileFileNameChange(this, this.FileName, newValue);
			base.ChangeFileName(newValue);
		}
		
		internal FileServiceOpenedFile(IFileService fileService, FileName fileName) : base(fileService)
		{
			this.file_service = fileService;
			this.FileName = fileName;
			IsUntitled = false;
			///file_change_watcher = new FileChangeWatcher(this);
		}
		
		internal FileServiceOpenedFile(IFileService fileService, byte[] fileData) : base(fileService)
		{
			this.file_service = fileService;
			this.FileName = null;
			SetData(fileData);
			IsUntitled = true;
			MakeDirty();
			//file_change_watcher = new FileChangeWatcher(this);
		}
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public override IList<ViewContentViewModel> RegisteredViewContents {
			get { return registered_views.AsReadOnly(); }
		}
		
		public override void ForceInitializeView(ViewContentViewModel view)
		{
			if(view == null)
				throw new ArgumentNullException("view");
			
			if(!registered_views.Contains(view))
				throw new ArgumentException("registeredViews must contain view");
			
			base.ForceInitializeView(view);
		}
		
		public override void RegisterView(ViewContentViewModel view)
		{
			if(view == null)
				throw new ArgumentNullException("view");
			
			if(registered_views.Contains(view))
				throw new ArgumentException("registeredViews already contains view");
			
			registered_views.Add(view);
			
			if(SD.Workbench != null){
				//SD.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
				if(SD.Workbench.ActiveViewContent == view)
					SwitchedToView(view);
			}
			#if DEBUG
			//view.Disposed += ViewDisposed;
			#endif
		}
		
		public override void UnregisterView(ViewContentViewModel view)
		{
			if(view == null)
				throw new ArgumentNullException("view");
			
			Debug.Assert(registered_views.Contains(view));
			
			if(SD.Workbench != null)
				//SD.Workbench.ActiveViewContentChanged -= WorkbenchActiveViewContentChanged;
			#if DEBUG
			//view.Disposed -= ViewDisposed;
			#endif
			
			registered_views.Remove(view);
			if(registered_views.Count > 0){
				if(current_view == view){
					SaveCurrentView();
					current_view = null;
				}
			}else{
				// all views to the file were closed
				CloseIfAllViewsClosed();
			}
		}
		
		public override void CloseIfAllViewsClosed()
		{
			if(registered_views.Count == 0){
				bool wasDirty = this.IsDirty;
				file_service.OpenedFileClosed(this);
				
				FileClosed(this, EventArgs.Empty);
				
				/*if(file_change_watcher != null){
					file_change_watcher.Dispose();
					file_change_watcher = null;
				}*/
				
				if(wasDirty){
					// We discarded some information when closing the file,
					// so we need to re-parse it.
					/*if (File.Exists(this.FileName))
						SD.ParserService.ParseAsync(this.FileName).FireAndForget();
					else
						SD.ParserService.ClearParseInformation(this.FileName);*/
				}
			}
		}
		
		#if DEBUG
		void ViewDisposed(object sender, EventArgs e)
		{
			Debug.Fail("View was disposed while still registered with OpenedFile!");
		}
		#endif
		
		void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			ViewContentViewModel newView = SD.Workbench.ActiveViewContent;
			
			if (!registered_views.Contains(newView))
				return;
			
			SwitchedToView(newView);
		}
		
		public override void SaveToDisk()
		{
			try{
				//if(file_change_watcher != null)
				//	file_change_watcher.Enabled = false;
				base.SaveToDisk();
			}finally{
				//if(file_change_watcher != null)
				//	file_change_watcher.Enabled = true;
			}
		}
		
		public override event EventHandler FileClosed = delegate {};
	}
}
