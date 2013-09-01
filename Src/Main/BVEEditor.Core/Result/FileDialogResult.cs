using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;
using Microsoft.Win32;

namespace BVEEditor.Result
{
    public enum FileDialogMode
    {
        Open = 1,
        Save = 2
    }

    /// <summary>
    /// An IResult implementation that shows a file-related dialog.
    /// </summary>
    public class FileDialogResult : Result
    {
        readonly string title;
        readonly string filter;
        readonly string file_name;

        public FileDialogMode Mode{get; private set;}
        public string File{get; private set;}

        /// <summary>
        /// Constructs a new <see cref="BVEEditor.Result.FileDialogResult"/> instance.
        /// Note that <code>title</code> and <code>filter</code> will be put into the <see cref="ICSharpCode.Core.StringParser.Parse"/>
        /// method.
        /// </summary>
        public FileDialogResult(string title, string filter, FileDialogMode mode, string fileName)
        {
            Mode = mode;
            this.title = title;
            this.filter = filter;
            file_name = fileName;
        }

        public override void Execute(Caliburn.Micro.ActionExecutionContext context)
        {
            var dialog = (Mode == FileDialogMode.Open) ? new OpenFileDialog() as FileDialog : new SaveFileDialog();
            dialog.FileName = file_name;
            dialog.Title = StringParser.Parse(title);
            dialog.Filter = StringParser.Parse(filter);

            if(dialog.ShowDialog().GetValueOrDefault())
                File = dialog.FileName;

            base.Execute(context);
        }
    }
}
