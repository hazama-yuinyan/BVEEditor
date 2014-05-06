using BVEEditor.Help;
using BVEEditor.Result;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Workbench
{
    public class HelpMenuViewModel : MenuViewModelBase
    {
        readonly IResultFactory result_factory;

        public HelpMenuViewModel(IResultFactory resultFactory) : base(LogManager.GetLog(typeof(HelpMenuViewModel)), "Help")
        {
            result_factory = resultFactory;
        }

        public IEnumerable<IResult> ShowAboutDialog()
        {
            yield return result_factory.ShowDialogResult<AboutDialogViewModel>();
        }
    }
}
