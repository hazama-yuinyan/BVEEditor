using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Result;
using Caliburn.Micro;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// The view model for dialogs or windows.
    /// </summary>
    public abstract class ShellPresentationViewModel : Screen
    {
        public IResultFactory Result{get; private set;}

        public ShellPresentationViewModel(IResultFactory resultFactory)
        {
            Result = resultFactory;
        }

        public override void CanClose(Action<bool> callback)
        {
            Coroutine.BeginExecute(CanClose().GetEnumerator(), null, (s, e) => callback(!e.WasCancelled));
        }

        protected virtual IEnumerable<IResult> CanClose()
        {
            yield break;
        }
    }
}
