using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Logging;
using Caliburn.Micro;

namespace BVEEditor.Options
{
    /// <summary>
    /// Represents the category name node. It is used to show the first child item when the user clicks the category name
    /// in the tree view. It does nothing in functionality.
    /// </summary>
    public class OptionCategoryViewModel : OptionPanelViewModel
    {
        static readonly ILog Logger = LogManager.GetLog(typeof(OptionCategoryViewModel));

        public OptionCategoryViewModel() : base(null)
        {
        }

        public override void LoadOptions()
        {
            foreach(var child in Children){
                child.LoadOptions();
                Logger.Info("Loaded options in {0} from disk.", child.Title);
            }
        }

        public override bool SaveOptions()
        {
            foreach(var child in Children){
                if(!child.SaveOptions())
                    return false;
                else
                    Logger.Info("Saved options in {0} to disk.", child.Title);
            }

            return true;
        }
    }
}
