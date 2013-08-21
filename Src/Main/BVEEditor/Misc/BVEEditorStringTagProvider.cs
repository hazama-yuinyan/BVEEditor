/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/26
 * Time: 16:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows;
using BVEEditor.Editor;
using BVEEditor.Workbench;
using ICSharpCode.Core;

namespace BVEEditor.Commands
{
	/// <summary>
	/// Provides tag to string mapping for BVEEditor. Tags are mapped to strings by several methods
	/// such as registry and resource files.
	/// </summary>
	public class BVEEditorStringTagProvider : IStringTagProvider
	{
        readonly IWorkbench workbench;

        public BVEEditorStringTagProvider(IWorkbench workbench)
        {
            this.workbench = workbench;
        }

		string GetCurrentItemPath()
		{
			return workbench.ActiveDocument.FileName;
		}
		
		string GetCurrentTargetPath()
		{
			/*if (ProjectService.CurrentProject != null) {
				return ProjectService.CurrentProject.OutputAssemblyFullPath;
			}
			/*if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
				return ProjectService.GetOutputAssemblyName(fileName);
			}*/
			return String.Empty;
		}
		
		public string ProvideString(string tag, StringTagPair[] customTags)
		{
			return ProvideString(tag);
		}
		
		public string ProvideString(string tag)
		{
			/*switch (tag) {
				case "TaskService.Warnings":
					return TaskService.GetCount(TaskType.Warning).ToString();
				case "TaskService.Errors":
					return TaskService.GetCount(TaskType.Error).ToString();
				case "TaskService.Messages":
					return TaskService.GetCount(TaskType.Message).ToString();
				case "CurrentProjectName":
					if (ProjectService.CurrentProject == null)
						return "<no current project>";
					else
						return ProjectService.CurrentProject.Name;
			}*/
			
            switch(tag.ToUpperInvariant()){
				case "ITEMPATH":
					try{
						return GetCurrentItemPath() ?? string.Empty;
					}catch(Exception){}
					return string.Empty;

				case "ITEMDIR":
					try{
						return Path.GetDirectoryName(GetCurrentItemPath()) ?? string.Empty;
					}catch(Exception){}
					return string.Empty;

				case "ITEMFILENAME":
					try{
						return Path.GetFileName(GetCurrentItemPath()) ?? string.Empty;
					}catch(Exception){}
					return string.Empty;

				case "ITEMEXT":
					try{
						return Path.GetExtension(GetCurrentItemPath()) ?? string.Empty;
					}catch(Exception){}
					return string.Empty;

				case "ITEMNAMENOEXT":
					try{
						return Path.GetFileNameWithoutExtension(GetCurrentItemPath()) ?? string.Empty;
					}catch(Exception){}
					return string.Empty;
					
				case "CURLINE":
					{
						IPositionable positionable = workbench.ActiveDocument as IPositionable;
						if(positionable != null)
							return positionable.Line.ToString();
						
                        return string.Empty;
					}
				case "CURCOL":
					{
						IPositionable positionable = workbench.ActiveDocument as IPositionable;
						if(positionable != null)
							return positionable.Column.ToString();
						
                        return string.Empty;
					}
				case "CURTEXT":
					{
						ITextEditor editor = workbench.ActiveDocument as ITextEditor;
						if(editor != null)
							return editor.SelectedText;
						
						return string.Empty;
					}

				case "TARGETPATH":
					try {
						return GetCurrentTargetPath() ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;

				case "TARGETDIR":
					try {
						return Path.GetDirectoryName(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;

				case "TARGETNAME":
					try {
						return Path.GetFileName(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;

				case "TARGETEXT":
					try {
						return Path.GetExtension(GetCurrentTargetPath()) ?? string.Empty;
					} catch (Exception) {}
					return string.Empty;

				case "PROJECTDIR":
					/*if (ProjectService.CurrentProject != null) {
						return ProjectService.CurrentProject.Directory;
					}*/
					return string.Empty;

				case "PROJECTFILENAME":
					/*if (ProjectService.CurrentProject != null) {
						try {
							return Path.GetFileName(ProjectService.CurrentProject.FileName);
						} catch (Exception) {}
					}*/
					return string.Empty;

				case "COMBINEDIR":
				case "SOLUTIONDIR":
					//return Path.GetDirectoryName(ProjectService.OpenSolution.FileName);

				case "SOLUTIONFILENAME":
				case "COMBINEFILENAME":
					try {
					//	return Path.GetFileName(ProjectService.OpenSolution.FileName);
					} catch (Exception) {}
					return string.Empty;

				case "BVEEDITORBINPATH":
					return Path.GetDirectoryName(typeof(BVEEditorStringTagProvider).Assembly.Location);

				case "STARTUPPATH":
					return Application.Current.StartupUri.ToString();

				default:
					return null;
			}
		}
	}
}
