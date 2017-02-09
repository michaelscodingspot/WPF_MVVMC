using MainApp.Models;
using MVVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Pages.AddWizard
{
    public class AddWizardController : Controller
    {
        Employee _newEmployee = null;

        Dictionary<string, object> _viewBag = null;
        public void Prev()
        {
            string currentPage = GetCurrentPageName();
            string target = null;
            switch (currentPage)
            {
                case "Initial":
                    NavigationService.NavigateWithController<MainOperation.InitialViewModel>();
                    return;
                case "FirstName":
                    target = "Initial"; break;
                case "LastName":
                    target = "FirstName"; break;
                case "Salary":
                    target = "LastName"; break;
                case "Confirm":
                    target = "Salary"; break;
                default:
                    break;
            }
            base.Navigate(target, null);
        }

        public void Next()
        {
            _viewBag = null;
            string currentPage = GetCurrentPageName();
            string target = "Initial";
            switch (currentPage)
            {
                case "Initial":
                    target = "FirstName";
                    _newEmployee = new Employee();
                    break;
                case "FirstName":
                    MVVMCViewModel vm = base.GetCurrentViewModel();
                    var step1VM = vm as FirstNameViewModel;
                    _newEmployee.FirstName = step1VM?.FirstName;
                    target = "LastName"; break;
                case "LastName":
                    MVVMCViewModel vm2 = base.GetCurrentViewModel();
                    var step2VM = vm2 as LastNameViewModel;
                    _newEmployee.LastName = step2VM?.LastName;
                    target = "Salary"; break;
                case "Salary":
                    MVVMCViewModel vm3 = base.GetCurrentViewModel();
                    var step3VM = vm3 as SalaryViewModel;
                    _newEmployee.Salary = int.Parse(step3VM?.Salary);
                    _viewBag = new Dictionary<string, object>();
                    _viewBag.Add("Employee", _newEmployee);
                    target = "Confirm"; break;
                case "Confirm":
                    EmployeesDB.Employees.Add(_newEmployee);
                    NavigationService.GetController("MainOperation").NavigateToInitial();
                    return;
                default:
                    break;
            }
            base.Navigate(target, null);
        }

        public void Initial()
        {
            ExecuteNavigation();
        }
        public void FirstName()
        {
            ExecuteNavigation(null, _viewBag);
        }

        public void LastName()
        {
            ExecuteNavigation(null, _viewBag);
        }

        public void Salary()
        {
            ExecuteNavigation(null, _viewBag);
        }

        public void Confirm()
        {
            ExecuteNavigation(null, _viewBag);
        }

    }
}
