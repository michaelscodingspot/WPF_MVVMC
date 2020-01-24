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
        bool IsControllerExists<TControllerType>() where TControllerType : Controller;
        string GetControllerId<TControllerType>() where TControllerType : Controller;
        event NavigationOccuredEventArgs NavigationOccured;


        MVVMCViewModel GetCurrentViewModelByControllerID(string controllerID);

        string GetCurrentPageNameByControllerID(string controllerID);

        event Action<string> CanGoBackChangedEvent;
        event Action<string> CanGoForwardChangedEvent;

    }
}
