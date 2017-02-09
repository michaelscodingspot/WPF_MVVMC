using MainApp.Models;
using MVVMC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Pages.AllEmployees
{
    public class AllEmployeesController : Controller
    {
        public void Info(object parameter)
        {
            var viewBag = new Dictionary<string, object>();
            viewBag.Add("Employee", parameter as Employee);
            ExecuteNavigation(parameter, viewBag);
        }


        public void SelectEmployee()
        {
            ExecuteNavigation();
        }

        public void Initial()
        {
            SelectEmployee();
        }
    }
}
