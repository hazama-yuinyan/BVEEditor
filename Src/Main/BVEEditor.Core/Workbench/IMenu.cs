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
    /// Interface for describing a menu item.
    /// </summary>
    public interface IMenu : IParent<IMenu>, IUnique, IChild, ICanReferToWorkbench, IActivate, IDeactivate
    {
        /// <summary>
        /// Gets the collection of child menu items that will be inserted before other static items.
        /// </summary>
        BindableCollection<IMenu> ItemsBefore{get;}

        /// <summary>
        /// Gets the collection of child menu items that will be appended.
        /// </summary>
        BindableCollection<IMenu> ItemsAfter{get;}
        
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
