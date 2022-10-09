using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace STFC_EventLogger
{
    public class NullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
                if (string.IsNullOrEmpty(stringValue) | string.IsNullOrWhiteSpace(stringValue))
                    return DependencyProperty.UnsetValue;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

