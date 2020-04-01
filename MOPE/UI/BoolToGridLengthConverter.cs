using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class BoolToGridLengthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(GridLength))
				throw new InvalidOperationException("Unexpected target type");

			// star sizing when expanded, else auto
			if ((bool)value)
				return GridLength.Auto;
			else
				return new GridLength(1, GridUnitType.Star);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
