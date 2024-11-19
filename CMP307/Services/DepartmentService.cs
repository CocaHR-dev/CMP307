using CMP307.Data;
using CMP307.Models;
using System.Collections.Generic;
using System.Linq;

namespace CMP307.Services
{
    public class DepartmentService
    {
        private readonly ScottishGlenContext _context;

        public DepartmentService(ScottishGlenContext context)
        {
            _context = context;
        }

        public void AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        public void EditDepartment(Department department)
        {
            _context.Departments.Update(department);
            _context.SaveChanges();
        }

        public void DeleteDepartment(Department department)
        {
            _context.Departments.Remove(department);
            _context.SaveChanges();
        }

        public List<Department> GetAllDepartments()
        {
            return _context.Departments.ToList();
        }
    }
}
