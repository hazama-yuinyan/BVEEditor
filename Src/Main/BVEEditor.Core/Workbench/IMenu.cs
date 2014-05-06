using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// Interface for defining a menu.
    /// </summary>
    public interface IMenu : IParent<IRootMenu>, IUnique, IChild, ICanReferToWorkbench, IActivate, IDeactivate
    {
        /// <summary>
        /// Gets the collection of the child menu items.
        /// </summary>
        IList<IRootMenu> Items{get;}
    }

    /// <summary>
    /// Interface for describing a root menu item.
    /// </summary>
    public interface IRootMenu : IChild, IUnique, ICanReferToWorkbench, IActivate, IDeactivate
    {
        /// <summary>
        /// The assembly name in which the view for this menu resides.
        /// </summary>
        string ReferenceAssemblyName{
            get;
        }

        /// <summary>
        /// The name of this menu.
        /// It is only used for logging and retrieving the corresponding data template.
        /// </summary>
        string MenuName{
            get;
        }
    }
}
