using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    /// <summary>
    /// A dummy implementation of <see cref="Caliburn.Micro.IResult"/>.
    /// </summary>
    public class DummyResult : IResult
    {
        #region IResult メンバー

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public void Execute(ActionExecutionContext context)
        {
        }

        #endregion
    }
}
