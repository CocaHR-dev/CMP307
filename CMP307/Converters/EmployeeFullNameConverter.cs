using System;
using System.Globalization;
using System.Windows.Data;
using CMP307.Models;

namespace CMP307.Converters
{
    public class EmployeeFullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Employee employee)
            {
                return $"{employee.FirstName} {employee.LastName}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

