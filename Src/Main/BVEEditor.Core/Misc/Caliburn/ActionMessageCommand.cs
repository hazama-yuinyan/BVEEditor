using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;

namespace BVEEditor.Misc.Caliburn
{
    /// <summary>
    /// An <see cref="System.Windows.Input.ICommand"/> implementation that invokes actions using <see cref="Caliburn.Micro.ActionMessage"/>.
    /// </summary>
    public class ActionMessageCommand : ActionMessage, ICommand
    {
        static ActionMessageCommand()
        {
            ActionMessage.EnforceGuardsDuringInvocation = true;
        }

        #region ICommand メンバー

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Invoke(parameter);
        }

        #endregion
    }
}
