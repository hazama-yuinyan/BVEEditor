using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Options
{
    /// <summary>
    /// Creates DefaultOptionPanelDescriptor objects that are used in option dialogs.
    /// </summary>
    /// <attribute name="class">
    /// Name of the OptionPanelViewModel class. Optional if the page has subpages.
    /// </attribute>
    /// <attribute name="label" use="required">
    /// Caption of the dialog panel. Note that it will be put into <see cref="ICSharpCode.StringParser.Parse"/> method.
    /// </attribute>
    /// <children childTypes="OptionPanel">
    /// In the BVEEditor options, option pages can have subpages by specifying them
    /// as children in the AddInTree.
    /// </children>
    /// <usage>In /BVEEditor/BackendBindings/ProjectOptions and /BVEEditor/Dialogs/OptionsDialog</usage>
    /// <returns>
    /// A DefaultOptionPanelDescriptor object.
    /// </returns>
    public class OptionPanelDoozer : IDoozer
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
            string label = args.Codon["label"];
            string id = args.Codon.Id;

            var sub_items = args.BuildSubItems<IOptionPanelDescriptor>();
            if(sub_items.Count == 0){
                if(args.Codon.Properties.Contains("class"))
                    return new DefaultOptionPanelDescriptor(id, label, args.AddIn, args.Parameter, args.Codon["class"]);
                else
                    return new DefaultOptionPanelDescriptor(id, label, args.AddIn);
            }

            return new DefaultOptionPanelDescriptor(id, label, args.AddIn, sub_items);
        }
    }
}
