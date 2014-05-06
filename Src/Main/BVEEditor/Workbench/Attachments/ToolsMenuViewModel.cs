using BVEEditor.Options;
using BVEEditor.Result;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// The view model for Tools menu.
    /// </summary>
    public class ToolsMenuViewModel : MenuViewModelBase
    {
        readonly IResultFactory result_factory;

        public ToolsMenuViewModel(IResultFactory resultFactory) : base(LogManager.GetLog(typeof(ToolsMenuViewModel)), "Tools")
        {
            result_factory = resultFactory;
        }

        public IEnumerable<IResult> ShowOptionsDialog()
        {
            yield return result_factory.ShowDialogResult<OptionsViewModel>();
        }
    }
}
