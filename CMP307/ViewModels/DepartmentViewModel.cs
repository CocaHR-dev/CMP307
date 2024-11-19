using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using CMP307.Models;
using CMP307.Services;
using CMP307.Data;
using System;
using System.Windows;


namespace CMP307.ViewModels
{
    public class DepartmentViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly DepartmentService _departmentService;
        private Department _department;

        public DepartmentViewModel(DepartmentService departmentService)
        {
            _departmentService = departmentService;
            _department = new Department();
            AddCommand = new RelayCommand(AddDepartment, CanAddDepartment);
            EditCommand = new RelayCommand(EditDepartment, CanEditDepartment);
            DeleteCommand = new RelayCommand(DeleteDepartment, CanDeleteDepartment);
        }

        public Department Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged(nameof(Department));
                ValidateAllProperties();
            }
        }

        [Required(ErrorMessage = "Department Name is required")]
        public string DepartmentName
        {
            get => _department.DepartmentName;
            set
            {
                _department.DepartmentName = value;
                OnPropertyChanged(nameof(DepartmentName));
                ValidateProperty(value, nameof(DepartmentName));
            }
        }

        public ICollection<Employee> Employees
        {
            get => _department.Employees;
            set
            {
                _department.Employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanAddDepartment(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool CanEditDepartment(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool CanDeleteDepartment(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void AddDepartment(object parameter)
        {
            _departmentService.AddDepartment(_department);
        }

        private void EditDepartment(object parameter)
        {
            _departmentService.EditDepartment(_department);
        }

        private void DeleteDepartment(object parameter)
        {
            _departmentService.DeleteDepartment(_department);
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

            // Custom validation for non-nullable text fields
            if (propertyName == nameof(DepartmentName))
            {
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    _errors[propertyName] = new List<string> { $"{propertyName} cannot be empty." };
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
