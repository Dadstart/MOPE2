using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace B4.Mope.UI
{
	public class DiffPackageItemToBackgroundConverter : IValueConverter
	{
		static Color s_addColor = Color.FromRgb(255, 255, 128);
		static Color s_delColor = Color.FromRgb(255, 119, 119);
		static Color s_changeColor = Color.FromRgb(255, 187, 124);
		static Color s_sameColor = Color.FromRgb(255, 255, 255);

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var item = value as DiffPackageItem;

			Color? color;
			if (item.IsFolder())
			{
				//bool allAdd = true, allDel = true, hasChange = false;

				//foreach (var child in item.Children)
				//{
				//	if (child.Part.Left == null)
				//	{
				//		hasChange = true;
				//		allDel = false;
				//	}
				//	else if (child.Part.Right == null)
				//	{
				//		hasChange = true;
				//		allAdd = false;
				//	}
				//	else if (child.Part.Left.Crc32 != child.Part.Right.Crc32)
				//	{
				//		hasChange = true;
				//		allDel = false;
				//		allAdd = false;
				//	}
				//	else
				//	{
				//		allDel = false;
				//		allAdd = false;
				//	}
				//}

				//if (allAdd)
				//	color = s_addColor;
				//else if (allDel)
				//	color = s_delColor;
				//else if (hasChange)
				//	color = s_changeColor;
				//else
				//	color = null;
				color = null;
			}
			else if (item.Part.Left == null)
			{
				color = s_addColor;
			}
			else if (item.Part.Right == null)
			{
				color = s_delColor;
			}
			else if (item.Part.Left.Crc32 != item.Part.Right.Crc32)
			{
				color = s_changeColor;
			}
			else
			{
				color = null;
			}

			if (color == null)
				return new SolidColorBrush();
			else
				return new SolidColorBrush((Color)color);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
