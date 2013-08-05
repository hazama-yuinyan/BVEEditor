/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/17
 * Time: 1:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// Represents an opened file. It also acts as the model for a view content in the MVVM pattern.
	/// </summary>
	public abstract class OpenedFile : ICanBeDirty
	{
		protected ViewContentViewModel current_view;
		bool in_load_operation;
		bool in_save_operation;
        protected IFileService file_service;

        public OpenedFile(IFileService fileService)
        {
            file_service = fileService;
        }
		
		/// <summary>
		/// holds unsaved file content in memory when view containing the file was closed but no other view
		/// activated
		/// </summary>
		byte[] file_data;
		
		#region IsDirty
		bool is_dirty;
		public event EventHandler IsDirtyChanged;
		
		/// <summary>
		/// Gets/sets if the file is has unsaved changes.
		/// </summary>
		public bool IsDirty{
			get{return is_dirty;}
			set{
				if(is_dirty != value){
					is_dirty = value;
					if(IsDirtyChanged != null)
						IsDirtyChanged(this, EventArgs.Empty);
				}
			}
		}
		
		/// <summary>
		/// Marks the file as dirty if it currently is not in a load operation.
		/// </summary>
		public virtual void MakeDirty()
		{
			if(!in_load_operation)
				this.IsDirty = true;
		}
		#endregion
		
		bool is_untitled;
		
		/// <summary>
		/// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
		/// </summary>
		public bool IsUntitled {
			get { return is_untitled; }
			protected set { is_untitled = value; }
		}
		
		FileName file_name;
		
		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		public FileName FileName{
			get{ return file_name; }
			set{
				if(file_name != value)
					ChangeFileName(value);
			}
		}
		
		public event EventHandler FileNameChanged;
		
		protected virtual void ChangeFileName(FileName newValue)
		{
			//SD.MainThread.VerifyAccess();
			
			file_name = newValue;
			if(FileNameChanged != null)
				FileNameChanged(this, EventArgs.Empty);
		}
		
		public abstract event EventHandler FileClosed;
		
		/// <summary>
		/// Use this method to save the file to disk using a new name.
		/// </summary>
		public void SaveToDisk(FileName newFileName)
		{
			this.FileName = newFileName;
			this.IsUntitled = false;
			SaveToDisk();
		}
		
		public abstract void RegisterView(ViewContentViewModel view);
		public abstract void UnregisterView(ViewContentViewModel view);
		
		public virtual void CloseIfAllViewsClosed()
		{
		}
		
		/// <summary>
		/// Forces initialization of the specified view.
		/// </summary>
		public virtual void ForceInitializeView(ViewContentViewModel view)
		{
			if(view == null)
				throw new ArgumentNullException("view");
			
			bool success = false;
			try{
				if(current_view != view){
					if(current_view == null){
						SwitchedToView(view);
					}else{
						try{
							in_load_operation = true;
							using(Stream source_stream = OpenRead()){
								view.Load(this, source_stream);
							}
						}finally{
							in_load_operation = false;
						}
					}
				}
				success = true;
			}finally{
				// Only in case of exceptions:
				// (try-finally with bool is better than try-catch-rethrow because it causes the debugger to stop
				// at the original error location, not at the rethrow)
			}
		}
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public abstract IList<ViewContentViewModel> RegisteredViewContents {
			get;
		}
		
		/// <summary>
		/// Gets the view content that currently edits this file.
		/// If there are multiple view contents registered, this returns the view content that was last
		/// active. The property might return null even if view contents are registered if the last active
		/// content was closed. In that case, the file is stored in-memory and loaded again when one of the
		/// registered view contents becomes active.
		/// </summary>
		public ViewContentViewModel CurrentView {
			get { return current_view; }
		}
		
		/// <summary>
		/// Opens the file for reading.
		/// </summary>
		public virtual Stream OpenRead()
		{
			if(file_data != null)
				return new MemoryStream(file_data, false);
			else
				return new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
		
		/// <summary>
		/// Sets the internally stored data to the specified byte array.
		/// This method should only be used when there is no current view or by the
		/// current view.
		/// </summary>
		/// <remarks>
		/// Use this method to specify the initial file content if you use a OpenedFile instance
		/// for a file that doesn't exist on disk but should be automatically created when a view
		/// with the file is saved, e.g. for .resx files created by the forms designer.
		/// </remarks>
		public virtual void SetData(byte[] fileData)
		{
			if(fileData == null)
				throw new ArgumentNullException("fileData");
			if(in_load_operation)
				throw new InvalidOperationException("SetData cannot be used while loading");
			if(in_save_operation)
				throw new InvalidOperationException("SetData cannot be used while saving");
			
			this.file_data = fileData;
		}
		
		/// <summary>
		/// Save the file to disk using the current name.
		/// </summary>
		public virtual void SaveToDisk()
		{
			if(IsUntitled)
				throw new InvalidOperationException("Cannot save an untitled file to disk!");
			
			LoggingService.Debug("Save " + FileName);
			bool safe_saving = file_service.SaveUsingTemporaryFile && File.Exists(FileName);
			string saveAs = safe_saving ? FileName + ".bak" : FileName;
			using(FileStream fs = new FileStream(saveAs, FileMode.Create, FileAccess.Write)){
				if(current_view != null)
					SaveCurrentViewToStream(fs);
				else
					fs.Write(file_data, 0, file_data.Length);
			}
			
			if(safe_saving){
				DateTime creation_time = File.GetCreationTimeUtc(FileName);
				File.Delete(FileName);
				try{
					File.Move(saveAs, FileName);
				}catch(UnauthorizedAccessException){
					// sometimes File.Move raise exception (TortoiseSVN, Anti-vir ?)
					// try again after short delay
					System.Threading.Thread.Sleep(250);
					File.Move(saveAs, FileName);
				}
				File.SetCreationTimeUtc(FileName, creation_time);
			}
			IsDirty = false;
		}
		
//		/// <summary>
//		/// Called before saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
//		/// </summary>
//		public event EventHandler SavingCurrentView;
//
//		/// <summary>
//		/// Called after saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
//		/// </summary>
//		public event EventHandler SavedCurrentView;
		
		
		void SaveCurrentViewToStream(Stream stream)
		{
//			if (SavingCurrentView != null)
//				SavingCurrentView(this, EventArgs.Empty);
			in_save_operation = true;
			try{
				current_view.Save(this, stream);
			}finally{
				in_save_operation = false;
			}
//			if (SavedCurrentView != null)
//				SavedCurrentView(this, EventArgs.Empty);
		}
		
		protected void SaveCurrentView()
		{
			using(MemoryStream memory_stream = new MemoryStream()){
				SaveCurrentViewToStream(memory_stream);
				file_data = memory_stream.ToArray();
			}
		}
		
		
		public void SwitchedToView(ViewContentViewModel newView)
		{
			if(newView == null)
				throw new ArgumentNullException("newView");
			
			if(current_view == newView)
				return;
			
			/*if(current_view != null){
				if (newView.SupportsSwitchToThisWithoutSaveLoad(this, current_view)
				    || current_view.SupportsSwitchFromThisWithoutSaveLoad(this, newView)){
					// switch without Save/Load
					current_view.SwitchFromThisWithoutSaveLoad(this, newView);
					newView.SwitchToThisWithoutSaveLoad(this, current_view);
					
					current_view = newView;
					return;
				}
				SaveCurrentView();
			}
			
			try{
				in_load_operation = true;
				ICSharpCode.Core.Properties memento = GetMemento(newView);
				using(Stream source_stream = OpenRead()){
					ViewContentViewModel old_view = current_view;
					bool success = false;
					try{
						current_view = newView;
						// don't reset fileData if the file is untitled, because OpenRead() wouldn't be able to read it otherwise
						if(this.IsUntitled == false)
							file_data = null;
						
						newView.Load(this, source_stream);
						success = true;
					}finally{
						// Use finally instead of catch+rethrow so that the debugger
						// breaks at the original crash location.
						if(!success){
							// stay with old view in case of exceptions
							current_view = old_view;
						}
					}
				}
				RestoreMemento(newView, memento);
			}finally{
				in_load_operation = false;
			}*/
		}
		
		public virtual void ReloadFromDisk()
		{
			var r = FileUtility.ObservedLoad(ReloadFromDiskInternal, FileName);
			if(r == FileOperationResult.Failed){
				//if(current_view != null)
				//	SD.Workbench.Close(current_view.ViewDocument);
			}
		}
		
		void ReloadFromDiskInternal()
		{
			file_data = null;
			if(current_view != null){
				try{
					in_load_operation = true;
					ICSharpCode.Core.Properties memento = GetMemento(current_view);
					using(Stream sourceStream = OpenRead()){
						current_view.Load(this, sourceStream);
					}
					IsDirty = false;
					RestoreMemento(current_view, memento);
				}finally{
					in_load_operation = false;
				}
			}
		}
		
		static ICSharpCode.Core.Properties GetMemento(ViewContentViewModel viewContent)
		{
			return null;
			/*IMementoCapable memento_capable = viewContent.GetService<IMementoCapable>();
			if(memento_capable == null)
				return null;
			else
				return memento_capable.CreateMemento();*/
		}
		
		static void RestoreMemento(ViewContentViewModel viewContent, ICSharpCode.Core.Properties memento)
		{
			if(memento != null)
				((IMementoCapable)viewContent).SetMemento(memento);
		}
	}
}
