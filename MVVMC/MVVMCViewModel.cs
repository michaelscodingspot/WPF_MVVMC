using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMC
{
    public abstract class MVVMCViewModel : BaseViewModel
    {
        protected Controller _controller;
        public virtual void SetController(Controller controller)
        {
            _controller = controller;
        }

        public Controller GetController()
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
        public override void SetController(Controller controller)
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
