using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace MainApp.Pages.AddWizard
{
    public class FirstNameViewModel :MVVMCViewModel, IAddEmployeeStep
    {
        public string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public bool IsNextLegal(out string errorMsg)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                errorMsg = "First name can't be empty";
                return false;
            }

            errorMsg = null;
            return true;
        }
    }
}
