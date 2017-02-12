using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MVVMC
{
    internal class MVVMCNavigationService : INavigationService, INavigationExecutor
    {
        #region SINGLETON
        private static MVVMCNavigationService _instance;
        private static object _lockInstance = new object();
        public static MVVMCNavigationService GetInstance()
        {
            if (_instance != null)
                return _instance;
            lock (_lockInstance)
            {
                if (_instance != null)
                    return _instance;
                _instance = new MVVMCNavigationService();
                return _instance;
            }
        }
        #endregion SINGLETON

        List<Region> _regions = new List<Region>();
        List<Controller> _controllers = new List<Controller>();
        private string _pagesNamespace;
        private Type[] _typesInPages;
        private Dispatcher _dispatcher;

        public void Initialize(string pagesNamespace)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            _pagesNamespace = pagesNamespace;
            _typesInPages = assembly.GetTypes().Where(type => type.Namespace.ToLower().StartsWith(_pagesNamespace.ToLower())).ToArray();
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void AddRegion(Region navArea)
        {
            _regions.Add(navArea);
        }

        internal void RemoveRegion(Region navigationArea)
        {
            _regions.Remove(navigationArea);
        }

        internal void CreateAndAddController(string controllerID)
        {
            var type = _typesInPages.First(elem => elem.Name.Equals(controllerID + "Controller", StringComparison.CurrentCultureIgnoreCase));
            var instance = Activator.CreateInstance(type);
            var controller = instance as Controller;
            controller.ID = controllerID;
            controller.NavigationService = this;
            controller.NavigationExecutor = this;
            _controllers.Add(controller);
        }

        internal void RemoveController(string controllerID)
        {
            var c = _controllers.First(elem => elem.ID == controllerID);
            _controllers.Remove(c);
        }

        public MVVMCViewModel GetCurrentViewModelByControllerID(string controllerID)
        {
            var view = GetViewByControllerID(controllerID);
            if (view == null)
                return null;
            var fe = view as FrameworkElement;
            var vm = fe?.DataContext;
            if (vm == null)
                return null;
            return vm as MVVMCViewModel;
        }

        public string GetCurrentPageNameByControllerID(string controllerID)
        {
            var view = GetViewByControllerID(controllerID);
            if (view == null)
                return null;
            var name = view.GetType().Name.Replace("View", "").Replace("view", "");
            return name;
        }

        private object GetViewByControllerID(string ID)
        {
            var navArea = FindRegionByID(ID);
            return navArea?.Content;
        }

        public void NavigateWithController<TViewModel>(object parameter) where TViewModel : MVVMCViewModel
        {
            var ns = typeof(TViewModel).Namespace;
            var name = typeof(TViewModel).Name.Replace("ViewModel", "").Replace("View", "");
            var controllerID = ns.Split('.').Last();
            var controller = GetController(controllerID);
            controller.Navigate(name, parameter);
        }

        public Controller GetController(string controllerID)
        {
            return _controllers.First(elem => elem.ID.Equals(controllerID, StringComparison.CurrentCultureIgnoreCase));
        }

        public TControllerType GetController<TControllerType>() where TControllerType : Controller
        {
            return _controllers.First(elem => elem is TControllerType) as TControllerType;
        }

        internal Region FindRegionByID(string ID)
        {
            return _regions.First(elem => elem.ControllerID == ID);
        }

        /// <summary>
        /// Creates an instance of View and ViewModel and sets View.DataContext to ViewModel
        /// </summary>
        /// <param name="controllerID"></param>
        /// <param name="pageName"></param>
        /// <returns></returns>
        internal FrameworkElement CreateViewAndViewModel(string controllerID, string pageName)
        {
            var view = CreateViewInstance(controllerID, pageName);
            var viewModel = CreateViewModelInstance(controllerID, pageName);
            view.DataContext = viewModel;
            return view;
        }

        internal MVVMCViewModel CreateViewModelInstance(string controllerID, string pageName)
        {
            var type = _typesInPages.FirstOrDefault(
                elem => elem.Namespace.Equals(_pagesNamespace + "." + controllerID, StringComparison.CurrentCultureIgnoreCase)
                && elem.Name.Equals(pageName + "ViewModel", StringComparison.CurrentCultureIgnoreCase));
            if (type == null)
                return null;

            var instance = Activator.CreateInstance(type);

            var controller = GetController(controllerID);
            var vm = instance as MVVMCViewModel;
            vm.SetController(controller);
            return vm;
        }

        private FrameworkElement CreateViewInstance(string controllerName, string pageName)
        {
            var type = _typesInPages.First(elem => elem.Namespace.Equals(_pagesNamespace + "." + controllerName, StringComparison.CurrentCultureIgnoreCase )
            && elem.Name.Equals(pageName + "View", StringComparison.CurrentCultureIgnoreCase));
            var instance = Activator.CreateInstance(type);
            return instance as FrameworkElement;
        }

        public void ExecuteNavigation(string controllerID, string pageName, object parameter, Dictionary<string, object> viewBag = null)
        {
            RunOnUIThread(() =>
            {
                var target = CreateViewAndViewModel(controllerID, pageName);

                var vm = target.DataContext;
                if (vm != null)
                {
                    var mvvmcVM = vm as MVVMCViewModel;
                    mvvmcVM.ViewBag = viewBag;
                    mvvmcVM.NavigationParameter = parameter;
                    mvvmcVM.Initialize();
                }

                ChangeContentInRegion(target, controllerID);
            });
        }

        private void ChangeContentInRegion(object content, string controllerID)
        {
            var currentThread = Thread.CurrentThread;
            Region navArea = FindRegionByID(controllerID);
            navArea.Content = content;
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


    }
}
