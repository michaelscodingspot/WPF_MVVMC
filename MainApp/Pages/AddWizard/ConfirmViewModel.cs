using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace MainApp.Pages.AddWizard
{
    public class ConfirmViewModel : MVVMCViewModel, IAddEmployeeStep
    {
        public bool IsNextLegal(out string errorMsg)
        {
            errorMsg = null;
            return true;
        }
    }
}
