using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    public class ShowQuickSearchPanelResult : IResult
    {
        #region IResult メンバー

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public void Execute(ActionExecutionContext context)
        {
            //var panel = new
        }

        #endregion
    }
}
