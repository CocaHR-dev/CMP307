using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using CMP307.Models;
using CMP307.Data;
using CMP307.Services;
using System;
using System.Windows;


namespace CMP307.ViewModels
{
    public class HardwareViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly HardwareService _hardwareService;
        private Hardware _hardware;

        public HardwareViewModel(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
            _hardware = new Hardware();
            AddCommand = new RelayCommand(AddHardware, CanAddHardware);
            EditCommand = new RelayCommand(EditHardware, CanEditHardware);
            DeleteCommand = new RelayCommand(DeleteHardware, CanDeleteHardware);
        }

        public Hardware Hardware
        {
            get => _hardware;
            set
            {
                _hardware = value;
                OnPropertyChanged(nameof(Hardware));
                ValidateAllProperties();
            }
        }

        [Required(ErrorMessage = "System Name is required")]
        [MaxLength(100)]
        public string SystemName
        {
            get => _hardware.SystemName;
            set
            {
                _hardware.SystemName = value;
                OnPropertyChanged(nameof(SystemName));
                ValidateProperty(value, nameof(SystemName));
            }
        }

        [Required(ErrorMessage = "Model is required")]
        [MaxLength(50)]
        public string Model
        {
            get => _hardware.Model;
            set
            {
                _hardware.Model = value;
                OnPropertyChanged(nameof(Model));
                ValidateProperty(value, nameof(Model));
            }
        }

        [Required(ErrorMessage = "Manufacturer is required")]
        public string Manufacturer
        {
            get => _hardware.Manufacturer;
            set
            {
                _hardware.Manufacturer = value;
                OnPropertyChanged(nameof(Manufacturer));
                ValidateProperty(value, nameof(Manufacturer));
            }
        }

        [Required(ErrorMessage = "Type is required")]
        public string Type
        {
            get => _hardware.Type;
            set
            {
                _hardware.Type = value;
                OnPropertyChanged(nameof(Type));
                ValidateProperty(value, nameof(Type));
            }
        }

        [Required(ErrorMessage = "IP Address is required")]
        public string IpAddress
        {
            get => _hardware.IpAddress;
            set
            {
                _hardware.IpAddress = value;
                OnPropertyChanged(nameof(IpAddress));
                ValidateProperty(value, nameof(IpAddress));
            }
        }

        public DateTime? PurchaseDate
        {
            get => _hardware.PurchaseDate;
            set
            {
                _hardware.PurchaseDate = value;
                OnPropertyChanged(nameof(PurchaseDate));
            }
        }

        public string? Note
        {
            get => _hardware.Note;
            set
            {
                _hardware.Note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeId
        {
            get => _hardware.EmployeeId;
            set
            {
                _hardware.EmployeeId = value;
                OnPropertyChanged(nameof(EmployeeId));
                ValidateProperty(value, nameof(EmployeeId));
            }
        }

        public Employee Employee
        {
            get => _hardware.Employee;
            set
            {
                _hardware.Employee = value;
                OnPropertyChanged(nameof(Employee));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private bool CanAddHardware(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool CanEditHardware(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private bool CanDeleteHardware(object parameter)
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                MessageBox.Show(string.Join(Environment.NewLine, _errors.Values.SelectMany(e => e)), "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void AddHardware(object parameter)
        {
            _hardwareService.AddHardware(_hardware);
        }

        private void EditHardware(object parameter)
        {
            _hardwareService.EditHardware(_hardware);
        }

        private void DeleteHardware(object parameter)
        {
            _hardwareService.DeleteHardware(_hardware);
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
            if (propertyName == nameof(SystemName) || propertyName == nameof(Model) || propertyName == nameof(Manufacturer) || propertyName == nameof(Type) || propertyName == nameof(IpAddress))
            {
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    _errors[propertyName] = new List<string> { $"{propertyName} cannot be empty." };
                }
            }
            else if (propertyName == nameof(EmployeeId))
            {
                if ((int)value == 0)
                {
                    _errors[propertyName] = new List<string> { "Employee is required." };
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
