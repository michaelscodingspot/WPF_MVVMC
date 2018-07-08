using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVVMC
{
    public interface INavigationService
    {
        void NavigateWithController<TViewModel>(object parameter = null) where TViewModel : MVVMCViewModel;

        Controller GetController(string controllerID);
        TControllerType GetController<TControllerType>() where TControllerType : Controller;
        bool IsControllerExists(string controllerID);

        MVVMCViewModel GetCurrentViewModelByControllerID(string controllerID);

        string GetCurrentPageNameByControllerID(string controllerID);

        void CanGoBackChanged(string controllerId);
        void CanGoForwardChanged(string controllerId);

        event Action<string> CanGoBackChangedEvent;
        event Action<string> CanGoForwardChangedEvent;

    }
}
