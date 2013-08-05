using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BVEEditor.Workbench;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    public interface IResultFactory
    {
        ShowDialogResult<TModel> ShowDialogResult<TModel>() where TModel : ShellPresentationViewModel;
        CloseResult Close();
        FileDialogResult ShowFileDialog(string title, string filter, FileDialogMode mode);
        FileDialogResult ShowFileDialog(string title, string filter, FileDialogMode mode, string fileName);
        MessageBoxResult ShowMessageBox(string caption, string text, MessageBoxButton buttons);
        IResult Cancel(System.Action callback);
    }
}
