using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class DiffPackageItemToBitmapSourceConverter : IValueConverter
	{
		public IconManager IconManager { get; }

		public DiffPackageItemToBitmapSourceConverter()
		{
			IconManager = DiffWindow.IconManager;
		}

		public DiffPackageItemToBitmapSourceConverter(IconManager iconManager)
		{
			IconManager = iconManager;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var item = value as DiffPackageItem;
			if (!int.TryParse(parameter as string, out int size))
				size = 32;

			if (item.IsFolder())
				return IconManager.GetFolderImage(size);
			else
				return IconManager.GetImageForPart(item.Part.Left ?? item.Part.Right, size);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
