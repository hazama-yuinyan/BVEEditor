using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    public class ResultFactory : IResultFactory
    {
        SimpleContainer container;

        public ResultFactory(SimpleContainer container)
        {
            this.container = container;
        }

        #region IResultFactory メンバー

        public ShowDialogResult<TModel> ShowDialogResult<TModel>() where TModel : Workbench.ShellPresentationViewModel
        {
            return (ShowDialogResult<TModel>)container.GetInstance(typeof(ShowDialogResult<TModel>), null);
        }

        public CloseResult Close()
        {
            return (CloseResult)container.GetInstance(typeof(CloseResult), null);
        }

        public FileDialogResult ShowFileDialog(string title, string filter, FileDialogMode mode)
        {
            return ShowFileDialog(title, filter, mode, null);
        }

        public FileDialogResult ShowFileDialog(string title, string filter, FileDialogMode mode, string fileName)
        {
            return new FileDialogResult(title, filter, mode, fileName);
        }

        public MessageBoxResult ShowMessageBox(string caption, string text, System.Windows.MessageBoxButton buttons)
        {
            return new MessageBoxResult(caption, text, buttons);
        }

        public IResult Cancel(System.Action callback)
        {
            return new CancelResult(callback);
        }

        #endregion
    }
}
