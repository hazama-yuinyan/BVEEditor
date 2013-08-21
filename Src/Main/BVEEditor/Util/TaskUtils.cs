using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Util
{
    /// <summary>
    /// Contains some task-related utility methods.
    /// </summary>
    public static class TaskUtils
    {
        public static async void CallLater(TimeSpan delay, System.Action method)
        {
            await Task.Delay(delay).ConfigureAwait(false);
            Execute.BeginOnUIThread(method);
        }
    }
}
