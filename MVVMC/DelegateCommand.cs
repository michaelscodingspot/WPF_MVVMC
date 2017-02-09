using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVMC
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> action, Func<T, bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return
                (
                    _canExecute == null ||
                    (typeof(T).IsClass || parameter != null) &&
                    _canExecute((T)parameter)
                );
        }

        public virtual void Execute(object parameter)
        {
            _action((T)parameter);
        }

        
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action action, Func<bool> canExecute = null) :
            base(_ => action(), canExecute == null ? (Func<object, bool>)null : (_ => canExecute()))
        {

        }
    }

    public static class DelegateCommandExtentions
    {
        public static void RaiseCanExecuteChanged<T>(this ICommand command)
        {
            var cmd = command as DelegateCommand<T>;
            if (cmd != null)
            {
                cmd.RaiseCanExecuteChanged();
            }
        }
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            var cmd = command as DelegateCommand;
            if (cmd != null)
            {
                cmd.RaiseCanExecuteChanged();
            }
        }
    }

}
