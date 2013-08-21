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
using System.Windows.Media;
using BVEEditor.Events;
using BVEEditor.Result;
using BVEEditor.Services;
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
	public abstract class ViewDocumentViewModel : PaneViewModel, IHandle<FileRenameEvent>
	{
		readonly static string ContextMenuPath = "/BVEEditor/Workbench/OpenFileTab/ContextMenu";
		
        protected readonly IFileSystem file_system;
        protected readonly IEventAggregator event_aggregator;
        protected readonly IResultFactory result_factory;
		
        #region Properties
        /// <summary>
		/// Gets the file name as string.
		/// </summary>
		public string FileName{
			get{
				return System.IO.Path.GetFileName(this.FilePath);
			}
		}

        public virtual FileName FilePath{
            get; set;
        }
		
		bool is_dirty;
		public virtual bool IsDirty{
			get{return is_dirty;}
            set{
                if(is_dirty != value){
                    is_dirty = value;
                    NotifyOfPropertyChange(() => is_dirty);
                }
            }
		}
		
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

        bool is_untitled;
        public bool IsUntitled{
            get{return is_untitled;}
            set{
                if(is_untitled != value){
                    is_untitled = value;
                    NotifyOfPropertyChange(() => is_untitled);
                }
            }
        }
		#endregion
		
		public ViewDocumentViewModel(IFileSystem fileSystem, IEventAggregator eventAggregator, IResultFactory resultFactory)
		{
            file_system = fileSystem;
            event_aggregator = eventAggregator;
            result_factory = resultFactory;
		}
		
		#region Dispose
		bool is_disposed;
		public bool IsDisposed {
			get{return is_disposed;}
		}
		
		event EventHandler Disposed;
		
		public virtual void Dispose()
		{
			is_disposed = true;
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		#endregion

        public virtual ViewDocumentViewModel Configure(string fileToOpen)
        {
            FilePath = ICSharpCode.Core.FileName.Create(fileToOpen);
            Title = System.IO.Path.GetFileName(fileToOpen);
            IsUntitled = string.IsNullOrEmpty(fileToOpen);
            return this;
        }
		
        /// <summary>
        /// Saves the content of this ViewDocument to the specified file.
        /// </summary>
        /// <param name="filePath">The file path to save the content to.</param>
        /// <returns></returns>
        public virtual void Save(string filePath)
        {
        }

        /// <summary>
        /// Loads the content into this ViewDocument.
        /// </summary>
        /// <param name="filePath">The file path to load the content from.</param>
        /// <returns></returns>
        public virtual void Load(string filePath)
        {
        }
		
		public void Select()
		{
            //event_aggregator.Publish(new ActiveViewContentChangedEvent(ViewContents[0]));
		}

        #region IHandle<FileRenameEvent> メンバー

        public void Handle(FileRenameEvent message)
        {
            if(!message.IsDirectory){
                FilePath = ICSharpCode.Core.FileName.Create(message.TargetFile);
                Title = System.IO.Path.GetFileName(FilePath);
                IsUntitled = false;
            }
        }

        #endregion
    }
}
