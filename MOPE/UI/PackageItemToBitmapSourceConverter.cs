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
	public class PackageItemToBitmapSourceConverter : IValueConverter
	{
		public IconManager IconManager { get; }

		public PackageItemToBitmapSourceConverter()
		{
			IconManager = MainWindow.IconManager;
		}

		public PackageItemToBitmapSourceConverter(IconManager iconManager)
		{
			IconManager = iconManager;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var item = value as PackageItem;
			if (!int.TryParse(parameter as string, out int size))
				size = 32;

			if (item.IsFolder())
				return IconManager.GetFolderImage(size);
			else
				return IconManager.GetImageForPart(item.Part, size);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
