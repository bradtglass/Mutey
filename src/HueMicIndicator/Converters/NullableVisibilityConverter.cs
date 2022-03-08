using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HueMicIndicator.Converters;

public class NullableVisibilityConverter : IValueConverter
{
    public Visibility NullValue { get; set; } = Visibility.Collapsed;

    public Visibility NotNullValue { get; set; } = Visibility.Visible;

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        => value is null ? NullValue : NotNullValue;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}