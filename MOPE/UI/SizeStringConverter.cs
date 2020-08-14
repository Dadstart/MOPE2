using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class SizeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var size = (long)value;
			return GetSizeString(size);
		}

		public static string GetSizeString(long size)
		{
			if (size < 1024)
			{
				return $"{size:N0} b";
			}
			else if (size < 1024 * 1024)
			{
				return $"{(decimal)(size / 1024):N2} kb";
			}
			else
			{
				return $"{(decimal)(size / (1024 * 1024)):N2} mb";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
