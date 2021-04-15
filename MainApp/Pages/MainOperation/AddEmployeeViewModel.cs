using MainApp.Pages.AddWizard;
using MVVMC;
using System.Windows.Input;

namespace MainApp.Pages.MainOperation
{
    public class AddEmployeeViewModel : MVVMCViewModel
    {
        public string _nextCaption = "Next";
        public string NextCaption
        {
            get { return _nextCaption; }
            set
            {
                _nextCaption = value;
                OnPropertyChanged();
            }
        }

        public ICommand _nextCommand;
        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                    _nextCommand = new DelegateCommand(() =>
                    {
                        INavigationService navigationService = NavigationServiceProvider.GetNavigationServiceInstance();
                        AddWizardController controller = navigationService.GetController<AddWizard.AddWizardController>();
                        controller.Next();
                        RefreshButtons();
                    },
                    () =>
                    {
                        return true;
                    });
                return _nextCommand;
            }
        }

        private void RefreshButtons()
        {
            NextCaption = "Next";
            var vm = NavigationServiceProvider.GetNavigationServiceInstance().GetController<AddWizardController>().GetCurrentViewModel();
            if (vm is AddWizard.ConfirmViewModel)
            {
                NextCaption = "Add Employee";
            }
        }
    }
}
