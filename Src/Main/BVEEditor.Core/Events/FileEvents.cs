/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/17
 * Time: 1:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Events
{
	public class FileEvent
	{
		public string FileName{get; private set;}
		
		public bool IsDirectory{get; private set;}
		
		public FileEvent(string fileName, bool isDirectory)
		{
			FileName = fileName;
			IsDirectory = isDirectory;
		}
	}
	
	public class FileCancelEvent : FileEvent
	{
		public bool Cancel{get; set;}
		public bool OperationAlreadyDone{get; set;}
		
		public FileCancelEvent(string fileName, bool isDirectory) : base(fileName, isDirectory)
		{
		}
	}

    /// <summary>
    /// An event fired immediately before a file is renamed.
    /// </summary>
    public class FileRenamingEvent : FileRenameEvent
    {
        public bool Cancel
        {
            get;
            set;
        }

        public bool OperationAlreadyDone
        {
            get;
            set;
        }

        public FileRenamingEvent(string sourceFile, string targetFile, bool isDirectory)
            : base(sourceFile, targetFile, isDirectory)
        {
        }
    }

    /// <summary>
    /// An event fired immediately after a file is renamed.
    /// </summary>
    public class FileRenameEvent
    {
        public string SourceFile
        {
            get;
            private set;
        }

        public string TargetFile
        {
            get;
            private set;
        }

        public bool IsDirectory
        {
            get;
            private set;
        }

        public FileRenameEvent(string sourceFile, string targetFile, bool isDirectory)
        {
            SourceFile = sourceFile;
            TargetFile = targetFile;
            IsDirectory = isDirectory;
        }
    }

    /// <summary>
    /// An event fired immediately before a file is removed.
    /// </summary>
    public class FileRemovingEvent : FileRenamingEvent
    {
        public FileRemovingEvent(string sourceFile, string targetFile, bool isDirectory)
            : base(sourceFile, targetFile, isDirectory)
        {}
    }

    /// <summary>
    /// An event fired immediately after a file is removed.
    /// </summary>
    public class FileRemovedEvent : FileRenameEvent
    {
        public FileRemovedEvent(string sourceFile, string targetFile, bool isDirectory)
            : base(sourceFile, targetFile, isDirectory)
        {}
    }

    /// <summary>
    /// An event fired immediately before a file is copied.
    /// </summary>
    public class FileCopyingEvent : FileRenamingEvent
    {
        public FileCopyingEvent(string sourceFile, string targetFile, bool isDirectory)
            : base(sourceFile, targetFile, isDirectory)
        {}
    }

    /// <summary>
    /// An event fired immediately after a file is copied.
    /// </summary>
    public class FileCopiedEvent : FileRenameEvent
    {
        public FileCopiedEvent(string sourceFile, string targetFile, bool isDirectory)
            : base(sourceFile, targetFile, isDirectory)
        {}
    }
}
