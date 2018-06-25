using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MVVMC
{
    public abstract class Controller : IController
    {
        private Type _thisType;
        private MethodInfo[] _methods;

        public INavigationService NavigationService { get; set; }
        public INavigationExecutor NavigationExecutor { get; set; }

        public string ID { get; internal set; }

        public Controller()
        {
            _thisType = this.GetType();
            _methods = _thisType.GetMethods();
        }

        public void NavigateToInitial()
        {
            Navigate("Initial", null);
        }

        public void Navigate(string action, object parameter)
        {
            var method = _methods.FirstOrDefault(m => m.Name == action);
            if (method == null)
            {
                throw new Exception($"Navigate failed. Can't find method '{action}' in controller '{ID}'");
            }
            else
            {
                var prms = method.GetParameters();
                if (prms.Length == 0)
                    method.Invoke(this, new object[] { });
                else
                    method.Invoke(this, new object[] { parameter });
            }
        }

        protected void ExecuteNavigation([CallerMemberName]string pageName = null)
        {
            ExecuteNavigationInternal(pageName, null, null);
        }

        protected void ExecuteNavigation(object parameter, Dictionary<string, object> viewBag, [CallerMemberName]string pageName = null)
        {
            ExecuteNavigationInternal(pageName, parameter, viewBag);
        }

        private void ExecuteNavigationInternal(string pageName, object parameter, Dictionary<string, object> viewBag = null)
        {
            NavigationExecutor.ExecuteNavigation(ID, pageName, parameter, viewBag);
        }
        
        public MVVMCViewModel GetCurrentViewModel()
        {
            return NavigationService.GetCurrentViewModelByControllerID(ID);
        }

        protected string GetCurrentPageName()
        {
            return NavigationService.GetCurrentPageNameByControllerID(ID);
        }

        public abstract void Initial();

    }
}
