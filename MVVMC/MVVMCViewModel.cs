using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMC
{
    public abstract class MVVMCViewModel : BaseViewModel
    {
        public Dictionary<string, object> ViewBag { get; set; }
        public object NavigationParameter { get; set; }
        protected IController _controller;
        
        /// <summary>
        /// Represents the navigation mode to this ViewModel. For example, when this equals to 'HistoryBack', it means the Page
        /// was navigated to with the 'GoBack' command.
        /// </summary>
        public NavigationMode NavigatedToMode { get; set; }

        /// <summary>
        /// Called after ViewModel is created and NavigationParameter and ViewBag are set
        /// But before Region content is changed and before View is loaded
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// Called after Region content is changed and View is already Loaded.
        /// You can execute additional navigations here.
        /// </summary>
        public virtual void OnLoad()
        {

        }

        /// <summary>
        /// Called when navigating away from ViewModel.
        /// It's possible to cancel navigation with "args.CancelNavigation = true"
        /// </summary>
        public virtual void OnLeave(LeavingPageEventArgs args)
        {
            
        }

        public virtual void SetController(IController controller)
        {
            _controller = controller;
        }

        public IController GetController()
        {
            return _controller;
        }
    }

    public class MVVMCViewModel<TController> : MVVMCViewModel where TController : Controller
    {
        TController _exactController = null;
        public override void SetController(IController controller)
        {
            _controller = controller;
            _exactController = controller as TController;
        }

        public TController GetExactController()
        {
            return _exactController;
        }

    }
}
