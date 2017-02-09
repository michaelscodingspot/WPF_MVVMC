using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMC
{
    public class NavigationServiceProvider
    {
        public static INavigationService GetNavigationServiceInstance()
        {
            return MVVMCNavigationService.GetInstance();
        }
    }
}
