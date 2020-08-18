﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class BoolToAsteriskConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return "*";
			else
				return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
