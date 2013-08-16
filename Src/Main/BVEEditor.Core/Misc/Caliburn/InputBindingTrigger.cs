using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using Caliburn.Micro;

namespace BVEEditor.Misc.Caliburn
{
    /// <summary>
    /// Represents an <see cref="System.Windows.Input.InputBinding"/> that can be triggered locally or globally.
    /// </summary>
    public class InputBindingTrigger : TriggerBase<FrameworkElement>, ICommand
    {
        public static readonly DependencyProperty LocalInputBindingsProperty =
            DependencyProperty.Register("LocalInputBindings", typeof(BindableCollection<InputBinding>), typeof(InputBindingTrigger),
            new PropertyMetadata(default(BindableCollection<InputBinding>)));

        public static readonly DependencyProperty GlobalInputBindingsProperty =
            DependencyProperty.Register("GlobalInputBindings", typeof(BindableCollection<InputBinding>), typeof(InputBindingTrigger),
            new UIPropertyMetadata(null));

        public BindableCollection<InputBinding> LocalInputBindings{
            get{return (BindableCollection<InputBinding>)GetValue(LocalInputBindingsProperty);}
            set{SetValue(LocalInputBindingsProperty, value);}
        }

        public BindableCollection<InputBinding> GlobalInputBindings{
            get{return (BindableCollection<InputBinding>)GetValue(GlobalInputBindingsProperty);}
            set{SetValue(GlobalInputBindingsProperty, value);}
        }

        public InputBindingTrigger()
        {
            GlobalInputBindings = new BindableCollection<InputBinding>();
            LocalInputBindings = new BindableCollection<InputBinding>();
        }

        protected override void OnAttached()
        {
            foreach(var binding in GlobalInputBindings.Union(LocalInputBindings))
                binding.Command = this;

            AssociatedObject.Loaded += delegate{
                var window = GetWindow(AssociatedObject);

                foreach(var binding in GlobalInputBindings)
                    window.InputBindings.Add(binding);

                foreach(var binding in LocalInputBindings)
                    AssociatedObject.InputBindings.Add(binding);
            };

            base.OnAttached();
        }

        static Window GetWindow(FrameworkElement element)
        {
            var window = element as Window;
            if(window != null)
                return window;

            var parent = element.Parent as FrameworkElement;
            return GetWindow(parent);
        }

        #region ICommand メンバー

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            InvokeActions(parameter);
        }

        #endregion
    }
}
