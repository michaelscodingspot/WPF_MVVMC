using MainApp.Models;
using MVVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MainApp.Pages.AllEmployees
{
    public class SelectEmployeeViewModel : MVVMCViewModel<AllEmployeesController>
    {
        
        public ICommand _selectEmployeeCommand;
        public ICommand SelectEmployeeCommand
        {
            get
            {
                if (_selectEmployeeCommand == null)
                    _selectEmployeeCommand = new DelegateCommand(() =>
                    {
                        GetExactController().Info(SelectedEmployee);
                    },
                    ()=>
                    {
                        return SelectedEmployee != null;
                    });
                return _selectEmployeeCommand;
            }
        }

        public Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
                _selectEmployeeCommand.RaiseCanExecuteChanged();
            }
        }

        public List<Employee> Employees
        {
            get { return new List<Employee>(Models.EmployeesDB.Employees); }
        }



    }
}
