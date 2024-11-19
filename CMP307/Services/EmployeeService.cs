using CMP307.Data;
using CMP307.Models;
using System.Collections.Generic;
using System.Linq;

namespace CMP307.Services
{
    public class EmployeeService
    {
        private readonly ScottishGlenContext _context;

        public EmployeeService(ScottishGlenContext context)
        {
            _context = context;
        }

        public void AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public void EditEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
        }

        public void DeleteEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
        }

        public List<Employee> GetAllEmployees()
        {
            return _context.Employees.ToList();
        }
    }
}
