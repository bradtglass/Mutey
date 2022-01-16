using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HueMicIndicator.Converters;

public class BoolVisibilityConverter : IValueConverter
{
    public Visibility TrueValue { get; set; } = Visibility.Visible;

    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (bool)value ? TrueValue : FalseValue;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var visibility = (Visibility)value;

        if (visibility == TrueValue)
            return true;

        if (visibility == FalseValue)
            return false;

        return Binding.DoNothing;
    }
}