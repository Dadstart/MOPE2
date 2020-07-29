using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace B4.Mope.UI
{
	public class ContentTypeToIconConverter : IValueConverter
	{
		public IconManager IconManager { get; }

		public ContentTypeToIconConverter()
		{
			IconManager = MainWindow.IconManager;
		}

		public ContentTypeToIconConverter(IconManager iconManager)
		{
			IconManager = iconManager;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//return "icons/unknown.ico";
			if (!int.TryParse(parameter as string, out int size))
				size = 32;
			return IconManager.GetImageForContentType(value as string, size);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
