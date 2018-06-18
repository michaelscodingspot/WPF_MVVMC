using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMC;

namespace MainApp.Pages.AddWizard
{
    public class SalaryViewModel :MVVMCViewModel , IAddEmployeeStep
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

        public bool IsNextLegal(out string errorMsg)
        {
            if (string.IsNullOrEmpty(Salary))
            {
                errorMsg = "Salary can't be empty.";
                return false;
            }

            if (!int.TryParse(Salary, out int res))
            {
                errorMsg = "Salary has to be an integer number.";
                return false;
            }

            errorMsg = null;
            return true;
        }
    }
}
