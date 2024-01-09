using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI.Converter
{
   [ValueConversion(typeof(object), typeof(Visibility))]
    public class IntToVisConverterManiHub1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short)
            {
                if (1<=(short)value && (short)value<=8)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
