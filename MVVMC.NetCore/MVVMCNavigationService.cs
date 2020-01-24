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
    public delegate void NavigationOccuredEventArgs(string controllerId, string previousPage, string newPage);

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

        private List<Type> _controllerTypes;
        private List<Type> _viewModelTypes;
        private List<Type> _viewTypes;

        List<Region> _regions = new List<Region>();
        List<Controller> _controllers = new List<Controller>();
        private Dispatcher _dispatcher;

        internal event Action<string> ControllerCreated;
        public event Action<string> CanGoBackChangedEvent;
        public event Action<string> CanGoForwardChangedEvent;

        public event NavigationOccuredEventArgs NavigationOccured;


        private List<WeakReference<GoBackCommand>> _goBackCommands = new List<WeakReference<GoBackCommand>>();
        private List<WeakReference<GoForwardCommand>> _goForwardCommands = new List<WeakReference<GoForwardCommand>>();


        private MVVMCNavigationService()
        {
            Initialize();
        }

        private void Initialize()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            var assemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetLoadableTypes).ToList();
            _controllerTypes = assemblyTypes.Where(t => t.BaseType?.FullName == "MVVMC.Controller").ToList();

            _viewModelTypes = assemblyTypes
                .Where(t => HasBaseType(t, "MVVMC.MVVMCViewModel"))
                .ToList();
            var controllerNamespaces = _controllerTypes.Select(vm => vm.Namespace);
            _viewTypes = assemblyTypes.Where(t =>
                controllerNamespaces.Contains(t.Namespace) &&
                t.Name.EndsWith("View", StringComparison.InvariantCultureIgnoreCase)).ToList();

            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        private bool HasBaseType(Type type, string baseTypeStr)
        {
            int count = 0;
            while (type.BaseType != null && count++ < 10)
            {
                if (type.BaseType != null && type.BaseType?.FullName != null &&
                    type.BaseType.FullName.StartsWith(baseTypeStr))
                    return true;
                type = type.BaseType;
                
            }
            return false;
        }

        /// <summary>
        /// From https://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
        /// </summary>
        private IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        private string GetControllerName(Type controllerType)
        {
            if (!controllerType.Name.EndsWith("Controller"))
                throw new Exception($"Please change the name of '{controllerType.Name}'. All controllers must end with 'Controller' postfix.");
            return controllerType.Name.Substring(0, controllerType.Name.Length - ("Controller".Length));
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
            Type type = GetControllerTypeById(controllerID);
            var instance = Activator.CreateInstance(type);
            var controller = instance as Controller;
            controller.ID = controllerID;
            controller.SetNavigationService(this);
            controller.NavigationExecutor = this;
            _controllers.Add(controller);
            ControllerCreated?.Invoke(controllerID);
        }

        private Type GetControllerTypeById(string controllerID)
        {
            return _controllerTypes.First(c =>
                GetControllerName(c).Equals(controllerID, StringComparison.CurrentCultureIgnoreCase));
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

        public bool IsControllerExists(string controllerID)
        {
            return _controllers.Any(elem => elem.ID.Equals(controllerID, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsControllerExists<TControllerType>() where TControllerType : Controller
        {
            return IsControllerExists(GetControllerId<TControllerType>());
        }

        public string GetControllerId<TControllerType>() where TControllerType : Controller
        {
            string controllerIdWithPostfix = typeof(TControllerType).Name;
            if (controllerIdWithPostfix.EndsWith("Controller"))
                return controllerIdWithPostfix.Substring(0, controllerIdWithPostfix.Length - ("Controller".Length));
            throw new InvalidOperationException("Controller classes must end with 'Controller' postfix");
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
            var controllerNamespace = GetControllerTypeById(controllerID).Namespace;
            var type = _viewModelTypes.FirstOrDefault(vm =>
                vm.Namespace == controllerNamespace 
                && vm.Name.Equals(pageName + "ViewModel", StringComparison.CurrentCultureIgnoreCase));

            if (type == null)
                return null;

            var instance = Activator.CreateInstance(type);

            var controller = GetController(controllerID);
            var viewModel = instance as MVVMCViewModel;
            viewModel.SetController(controller);
            return viewModel;
        }

        private FrameworkElement CreateViewInstance(string controllerName, string pageName)
        {
            var controllerNamespace = GetControllerTypeById(controllerName).Namespace;
            var type = _viewTypes.FirstOrDefault(vm =>
                vm.Namespace == controllerNamespace
                && vm.Name.Equals(pageName + "View", StringComparison.CurrentCultureIgnoreCase));
            if (type == null)
            {
                throw new Exception($"Navigation failed! Can't find a class {pageName + "View"} in namespace {controllerNamespace}. A UserControl/UIElement by that name should exist.");
            }
            var instance = Activator.CreateInstance(type);
            return instance as FrameworkElement;
        }

        public void ExecuteNavigation(string controllerId, string pageName, object parameter, NavigationMode navigationMode, Dictionary<string, object> viewBag = null)
        {
            RunOnUIThread(() =>
            {
                var prevPage = GetController(controllerId).GetCurrentPageName();

                var target = CreateViewAndViewModel(controllerId, pageName);

                var vm = target.DataContext;
                if (vm != null)
                {
                    var mvvmcVM = vm as MVVMCViewModel;
                    mvvmcVM.NavigatedToMode = navigationMode;
                    mvvmcVM.ViewBag = viewBag;
                    mvvmcVM.NavigationParameter = parameter;
                    mvvmcVM.Initialize();
                }

                ChangeContentInRegion(target, controllerId);
                NavigationOccured?.Invoke(controllerId, prevPage, pageName);
            });
        }

        private void ChangeContentInRegion(object content, string controllerID)
        {
            var currentThread = Thread.CurrentThread;
            Region navArea = FindRegionByID(controllerID);
            navArea.Content = content;
        }

        public void RunOnUIThread(Action act)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                act();
            else
            {
                _dispatcher.Invoke(act);
            }
        }

        public void RunOnUIThreadAsync(Action act)
        {
            _dispatcher.BeginInvoke(act);
        }

        public void AddGoBackCommand(GoBackCommand goBackCommand)
        {
            _goBackCommands.Add(new WeakReference<GoBackCommand>(goBackCommand));
        }

        public void AddGoForwardCommand(GoForwardCommand goForwardCommand)
        {
            _goForwardCommands.Add(new WeakReference<GoForwardCommand>(goForwardCommand));
        }

        public void ChangeCanGoBack(string controllerId)
        {
            var goBackCommands = GetGoBackCommands(controllerId);
            foreach (var goBackCommand in goBackCommands)
            {
                goBackCommand.ChangeCanExecute();
            }
            CanGoBackChangedEvent?.Invoke(controllerId);
        }

        public void ChangeCanGoForward(string controllerId)
        {
            var goForwardCommands = GetGoForwardCommands(controllerId);
            foreach (var goForwardCommand in goForwardCommands)
            {
                goForwardCommand.ChangeCanExecute();
            }
            CanGoForwardChangedEvent?.Invoke(controllerId);
        }

        public IEnumerable<GoBackCommand> GetGoBackCommands(string controllerId)
        {
            RefreshGoBackCommands();
            return _goBackCommands.Select(wr =>
            {
                wr.TryGetTarget(out GoBackCommand goBackCommand);
                return goBackCommand;
            }).Where(cmd => cmd != null && cmd.ControllerID.Equals(controllerId, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<GoForwardCommand> GetGoForwardCommands(string controllerId)
        {
            RefreshGoForwardCommands();
            return _goForwardCommands.Select(wr =>
            {
                wr.TryGetTarget(out GoForwardCommand goForwardCommand);
                return goForwardCommand;
            }).Where(cmd => cmd != null && cmd.ControllerID.Equals(controllerId, StringComparison.CurrentCultureIgnoreCase));
        }

        private void RefreshGoBackCommands()
        {
            List<WeakReference<GoBackCommand>> toRemove =
                _goBackCommands.Where(wr => !wr.TryGetTarget(out GoBackCommand goBackCommand)).ToList();
            foreach (var wr in toRemove)
            {
                _goBackCommands.Remove(wr);
            }
        }

        private void RefreshGoForwardCommands()
        {
            List<WeakReference<GoForwardCommand>> toRemove =
                _goForwardCommands.Where(wr => !wr.TryGetTarget(out GoForwardCommand goForwardCommand)).ToList();
            foreach (var wr in toRemove)
            {
                _goForwardCommands.Remove(wr);
            }
        }
    }
}
