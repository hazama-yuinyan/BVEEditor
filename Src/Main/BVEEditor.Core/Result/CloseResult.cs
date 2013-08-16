using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    /// <summary>
    /// An IResult implementation that closes a window.
    /// </summary>
    public class CloseResult : Result
    {
        public override void Execute(ActionExecutionContext context)
        {
            var window = Window.GetWindow(context.View);
            window.Close();

            base.Execute(context);
        }
    }
}
