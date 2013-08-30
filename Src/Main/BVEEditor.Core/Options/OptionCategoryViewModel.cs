using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Logging;

namespace BVEEditor.Options
{
    /// <summary>
    /// Represents the category name node. It is used to show the first child item when the user clicks the category name
    /// in the tree view. It does nothing in functionality.
    /// </summary>
    public class OptionCategoryViewModel : OptionPanelViewModel
    {
        public OptionCategoryViewModel() : base(null)
        {}

        public override void LoadOptions()
        {
            foreach(var child in Children){
                child.LoadOptions();
                Log4netLogger.Instance.Info("Loaded options in " + child.Title + " from disk");
            }
        }

        public override bool SaveOptions()
        {
            foreach(var child in Children){
                if(!child.SaveOptions())
                    return false;
                else
                    Log4netLogger.Instance.Info("Saved options in " + child.Title + " to disk");
            }

            return true;
        }
    }
}
