using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CMP307.Data;
using Microsoft.EntityFrameworkCore;
using CMP307.Converters;

namespace CMP307
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ScottishGlenContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ScottishGlenContext();
        }

        private void EmployeesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var employees = _context.Employees.Include(e => e.Department).ToList();
            MainDataGrid.ItemsSource = employees;
            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Employee ID", Binding = new System.Windows.Data.Binding("EmployeeId") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "First Name", Binding = new System.Windows.Data.Binding("FirstName") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Last Name", Binding = new System.Windows.Data.Binding("LastName") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new System.Windows.Data.Binding("Email") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Department Name", Binding = new System.Windows.Data.Binding("Department.DepartmentName") });
        }

        private void HardwareMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var hardware = _context.HardwareAssets.Include(h => h.Employee).ToList();
            MainDataGrid.ItemsSource = hardware;
            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Hardware ID", Binding = new System.Windows.Data.Binding("HardwareId") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "System Name", Binding = new System.Windows.Data.Binding("SystemName") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new System.Windows.Data.Binding("Model") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Manufacturer", Binding = new System.Windows.Data.Binding("Manufacturer") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Type", Binding = new System.Windows.Data.Binding("Type") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "IP Address", Binding = new System.Windows.Data.Binding("IpAddress") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Purchase Date", Binding = new System.Windows.Data.Binding("PurchaseDate") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Note", Binding = new System.Windows.Data.Binding("Note") });

            // Custom column for Employee Full Name
            MainDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Employee Name",
                Binding = new System.Windows.Data.Binding("Employee")
                {
                    Converter = new EmployeeFullNameConverter()
                }
            });
        }

        private void DepartmentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var departments = _context.Departments.ToList();
            MainDataGrid.ItemsSource = departments;
            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Department ID", Binding = new System.Windows.Data.Binding("DepartmentId") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Department Name", Binding = new System.Windows.Data.Binding("DepartmentName") });
        }

        private void DepartmentsMenuItem_MouseEnter (object sender, RoutedEventArgs e)
        {
            DepartmentsMenuItem.Foreground = Brushes.Red;
        }
    }
}
