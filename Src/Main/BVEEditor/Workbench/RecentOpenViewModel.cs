/*
 * Created by SharpDevelop.
 * User: Ryouta
 * Date: 2013/06/17
 * Time: 14:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Shell;
using BVEEditor.Events;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of BVEEditor
	/// </summary>
	public sealed class RecentOpenViewModel : IRecentOpen, IHandle<FileEvent>, IHandle<FileRenameEvent>,
        IHandle<ViewDocumentAddedEvent>
	{
		/// <summary>
		/// This variable controls the maximal length of lastfile/lastopen entries
		/// must be positive
		/// </summary>
		static int MaxLength = 10;
		
		ObservableCollection<FileName> recent_files    = new ObservableCollection<FileName>();
		ObservableCollection<FileName> recent_projects = new ObservableCollection<FileName>();
		ICSharpCode.Core.Properties properties;
		
		public IReadOnlyList<FileName> RecentFiles{
			get{return recent_files;}
		}

		public IReadOnlyList<FileName> RecentProjects{
			get{return recent_projects;}
		}
		
		public RecentOpenViewModel(IPropertyService propertyService, IEventAggregator eventAggregator)
		{
            eventAggregator.Subscribe(this);

			// don't check whether files exist because that might be slow (e.g. if file is on network
			// drive that's unavailable)
			this.properties = propertyService.NestedProperties("RecentFilesEntry");
			recent_files.AddRange(properties.GetList<string>("Files").Select(FileName.Create));
			recent_projects.AddRange(properties.GetList<string>("Projects").Select(FileName.Create));
		}
		
		void AddRecentFile(FileName name)
		{
			recent_files.Remove(name); // remove the old one if the filename already exists in the list
			
			while(recent_files.Count >= MaxLength)
				recent_files.RemoveAt(recent_files.Count - 1);
			
			recent_files.Insert(0, name);
			properties.SetList("Files", recent_files);
		}
		
		void ClearRecentFiles()
		{
			recent_files.Clear();
			properties.SetList("Files", recent_files);
		}
		
		void ClearRecentProjects()
		{
			recent_projects.Clear();
			properties.SetList("Projects", recent_projects);
		}
		
		void AddRecentProject(FileName name)
		{
			recent_projects.Remove(name);
			
			while(recent_projects.Count >= MaxLength)
				recent_projects.RemoveAt(recent_projects.Count - 1);
			
			recent_projects.Insert(0, name);
			JumpList.AddToRecentCategory(name);
			properties.SetList("Projects", recent_projects);
		}
		
        #region IHandle<FileEvent> メンバー

        public void Handle(FileEvent message)
        {
            for(int i = 0; i < recent_files.Count; ++i){
                string file = recent_files[i].ToString();
                if(message.FileName == file){
                    recent_files.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region IHandle<FileRenameEvent> メンバー

        public void Handle(FileRenameEvent message)
        {
            for(int i = 0; i < recent_files.Count; ++i){
                string file = recent_files[i].ToString();
                if(message.SourceFile == file){
                    recent_files.RemoveAt(i);
                    recent_files.Insert(i, FileName.Create(message.TargetFile));
                    break;
                }
            }
        }

        #endregion

        #region IHandle<ViewDocumentAddedEvent> メンバー

        public void Handle(ViewDocumentAddedEvent message)
        {
            AddRecentFile(message.Document.FilePath);
        }

        #endregion
    }    
}
