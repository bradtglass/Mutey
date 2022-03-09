namespace Mutey.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class EqualityConverter : IValueConverter
    {
        public object? EqualValue { get; set; }

        public object? NotEqualValue { get; set; }

        public object? Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return parameter.Equals( value ) ? EqualValue : NotEqualValue;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( Equals( value, EqualValue ) )
            {
                return parameter;
            }

            return Binding.DoNothing;
        }
    }
}
