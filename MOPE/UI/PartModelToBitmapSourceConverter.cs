using System;
using System.Globalization;
using System.Windows.Data;

namespace B4.Mope.UI
{
	public class PartModelToBitmapSourceConverter : IValueConverter
	{
		public IconManager IconManager { get; }

		public PartModelToBitmapSourceConverter()
		{
			IconManager = MainWindow.IconManager;
		}

		public PartModelToBitmapSourceConverter(IconManager iconManager)
		{
			IconManager = iconManager;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var model = value as PartModel;

			if (!int.TryParse(parameter as string, out int size))
				size = 32;

			if (model == null)
				return IconManager.UnknownIcon.Get(size);

			return IconManager.GetImageForPart(model?.Part, size);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
