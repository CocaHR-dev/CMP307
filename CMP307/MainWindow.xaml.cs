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
using CMP307.Models;
using CMP307.TemplateSelectors;
using System.Management;
using CMP307.Services;
using CMP307.ViewModels;


namespace CMP307
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ScottishGlenContext _context;
        private readonly AddEditButtonTemplateSelector _addEditButtonTemplateSelector;
        private readonly DeleteButtonTemplateSelector _deleteButtonTemplateSelector;
        private readonly EmployeeViewModel _employeeViewModel;
        private readonly HardwareViewModel _hardwareViewModel;
        private readonly DepartmentViewModel _departmentViewModel;

        public MainWindow(ScottishGlenContext context, AddEditButtonTemplateSelector addEditButtonTemplateSelector, DeleteButtonTemplateSelector deleteButtonTemplateSelector, EmployeeViewModel employeeViewModel, HardwareViewModel hardwareViewModel, DepartmentViewModel departmentViewModel)
        {
            InitializeComponent();
            _context = context;
            _addEditButtonTemplateSelector = addEditButtonTemplateSelector;
            _deleteButtonTemplateSelector = deleteButtonTemplateSelector;
            _employeeViewModel = employeeViewModel;
            _hardwareViewModel = hardwareViewModel;
            _departmentViewModel = departmentViewModel;

            DataContext = _employeeViewModel;
        }

        private async void EmployeesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataContext = _employeeViewModel;
            await DisplayEmployeesTable();
            HardwareScanButton.Visibility = Visibility.Collapsed;
        }

        private async void HardwareMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataContext = _hardwareViewModel;
            await DisplayHardwareTable();
            HardwareScanButton.Visibility = Visibility.Visible;
        }

        private async void DepartmentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataContext = _departmentViewModel;
            await DisplayDepartmentsTable();
            HardwareScanButton.Visibility = Visibility.Collapsed;
        }

        private async Task DisplayEmployeesTable()
        {
            var employees = await _context.Employees.Include(e => e.Department).ToListAsync();
            var departments = await _context.Departments.ToListAsync();
            MainDataGrid.ItemsSource = employees;
            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Employee ID", Binding = new System.Windows.Data.Binding("EmployeeId"), IsReadOnly = true });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "First Name", Binding = new System.Windows.Data.Binding("FirstName") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Last Name", Binding = new System.Windows.Data.Binding("LastName") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new System.Windows.Data.Binding("Email") });
            MainDataGrid.Columns.Add(new DataGridComboBoxColumn
            {
                Header = "Department",
                SelectedValueBinding = new System.Windows.Data.Binding("DepartmentId"),
                DisplayMemberPath = "DepartmentName",
                SelectedValuePath = "DepartmentId",
                ItemsSource = departments
            });
            AddButtonColumns();
        }

        private async Task DisplayHardwareTable()
        {
            var hardware = await _context.HardwareAssets.Include(h => h.Employee).ToListAsync();
            var employees = await _context.Employees.ToListAsync();
            MainDataGrid.ItemsSource = hardware;
            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Hardware ID", Binding = new System.Windows.Data.Binding("HardwareId"), IsReadOnly = true });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "System Name", Binding = new System.Windows.Data.Binding("SystemName") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Model", Binding = new System.Windows.Data.Binding("Model") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Manufacturer", Binding = new System.Windows.Data.Binding("Manufacturer") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Type", Binding = new System.Windows.Data.Binding("Type") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "IP Address", Binding = new System.Windows.Data.Binding("IpAddress") });

            // Custom column for Purchase Date with DatePicker
            var purchaseDateColumn = new DataGridTemplateColumn
            {
                Header = "Purchase Date",
                CellTemplate = new DataTemplate
                {
                    VisualTree = new FrameworkElementFactory(typeof(TextBlock))
                },
                CellEditingTemplate = new DataTemplate
                {
                    VisualTree = new FrameworkElementFactory(typeof(DatePicker))
                }
            };

            var cellTemplateFactory = purchaseDateColumn.CellTemplate.VisualTree;
            cellTemplateFactory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("PurchaseDate") { StringFormat = "d" });

            var cellEditingTemplateFactory = purchaseDateColumn.CellEditingTemplate.VisualTree;
            cellEditingTemplateFactory.SetBinding(DatePicker.SelectedDateProperty, new System.Windows.Data.Binding("PurchaseDate") { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            MainDataGrid.Columns.Add(purchaseDateColumn);

            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Note", Binding = new System.Windows.Data.Binding("Note") });

            // Custom column for Employee Full Name
            MainDataGrid.Columns.Add(new DataGridComboBoxColumn
            {
                Header = "Employee",
                SelectedValueBinding = new System.Windows.Data.Binding("EmployeeId"),
                DisplayMemberPath = "FullName",
                SelectedValuePath = "EmployeeId",
                ItemsSource = employees
            });
            AddButtonColumns();
        }

        private async Task DisplayDepartmentsTable()
        {
            var departments = await _context.Departments.ToListAsync();
            MainDataGrid.ItemsSource = departments;
            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Department ID", Binding = new System.Windows.Data.Binding("DepartmentId"), IsReadOnly = true });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Department Name", Binding = new System.Windows.Data.Binding("DepartmentName") });
            AddButtonColumns();
        }

        private void AddButtonColumns()
        {
            // Add/Edit Button Column
            var addEditButtonColumn = new DataGridTemplateColumn
            {
                Header = "Actions",
                CellTemplateSelector = _addEditButtonTemplateSelector
            };
            MainDataGrid.Columns.Add(addEditButtonColumn);

            // Delete Button Column
            var deleteButtonColumn = new DataGridTemplateColumn
            {
                Header = "Delete",
                CellTemplateSelector = _deleteButtonTemplateSelector
            };
            MainDataGrid.Columns.Add(deleteButtonColumn);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var row = DataGridRow.GetRowContainingElement(button);
                if (row != null)
                {
                    MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                    var item = row.Item;
                    if (item is Employee employee)
                    {
                        _employeeViewModel.Employee = employee;
                        if (_employeeViewModel.AddCommand.CanExecute(null))
                        {
                            _employeeViewModel.AddCommand.Execute(null);
                        }
                    }
                    else if (item is Hardware hardware)
                    {
                        _hardwareViewModel.Hardware = hardware;
                        if (_hardwareViewModel.AddCommand.CanExecute(null))
                        {
                            _hardwareViewModel.AddCommand.Execute(null);
                        }
                    }
                    else if (item is Department department)
                    {
                        _departmentViewModel.Department = department;
                        if (_departmentViewModel.AddCommand.CanExecute(null))
                        {
                            _departmentViewModel.AddCommand.Execute(null);
                        }
                    }
                    MainDataGrid.Items.Refresh();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var row = DataGridRow.GetRowContainingElement(button);
                if (row != null)
                {
                    MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                    var item = row.Item;
                    if (item is Employee employee)
                    {
                        _employeeViewModel.Employee = employee;
                        if (_employeeViewModel.EditCommand.CanExecute(null))
                        {
                            _employeeViewModel.EditCommand.Execute(null);
                        }
                    }
                    else if (item is Hardware hardware)
                    {
                        _hardwareViewModel.Hardware = hardware;
                        if (_hardwareViewModel.EditCommand.CanExecute(null))
                        {
                            _hardwareViewModel.EditCommand.Execute(null);
                        }
                    }
                    else if (item is Department department)
                    {
                        _departmentViewModel.Department = department;
                        if (_departmentViewModel.EditCommand.CanExecute(null))
                        {
                            _departmentViewModel.EditCommand.Execute(null);
                        }
                    }
                    MainDataGrid.Items.Refresh();
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var row = DataGridRow.GetRowContainingElement(button);
                if (row != null)
                {
                    var item = row.Item;
                    if (item is Employee employee)
                    {
                        _employeeViewModel.Employee = employee;
                        if (_employeeViewModel.DeleteCommand.CanExecute(null))
                        {
                            _employeeViewModel.DeleteCommand.Execute(null);
                            await DisplayEmployeesTable();
                        }
                    }
                    else if (item is Hardware hardware)
                    {
                        _hardwareViewModel.Hardware = hardware;
                        if (_hardwareViewModel.DeleteCommand.CanExecute(null))
                        {
                            _hardwareViewModel.DeleteCommand.Execute(null);
                            await DisplayHardwareTable();
                        }
                    }
                    else if (item is Department department)
                    {
                        _departmentViewModel.Department = department;
                        if (_departmentViewModel.DeleteCommand.CanExecute(null))
                        {
                            _departmentViewModel.DeleteCommand.Execute(null);
                            await DisplayDepartmentsTable();
                        }
                    }
                }
            }
        }

        private void HardwareScanButton_Click(object sender, RoutedEventArgs e)
        {
            // Scan the current hardware and get the data
            var scannedHardware = HardwareScanner.ScanHardware();

            // Create a new Hardware object with the scanned data
            var newHardware = new Hardware
            {
                SystemName = scannedHardware.SystemName,
                Model = scannedHardware.Model,
                Manufacturer = scannedHardware.Manufacturer,
                Type = scannedHardware.Type,
                IpAddress = scannedHardware.IpAddress
            };

            // Add the new hardware to the DataGrid
            var hardwareList = MainDataGrid.ItemsSource as List<Hardware>;
            if (hardwareList != null)
            {
                hardwareList.Add(newHardware);
                MainDataGrid.Items.Refresh();
            }
        }
    }
}
