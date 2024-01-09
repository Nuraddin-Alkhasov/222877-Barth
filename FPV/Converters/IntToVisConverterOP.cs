using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI.Converter
{
   [ValueConversion(typeof(object), typeof(Visibility))]
    public class IntToVisConverterOP : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short)
            {
                if ((short)value == 0)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
