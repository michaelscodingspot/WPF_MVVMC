using MainApp.Pages.AddWizard;
using MVVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string _prevCaption = "Cancel";
        public string PrevCaption
        {
            get { return _prevCaption; }
            set
            {
                _prevCaption = value;
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

        public ICommand _prevCommand;
        public ICommand PrevCommand
        {
            get
            {
                if (_prevCommand == null)
                    _prevCommand = new DelegateCommand(() =>
                    {
                        NavigationServiceProvider.GetNavigationServiceInstance().GetController<AddWizard.AddWizardController>().Prev();
                        RefreshButtons();
                    },
                    () =>
                    {
                        return true;
                    });
                return _prevCommand;
            }
        }

        private void RefreshButtons()
        {
            NextCaption = "Next";
            PrevCaption = "Back";
            var vm = GetController().GetCurrentViewModel();
            if (vm is AddWizard.InitialViewModel)
                PrevCaption = "Cancel";
            else if (vm is AddWizard.ConfirmViewModel)
                NextCaption = "Add Employee";
        }
    }
}
