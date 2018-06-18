using MVVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Pages.AddWizard
{
    public class InitialViewModel :MVVMCViewModel, IAddEmployeeStep
    {
        public bool IsNextLegal(out string errorMsg)
        {
            errorMsg = null;
            return true;
        }
    }
}
