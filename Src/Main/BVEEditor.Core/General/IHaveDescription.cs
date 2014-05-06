using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor
{
    /// <summary>
    /// Denotes the derived classes that they have additional information on their usage, features, etc.
    /// Usually used to show a tooltip.
    /// </summary>
    public interface IHaveDescription
    {
        string Description{get;}
    }
}
