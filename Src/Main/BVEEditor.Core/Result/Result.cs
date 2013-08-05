using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    /// <summary>
    /// Abstract implementation of IResult.
    /// </summary>
    public abstract class Result : IResult
    {
        public virtual void Execute(ActionExecutionContext context)
        {
            OnCompleted(this, new ResultCompletionEventArgs());
        }

        protected virtual void OnCompleted(object sender, ResultCompletionEventArgs args)
        {
            if(Completed != null)
                Completed(sender, args);
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
