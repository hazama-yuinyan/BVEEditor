using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// Creates DisplayBindingDescriptor objects.
    /// Primary display bindings can provide editors for additional file types
    /// (like the ResourceEditor), secondary display bindings can add tabs to
    /// existing display bindings (like the form designer).
    /// </summary>
    /// <attribute name="class" use="required">
    /// Name of the IDisplayBinding class.
    /// </attribute>
    /// <attribute name="title" use="required">
    /// Title of the display binding to use in the "Open With" dialog.
    /// </attribute>
    /// <attribute name="fileNamePattern" use="optional">
    /// Regular expression that specifies the file names for which the display binding
    /// will be used. Example: "\.res(x|ources)$"
    /// </attribute>
    /// <usage>Only in /BVEEditor/Workbench/DisplayBindings</usage>
    /// <returns>
    /// An DisplayBindingDescriptor object that wraps a IDisplayBinding object.
    /// </returns>
    /// <example title="Primary display binding: Resource editor">
    /// &lt;Path name = "/BVEEditor/Workbench/DisplayBindings"&gt;
    ///   &lt;DisplayBinding id    = "ResourceEditor"
    ///                   title = "Resource editor"
    ///                   class = "ResourceEditor.ResourceEditorDisplayBinding"
    ///                   insertbefore    = "Text"
    ///                   fileNamePattern = "\.res(x|ources)$"/&gt;
    /// &lt;/Path&gt;
    /// </example>
    sealed class DisplayBindingDoozer : IDoozer
    {
        /// <summary>
        /// Gets if the doozer handles codon conditions on its own.
        /// If this property return false, the item is excluded when the condition is not met.
        /// </summary>
        public bool HandleConditions{
            get{
                return false;
            }
        }

        /// <summary>
        /// Creates an item with the specified sub items. And the current
        /// Condition status for this item.
        /// </summary>
        public object BuildItem(BuildItemArgs args)
        {
            return new DisplayBindingDescriptor(args.Codon);
        }
    }
}
