using BVEEditor.Workbench;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor
{
    public interface ICanReferToWorkbench
    {
        IWorkbench Workbench{
            set;
        }
    }
}
