using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Markup;

namespace MVVMC
{
    public class GoBackCommand : MarkupExtension, ICommand
    {
        public event EventHandler CanExecuteChanged;
        Lazy<MVVMCNavigationService> _navigationService = new Lazy<MVVMCNavigationService>(() => MVVMCNavigationService.GetInstance());

        public HistoricalNavigationMode HistoricalNavigationMode { get; set; } = HistoricalNavigationMode.UseCommandParameter;
        public string ControllerID { get; set; }
        public Dictionary<string, object> ViewBag { get; set; }

        private bool _canExecute = false;

        public GoBackCommand()
        {
            _navigationService.Value.AddGoBackCommand(this);
        }

        public void ChangeCanExecute()
        {
            var newValue = _navigationService.Value.GetController(ControllerID).CanGoBack;
            if (newValue != _canExecute)
            {
                _canExecute = newValue;
                CanExecuteChanged?.Invoke(this, null);
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            if (HistoricalNavigationMode == HistoricalNavigationMode.UseCommandParameter)
            {
                _navigationService.Value.GetController(ControllerID).GoBack(parameter, ViewBag);
            }
            else
            {
                _navigationService.Value.GetController(ControllerID).GoBack();
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
