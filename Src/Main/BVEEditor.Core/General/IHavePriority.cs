using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor
{
    /// <summary>
    /// Denotes the derived classes that they can be ordered in their priorities.
    /// </summary>
    public interface IHavePriority : IComparable<IHavePriority>
    {
        double Priority{get;}
    }
}
