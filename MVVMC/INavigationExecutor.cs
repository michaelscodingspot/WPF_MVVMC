using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMC
{
    public interface INavigationExecutor
    {
        void ExecuteNavigation(string controllerID, string pageName, object parameter, Dictionary<string, object> viewBag = null);
    }
}
