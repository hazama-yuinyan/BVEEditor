using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BVEEditor.Result
{
    public enum FileDialogMode
    {
        Open = 1,
        Save = 2
    }

    public class FileDialogResult : Result
    {
        readonly string title;
        readonly string filter;
        readonly string file_name;

        public FileDialogMode Mode{get; private set;}
        public string File{get; private set;}

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
            dialog.Title = title;
            dialog.Filter = filter;

            dialog.ShowDialog();
            File = dialog.FileName;

            base.Execute(context);
        }
    }
}
