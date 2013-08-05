using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    public class ShowDialogResult<TModel> : Result
    {
        readonly IWindowManager window_manager;
        readonly TModel model;
        Action<TModel> configure;

        public ShowDialogResult(IWindowManager windowManager, TModel model)
        {
            window_manager = windowManager;
            this.model = model;
        }

        public IResult Configure(Action<TModel> configure)
        {
            this.configure = configure;
            return this;
        }

        public override void Execute(ActionExecutionContext context)
        {
            if(configure != null)
                configure(model);

            window_manager.ShowDialog(model);

            base.Execute(context);
        }
    }
}
