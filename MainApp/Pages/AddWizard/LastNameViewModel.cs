using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace MainApp.Pages.AddWizard
{
    public class LastNameViewModel : MVVMCViewModel
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
    }
}
