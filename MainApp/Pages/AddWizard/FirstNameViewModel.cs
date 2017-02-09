using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace MainApp.Pages.AddWizard
{
    public class FirstNameViewModel :MVVMCViewModel
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
    }
}
