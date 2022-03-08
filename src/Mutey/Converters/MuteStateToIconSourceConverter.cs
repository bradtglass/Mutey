using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Mutey.Converters
{
    using Mutey.Audio;

    public class MuteStateToIconSourceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not MuteState state)
                return null;

            return state switch
            {
                MuteState.Unknown => GetSource("Icons/microphone-blue.ico"),
                MuteState.Muted => GetSource("Icons/microphone-red.ico"),
                MuteState.Unmuted => GetSource("Icons/microphone-green.ico"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();

        private static ImageSource GetSource(string path)
            => new BitmapImage(new Uri("pack://application:,,,/" + path));
    }
}