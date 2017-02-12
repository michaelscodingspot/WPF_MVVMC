using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace MVVMC
{
    public class NavigateCommand : MarkupExtension, ICommand
    {
        public event EventHandler CanExecuteChanged;
        Lazy<MVVMCNavigationService> _navigationService = new Lazy<MVVMCNavigationService>(() => MVVMCNavigationService.GetInstance());

        public string ControllerID { get; set; }
        public string Action { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (Action == null)
                _navigationService.Value.GetController(ControllerID).NavigateToInitial();
            else
                _navigationService.Value.GetController(ControllerID).Navigate(Action, parameter);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
