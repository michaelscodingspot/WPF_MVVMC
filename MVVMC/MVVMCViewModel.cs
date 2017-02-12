using System;
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

        public virtual void Initialize()
        {
            //This is called after NavigationParameter and ViewBag are set
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
