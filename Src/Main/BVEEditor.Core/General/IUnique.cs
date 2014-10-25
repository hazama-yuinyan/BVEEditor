using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor
{
    /// <summary>
    /// Denotes the derived classes that they are identifiable by the ID.
    /// </summary>
    public interface IUnique
    {
        Guid Id{get;}
    }
}
