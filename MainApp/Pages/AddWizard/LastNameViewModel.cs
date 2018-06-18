using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace MainApp.Pages.AddWizard
{
    public class LastNameViewModel : MVVMCViewModel, IAddEmployeeStep
    {
        public string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName= value;
                OnPropertyChanged();
            }
        }

        public bool IsNextLegal(out string errorMsg)
        {
            if (string.IsNullOrEmpty(LastName))
            {
                errorMsg = "Last name can't be empty";
                return false;
            }

            errorMsg = null;
            return true;
        }
    }
}
