using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace BVEEditor
{
    /// <summary>
    /// A command that invokes a delegate.
    /// The command parameter must be of type T.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        readonly Predicate<T> can_execute;
        readonly Action<T> execute;

        public RelayCommand(Action<T> execute)
        {
            if(execute == null)
                throw new ArgumentNullException("execute");
            
            this.execute = execute;
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if(execute == null)
                throw new ArgumentNullException("execute");
            
            this.execute = execute;
            this.can_execute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add{
                if(can_execute != null)
                    CommandManager.RequerySuggested += value;
            }
            
            remove{
                if(can_execute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if(parameter != null && !(parameter is T))
                return false;
            
            return can_execute == null ? true : can_execute((T)parameter);
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }
    }

    /// <summary>
    /// A command that invokes a delegate.
    /// This class does not provide the command parameter to the delegate -
    /// if you need that, use the generic version of this class instead.
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly Func<bool> can_execute;
        readonly Action execute;

        public RelayCommand(Action execute)
        {
            if(execute == null)
                throw new ArgumentNullException("execute");
            
            this.execute = execute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if(execute == null)
                throw new ArgumentNullException("execute");
            
            this.execute = execute;
            this.can_execute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add{
                if(can_execute != null)
                    CommandManager.RequerySuggested += value;
            }

            remove{
                if(can_execute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return can_execute == null ? true : can_execute();
        }

        public void Execute(object parameter)
        {
            execute();
        }
    }
}
