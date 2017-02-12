using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMC
{
    public abstract class MVVMCViewModel : BaseViewModel
    {
        protected IController _controller;
        public virtual void SetController(IController controller)
        {
            _controller = controller;
        }

        public IController GetController()
        {
            return _controller;
        }

        public Dictionary<string, object> ViewBag { get; set; }
        public object NavigationParameter { get; set; }

        public virtual void Initialize()
        {
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
