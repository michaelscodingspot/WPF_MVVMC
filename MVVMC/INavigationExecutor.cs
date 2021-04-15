using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMC
{
    public interface INavigationExecutor
    {
        MVVMCViewModel ExecuteNavigation(string controllerID, string pageName, object parameter, MVVMCViewModel viewModel, NavigationMode navigationMode, Dictionary<string, object> viewBag = null);
    }
}
