using B4.Mope.Packaging;
using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class PartModelToContentTypeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var model = value as PartModel;
			var part = model?.Part;
			if (part == null)
				return "???";

			return part.ContentType;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
