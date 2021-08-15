using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Mutey.Converters
{
    public class EqualityConverter : IValueConverter
    {
        public object? EqualValue { get; set; }

        public object? NotEqualValue { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => parameter.Equals(value) ? EqualValue : NotEqualValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Equals(value, EqualValue))
                return parameter;

            return DependencyProperty.UnsetValue;
        }
    }
}