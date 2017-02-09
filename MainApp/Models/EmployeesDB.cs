using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Models
{
    public class Employee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Salary { get; set; }
        public int ChristmasBonus { get; set; }
    }

    public static class EmployeesDB
    {
        public static List<Employee> Employees { get; set; }

        static EmployeesDB()
        {
            Employees = new List<Employee>();
            Employees.Add(new Employee() { FirstName = "Bill", LastName = "Gates", Salary = 19000, ChristmasBonus = 10000 });
            Employees.Add(new Employee() { FirstName = "Warren", LastName = "Buffet", Salary = 23000, ChristmasBonus = 40000 });
            Employees.Add(new Employee() { FirstName = "Amancia", LastName = "Ortega", Salary = 15000, ChristmasBonus = 50000 });
            Employees.Add(new Employee() { FirstName = "Jeff", LastName = "Bezos", Salary = 98000, ChristmasBonus = 35000 });
            Employees.Add(new Employee() { FirstName = "Charles", LastName = "Koch", Salary = 105000, ChristmasBonus = 58000 });
        }
        
    }
}
