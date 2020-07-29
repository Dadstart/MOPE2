using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace B4.Mope.UI
{
	public class PartToBitmapSourceConverter : IValueConverter
	{
		public IconManager IconManager { get; }

		public PartToBitmapSourceConverter()
		{
			IconManager = MainWindow.IconManager;
		}

		public PartToBitmapSourceConverter(IconManager iconManager)
		{
			IconManager = iconManager;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var part = value as Part;
			//return "icons/unknown.ico";
			if (!int.TryParse(parameter as string, out int size))
				size = 32;
			return IconManager.GetImageForPart(part, size);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
