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
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor
{
	/// <summary>
	/// This class handles the recent open files and the recent open project files of BVEEditor
	/// </summary>
	sealed class RecentOpen : IRecentOpen, IHandle<FileEvent>, IHandle<FileRenameEvent>
	{
		/// <summary>
		/// This variable is the maximal length of lastfile/lastopen entries
		/// must be > 0
		/// </summary>
		int MAX_LENGTH = 10;
		
		ObservableCollection<FileName> recentFiles    = new ObservableCollection<FileName>();
		ObservableCollection<FileName> recentProjects = new ObservableCollection<FileName>();
		ICSharpCode.Core.Properties properties;
		
		public IReadOnlyList<FileName> RecentFiles {
			get { return recentFiles; }
		}

		public IReadOnlyList<FileName> RecentProjects {
			get { return recentProjects; }
		}
		
		public RecentOpen(ICSharpCode.Core.Properties p)
		{
			// don't check whether files exist because that might be slow (e.g. if file is on network
			// drive that's unavailable)
			this.properties = p;
			recentFiles.AddRange(p.GetList<string>("Files").Select(FileName.Create));
			recentProjects.AddRange(p.GetList<string>("Projects").Select(FileName.Create));
		}
		
		public void AddRecentFile(FileName name)
		{
			recentFiles.Remove(name); // remove if the filename is already in the list
			
			while(recentFiles.Count >= MAX_LENGTH)
				recentFiles.RemoveAt(recentFiles.Count - 1);
			
			recentFiles.Insert(0, name);
			properties.SetList("Files", recentFiles);
		}
		
		public void ClearRecentFiles()
		{
			recentFiles.Clear();
			properties.SetList("Files", recentFiles);
		}
		
		public void ClearRecentProjects()
		{
			recentProjects.Clear();
			properties.SetList("Projects", recentProjects);
		}
		
		public void AddRecentProject(FileName name)
		{
			recentProjects.Remove(name);
			
			while(recentProjects.Count >= MAX_LENGTH)
				recentProjects.RemoveAt(recentProjects.Count - 1);
			
			recentProjects.Insert(0, name);
			JumpList.AddToRecentCategory(name);
			properties.SetList("Projects", recentProjects);
		}
		
        #region IHandle<FileEvent> メンバー

        public void Handle(FileEvent message)
        {
            for(int i = 0; i < recentFiles.Count; ++i){
                string file = recentFiles[i].ToString();
                if(message.FileName == file){
                    recentFiles.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region IHandle<FileRenameEvent> メンバー

        public void Handle(FileRenameEvent message)
        {
            for(int i = 0; i < recentFiles.Count; ++i){
                string file = recentFiles[i].ToString();
                if(message.SourceFile == file){
                    recentFiles.RemoveAt(i);
                    recentFiles.Insert(i, FileName.Create(message.TargetFile));
                    break;
                }
            }
        }

        #endregion
    }
}
