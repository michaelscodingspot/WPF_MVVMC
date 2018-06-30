using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        public bool _canExecute = false;

        //private Controller _controller;

        public GoBackCommand()
        {
            if (_navigationService.Value.IsControllerExists(ControllerID))
            {
                //Do nothing
                var controller = _navigationService.Value.GetController(ControllerID);
                _canExecute = controller.CanGoBack;
                controller.CanGoBackChanged += () => CanExecuteChanged?.Invoke(this, null);
            }
            else
            {
                Action<string> handler = null;
                handler = (controllerID) =>
                {

                    if (ControllerID.Equals(controllerID, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var controller = _navigationService.Value.GetController(ControllerID);
                        _canExecute = controller.CanGoBack;
                        CanExecuteChanged?.Invoke(this, null);
                        _navigationService.Value.ControllerCreated -= handler;
                        controller.CanGoBackChanged += () =>
                        {
                            _canExecute = controller.CanGoBack;
                            CanExecuteChanged?.Invoke(this, null);
                        };

                    }
                };
                _navigationService.Value.ControllerCreated += handler;

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
