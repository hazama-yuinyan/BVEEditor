/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/07/10
 * Time: 12:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BVEEditor.Events;
using Caliburn.Micro;
using ICSharpCode.Core;
using Xceed.Wpf.AvalonDock.Layout;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// The ViewDocumentViewModel is the base view model whose content is shown in the document area.
	/// </summary>
	/// <remarks>
	/// A ViewDocument can contain multiple tabs. The first content(on screen it will show in the leftmost) is called the primary content,
	/// and the others are called secondary contents.
	/// </remarks>
	public abstract class ViewDocumentViewModel : PaneViewModel
	{
		readonly static string ContextMenuPath = "/BVEEditor/Workbench/OpenFileTab/ContextMenu";
		
        IFileService file_service;
        IEventAggregator event_aggregator;
		
		#region File
		OpenedFile file;
		public OpenedFile File{
			get{return file;}
			set{
				if(file != null)
					UnregisterFileEventHandlers(file);
				
				file = value;
				if(value != null)
					RegisterFileEventHandlers(file);
			}
		}
		
		bool automatically_register_view_on_files = true;
		internal bool AutomaricallyRegisterViewOnFiles{
			get{return automatically_register_view_on_files;}
			set{automatically_register_view_on_files = value;}
		}
		
		void RegisterFileEventHandlers(OpenedFile newItem)
		{
			newItem.FileNameChanged += OnFileNameChanged;
			newItem.IsDirtyChanged += OnIsDirtyChanged;
			if(automatically_register_view_on_files){
				//foreach(var vm in view_contents)
				//	newItem.RegisterView(vm);
			}
			
			OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
		}
		
		void UnregisterFileEventHandlers(OpenedFile oldItem)
		{
			oldItem.FileNameChanged -= OnFileNameChanged;
			oldItem.IsDirtyChanged -= OnIsDirtyChanged;
			if(automatically_register_view_on_files){
				//foreach(var vm in view_contents)
				//	oldItem.UnregisterView(vm);
			}
			
			OnIsDirtyChanged(null, EventArgs.Empty); // re-evaluate this.IsDirty after changing the file collection
		}
		
		void OnFileNameChanged(object sender, EventArgs e)
		{
			OnFileNameChanged((OpenedFile)sender);
			NotifyOfPropertyChange(() => file.FileName);
			
			if(Title == null && file != null){
				OnTitleNameChanged(EventArgs.Empty);
				NotifyOfPropertyChange(() => Title);
			}
		}
		
		/// <summary>
		/// Is called when the file name of a file opened in this view content changes.
		/// </summary>
		void OnFileNameChanged(OpenedFile file)
		{
			Title = file.FileName;
		}
		
		void OnTitleNameChanged(EventArgs e)
		{
			
		}
		#endregion
		
		/// <summary>
		/// Gets the file name in string.
		/// </summary>
		public string FileName{
			get{
				return file.FileName.GetFileName();
			}
		}
		
		#region Dirty
		bool IsDirtyInternal{
			get{
				return file.IsDirty;
			}
		}
		
		bool is_dirty;
		public bool IsDirty{
			get{return is_dirty;}
		}
		
		void OnIsDirtyChanged(object sender, EventArgs e)
		{
			bool new_is_dirty = IsDirtyInternal;
			if(new_is_dirty != is_dirty){
				is_dirty = new_is_dirty;
				NotifyOfPropertyChange(() => is_dirty);
			}
		}
		#endregion
		
		#region InfoTip
		string info_tip;
		/// <summary>
		/// The tooltip that will be shown when you hover the mouse over the title
		/// </summary>
		public string InfoTip{
			get{return info_tip;}
			set{
				if(info_tip != value){
					info_tip = value;
					NotifyOfPropertyChange(() => info_tip);
				}
			}
		}
		#endregion
		
		public ViewDocumentViewModel(IFileService fileService, IEventAggregator eventAggregator)
		{
            file_service = fileService;
            event_aggregator = eventAggregator;
		}
		
		#region Dispose
		bool is_disposed;
		public bool IsDisposed {
			get{return is_disposed;}
		}
		
		event EventHandler Disposed;
		
		public void Dispose()
		{
			if(automatically_register_view_on_files)
				File = null;
			
			is_disposed = true;
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		#endregion

        public ViewDocumentViewModel Configure(FileName fileToOpen)
        {
            //File = file_service.GetOrCreateOpenedFile(fileToOpen);
            return this;
        }
		
		public void AddView()
		{
			
		}
		
		public void Select()
		{
            //event_aggregator.Publish(new ActiveViewContentChangedEvent(ViewContents[0]));
		}
	}
}
