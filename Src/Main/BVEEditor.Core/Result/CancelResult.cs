using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    public class CancelResult : Result
    {
        readonly System.Action callback;

        public CancelResult(System.Action cancelCallback)
        {
            callback = cancelCallback;
        }

        public override void Execute(ActionExecutionContext context)
        {
            if(callback != null)
                callback();

            OnCompleted(this, new ResultCompletionEventArgs{WasCancelled = true});
        }
    }
}
