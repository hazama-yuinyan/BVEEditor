using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock;

namespace BVEEditor.AvalonDock
{
    /// <summary>
    /// Denotes the derived type that it can provide a DockingManager instance.
    /// </summary>
    internal interface IDockingManagerProvider
    {
        DockingManager DockingManager{get;}
    }
}
