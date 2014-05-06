using BVEEditor.Misc.Caliburn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor
{
    /// <summary>
    /// Denotes the derived classes that they can register <see cref="System.Windows.Input.ICommand"/>s that
    /// are invoked in response to input binding triggers.
    /// </summary>
    public interface IHaveInputBindingCommands
    {
        IEnumerable<InputBindingTrigger> GetInputBindingCommands();
    }
}
