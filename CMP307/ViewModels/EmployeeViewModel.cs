using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using CMP307.Models;
using CMP307.Data;
using System;
using CMP307.Services;
using System.Windows;

namespace CMP307.ViewModels
{
public class EmployeeViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly EmployeeService _employeeService;
        private Employee _employee;

        public EmployeeViewModel(EmployeeService employeeService)
        {
            _employeeService = employeeService;
            _employee = new Employee();
            AddCommand = new RelayCommand(AddEmployee, CanAddEmployee);
            EditCommand = new RelayCommand(EditEmployee, CanEditEmployee);
            DeleteCommand = new RelayCommand(DeleteEmployee, CanDeleteEmployee);
        }

        public Employee Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged(nameof(Employee));
                ValidateAllProperties();
            }
        }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName
        {
            get => _employee.FirstName;
            set
            {
                _employee.FirstName = value;
                OnPropertyChanged(nameof(FirstName));
                ValidateProperty(value, nameof(FirstName));
            }
        }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName
        {
            get => _employee.LastName;
            set
            {
                _employee.LastName = value;
                OnPropertyChanged(nameof(LastName));
                ValidateProperty(value, nameof(LastName));
            }
        }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email
        {
            get => _employee.Email;
            set
            {
                _employee.Email = value;
                OnPropertyChanged(nameof(Email));
                ValidateProperty(value, nameof(Email));
            }
        }

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId
        {
            get => _employee.DepartmentId;
            set
            {
                _employee.DepartmentId = value;
                OnPropertyChanged(nameof(DepartmentId));
                ValidateProperty(value, nameof(DepartmentId));
            }
        }

        public Department Department
        {
            get => _employee.Department;
            set
            {
                _employee.Department = value;
                OnPropertyChanged(nameof(Department));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanAddEmployee(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool CanEditEmployee(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool CanDeleteEmployee(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void AddEmployee(object parameter)
        {
            _employeeService.AddEmployee(_employee);
        }

        private void EditEmployee(object parameter)
        {
            _employeeService.EditEmployee(_employee);
        }

        private void DeleteEmployee(object parameter)
        {
            _employeeService.DeleteEmployee(_employee);
        }

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ValidateProperty(object value, string propertyName)
        {
            var validationContext = new ValidationContext(this) { MemberName = propertyName };
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(value, validationContext, results);

            if (results.Any())
            {
                _errors[propertyName] = results.Select(r => r.ErrorMessage).ToList();
            }
            else
            {
                _errors.Remove(propertyName);
            }

            // Custom validation for non-nullable text fields and foreign key IDs
            if (propertyName == nameof(FirstName) || propertyName == nameof(LastName) || propertyName == nameof(Email))
            {
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    _errors[propertyName] = new List<string> { $"{propertyName} cannot be empty." };
                }
            }
            else if (propertyName == nameof(DepartmentId))
            {
                if ((int)value == 0)
                {
                    _errors[propertyName] = new List<string> { "Department is required." };
                }
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ValidateAllProperties()
        {
            foreach (var property in GetType().GetProperties())
            {
                ValidateProperty(property.GetValue(this), property.Name);
            }
        }
    }
}