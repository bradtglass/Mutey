using System;
using System.Windows.Media;

namespace HueMicIndicator;

public static class ColorUtility
{
    public static Color TempToColor(double temp)
    {
        // Source: https://tannerhelland.com/2012/09/18/convert-temperature-rgb-algorithm-code.html
        var r = GetRed(temp);
        var g = GetGreen(temp);
        var b = GetBlue(temp);

        return Color.FromRgb(r, g, b);
    }

    private static byte GetRed(double temp)
    {
        temp /= 100;

        if (temp <= 66)
            return byte.MaxValue;

        var r = temp - 60;
        r = 329.698727446 * Math.Pow(r, -0.1332047592);

        return LimitToByte(r);
    }

    private static byte GetGreen(double temp)
    {
        temp /= 100;

        var g = temp;
        if (temp <= 66)
        {
            g = 99.4708025861 * Math.Log(g) - 161.1195681661;
        }
        else
        {
            g -= 60;
            g = 288.1221695283 * Math.Pow(g, -0.0755148492);
        }

        return LimitToByte(g);
    }

    private static byte GetBlue(double temp)
    {
        temp /= 100;

        if (temp >= 66)
            return byte.MaxValue;

        var b = temp - 10;
        b = 138.5177312231 * Math.Log(b) - 305.0447927307;

        return LimitToByte(b);
    }

    private static byte LimitToByte(double value)
        => value switch
        {
            > 255 => 255,
            < 0 => 0,
            _ => (byte)value
        };
}