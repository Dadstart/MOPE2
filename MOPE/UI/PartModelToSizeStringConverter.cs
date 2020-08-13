using B4.Mope.Packaging;
using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class PartModelToSizeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var model = value as PartModel;
			var part = model?.Part;
			if (part == null)
				return "???";

			return $"{SizeStringConverter.GetSizeString(part.Size)} ({SizeStringConverter.GetSizeString(part.CompressedSize)})";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
