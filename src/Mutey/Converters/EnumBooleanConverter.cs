using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Mutey.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => parameter.Equals(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
                return parameter;

            return DependencyProperty.UnsetValue;
        }
    }
}