using Caliburn.Micro;
using Ninject;

namespace BVEEditor.Result
{
    /// <summary>
    /// A factory class that creates various <see cref="Caliburn.Micro.IResult"/> implementations.
    /// </summary>
    public class ResultFactory : IResultFactory
    {
        IKernel kernel;

        public ResultFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        #region IResultFactory メンバー

        public ShowDialogResult<TModel> ShowDialogResult<TModel>() where TModel : Workbench.ShellPresentationViewModel
        {
            return kernel.Get<ShowDialogResult<TModel>>();
        }

        public CloseResult Close()
        {
            return kernel.Get<CloseResult>();
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
