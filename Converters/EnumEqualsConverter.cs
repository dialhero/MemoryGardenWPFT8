using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoryGardenWPF.Converters
{
    public class EnumEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value?.Equals(parameter) == true;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? parameter! : Binding.DoNothing;
    }
}
