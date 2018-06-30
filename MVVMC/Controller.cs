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
    /// <summary>
    /// When mode is SaveParameterInstance, on navigation, the parameter instance and ViewBag is saved until the history is cleared or Region is unloaded.
    /// This can be convenient to simply "GoBack()" with Controller, but can potentially cause memory leaks.
    /// When mode is DiscardParameterInstance, the parameter and ViewBag is not saved and a parameter is required on GoBack / GoForward operation
    /// </summary>
    public enum HistoryMode
    {
        SaveParameterInstance,
        DiscardParameterInstance
    }


    public abstract class Controller : IController
    {
        enum NavigationMode { Regular, HistoryBack, HistoryForward };

        protected class HistoryItem
        {
            public string PageName { get; set; }
            public object NavigationParameter { get; set; }
            public Dictionary<string, object> ViewBag { get; set; }
            public HistoryMode HistoryModeAtTheTime { get; set; }
        }

        private Type _thisType;
        private MethodInfo[] _methods;

        public INavigationService NavigationService { get; set; }
        public INavigationExecutor NavigationExecutor { get; set; }

        public string ID { get; internal set; }

        protected HistoryMode HistoryMode { get; set; } = HistoryMode.DiscardParameterInstance;
        protected List<HistoryItem> History { get; private set; } = new List<HistoryItem>();
        protected int HistoryCurrentItemIndex { get; set; } = -1;

        internal event Action CanGoBackChanged;
        internal event Action CanGoForwardChanged;

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

        public virtual void GoBack()
        {
            CheckGoBackPossible();
            var historyItem = History[HistoryCurrentItemIndex - 1];
            VerifyHistoryModeAtTheTimeWasSaveParameter(historyItem.HistoryModeAtTheTime);
            ExecuteNavigationInternal(historyItem.PageName, 
                historyItem.NavigationParameter, 
                NavigationMode.HistoryBack,
                historyItem.ViewBag == null ? null : new Dictionary<string, object>(historyItem.ViewBag));
        }

        public virtual void GoBack(object parameter, Dictionary<string, object> viewBag)
        {
            CheckGoBackPossible();
            var historyItem = History[HistoryCurrentItemIndex - 1];
            ExecuteNavigationInternal(historyItem.PageName, parameter, NavigationMode.HistoryBack, viewBag);
        }

        private void CheckGoBackPossible()
        {
            if (HistoryCurrentItemIndex <= 0)
            {
                throw new InvalidOperationException("Trying to GoBack with Controller, when it's the first navigation history item");
            }
        }

        public virtual void GoForward()
        {
            CheckGoForwardPossible();
            var historyItem = History[HistoryCurrentItemIndex + 1];
            VerifyHistoryModeAtTheTimeWasSaveParameter(historyItem.HistoryModeAtTheTime);
            ExecuteNavigationInternal(historyItem.PageName,
                historyItem.NavigationParameter,
                NavigationMode.HistoryBack,
                historyItem.ViewBag == null ? null : new Dictionary<string, object>(historyItem.ViewBag));
        }

        public virtual void GoForward(object parameter, Dictionary<string, object> viewBag)
        {
            CheckGoForwardPossible();
            var historyItem = History[HistoryCurrentItemIndex + 1];
            ExecuteNavigationInternal(historyItem.PageName, parameter, NavigationMode.HistoryForward, viewBag);
        }

        private void VerifyHistoryModeAtTheTimeWasSaveParameter(HistoryMode historyItemHistoryModeAtTheTime)
        {
            if (historyItemHistoryModeAtTheTime != HistoryMode.SaveParameterInstance)
            {
                throw new InvalidOperationException("Can't GoBack / GoForward without parameter because at the navigation time, " +
                                                    "the history mode was 'DiscardParameterInstance'. You can use the other overload and specify parameter and viewbag, even if null");
            }
        }

        private void CheckGoForwardPossible()
        {
            if (HistoryCurrentItemIndex >= History.Count -1)
            {
                throw new InvalidOperationException("Trying to GoForward with Controller, when it's the last navigation history item");
            }
        }

        public virtual bool CanGoBack
        {
            get { return HistoryCurrentItemIndex > 0; }
        }

        public virtual bool CanGoForward
        {
            get { return HistoryCurrentItemIndex < History.Count - 1; }
        }


        protected void ExecuteNavigation([CallerMemberName]string pageName = null)
        {
            ExecuteNavigationInternal(pageName, null, NavigationMode.Regular, null);
        }

        protected void ExecuteNavigation(object parameter, Dictionary<string, object> viewBag, [CallerMemberName]string pageName = null)
        {
            ExecuteNavigationInternal(pageName, parameter, NavigationMode.Regular, viewBag);
        }

        private void ExecuteNavigationInternal(string pageName, object parameter, NavigationMode navigationMode, Dictionary<string, object> viewBag = null)
        {
            ModifyHistory(pageName, parameter, navigationMode, viewBag);
            NavigationExecutor.ExecuteNavigation(ID, pageName, parameter, viewBag);
            CanGoBackChanged?.Invoke();
            CanGoForwardChanged?.Invoke();
        }

        private void ModifyHistory(string pageName, object parameter, NavigationMode navigationMode, Dictionary<string, object> viewBag)
        {
            switch (navigationMode)
            {
                case NavigationMode.Regular:
                    if (History.Count > HistoryCurrentItemIndex + 1) //Means current item is not last. We need to remove from history all items in front
                    {
                        History.RemoveRange(HistoryCurrentItemIndex + 1, History.Count - HistoryCurrentItemIndex - 1);
                    }

                    var newHistoryItem = new HistoryItem()
                    {
                        PageName = pageName,
                        NavigationParameter = HistoryMode == HistoryMode.SaveParameterInstance ? parameter : null,
                        ViewBag = viewBag == null ? null : new Dictionary<string, object>(viewBag),
                        HistoryModeAtTheTime = this.HistoryMode
                    };
                    History.Add(newHistoryItem);
                    HistoryCurrentItemIndex++;

                    break;
                case NavigationMode.HistoryBack:
                    
                    HistoryCurrentItemIndex--;
                    break;

                case NavigationMode.HistoryForward:
                    HistoryCurrentItemIndex++;
                    break;
            }

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
