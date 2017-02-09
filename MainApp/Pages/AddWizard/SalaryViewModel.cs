using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace MainApp.Pages.AddWizard
{
    public class SalaryViewModel :MVVMCViewModel
    {
        public string _salary;
        public string Salary
        {
            get { return _salary; }
            set
            {
                _salary = value;
                OnPropertyChanged();
            }
        }
    }
}
