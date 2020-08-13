using B4.Mope.Packaging;
using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class PackageItemToNameStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var info = value as PackageItem;
			return info.Name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
