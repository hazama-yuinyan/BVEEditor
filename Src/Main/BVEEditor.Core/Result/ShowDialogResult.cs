using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace BVEEditor.Result
{
    /// <summary>
    /// An IResult implementation that shows a dialog using <typeparamref name="TModel"/> class as its view model.
    /// </summary>
    /// <typeparam name="TModel">The view model class that takes control of the view.</typeparam>
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

        /// <summary>
        /// Sets the configure delegate.
        /// Usually the configure delegate is the only place that you can initialize the view model,
        /// because view models are preferred to be constructed by an IoC container and in that case, you can't call
        /// the constructor yourself.
        /// </summary>
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
