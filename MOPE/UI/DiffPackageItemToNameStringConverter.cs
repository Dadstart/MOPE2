using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class DiffPackageItemToNameStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var item = value as DiffPackageItem;
			if (item.IsFolder())
				return $"{item.Path}/";
			else
				return item.Path;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
