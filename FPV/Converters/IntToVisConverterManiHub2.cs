using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HMI.Converter
{
   [ValueConversion(typeof(object), typeof(Visibility))]
    public class IntToVisConverterManiHub2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short)
            {
                if (9<=(short)value && (short)value<=14)
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
