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
        private MVVMCNavigationService _navigationService;
        private Type _thisType;
        private MethodInfo[] _methods;
        private Dispatcher _dispatcher;

        public INavigationService NavigationService { get { return _navigationService; } }

        public string ID { get; internal set; }

        public Controller()
        {
            _thisType = this.GetType();
            _methods = _thisType.GetMethods();
            _navigationService = MVVMC.MVVMCNavigationService.GetInstance();
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void NavigateToInitial(object parameter = null)
        {
            Navigate("Initial", parameter);
        }

        public void Navigate(string action, object parameter)
        {
            var method = _methods.FirstOrDefault(m => m.Name == action);
            if (method == null)
            {
                throw new Exception($"Navigate failed. Can't find method {method.Name} in controller {ID}");
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
            RunOnUIThread(() =>
            {
                var target = _navigationService.CreateViewAndViewModel(ID, pageName);

                var vm = target.DataContext;
                if (vm != null)
                {
                    var mvvmcVM = vm as MVVMCViewModel;
                    mvvmcVM.ViewBag = viewBag;
                    mvvmcVM.NavigationParameter = parameter;
                    mvvmcVM.Initialize();
                }

                ChangeContentInRegion(target);
            });
        }

        private void RunOnUIThread(Action act)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                act();
            else
            {
                _dispatcher.Invoke(act);
            }
        }

        /// <summary>
        /// This is the only method in the infrastructure to actually do the navigation by changing Content of NavigationArea.
        /// The is on purpose since only the controller related to a Navigation Area should be able to change its Content.
        /// </summary>
        private void ChangeContentInRegion(object content)
        {
            var currentThread = Thread.CurrentThread;
            Region navArea = _navigationService.FindRegionByID(ID);
            navArea.Content = content;
        }

        public MVVMCViewModel GetCurrentViewModel()
        {
            return _navigationService.GetCurrentViewModelByControllerID(ID);
        }

        protected string GetCurrentPageName()
        {
            return _navigationService.GetCurrentPageNameByControllerID(ID);
        }

        public abstract void Initial();

    }
}
