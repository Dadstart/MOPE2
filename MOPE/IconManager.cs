using B4.Mope.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace B4.Mope
{
	public class IconManager
	{
		public Icon UnknownIcon { get; }
		public Icon RelsIcon { get; }
		public Icon ContentTypesIcon { get; }
		public Icon FolderIcon { get; }
		public IconManager()
		{
			// load known icons
			UnknownIcon = LoadKnownIcon("unknown");
		}

		private Icon LoadKnownIcon(string v)
		{
			var stream = Application.GetResourceStream(new Uri("pack://application:,,,/MOPE;component/icons/unknown.ico")).Stream;
			return new System.Drawing.Icon(stream);
		}

		public Icon GetIconForContentType(string contentType)
		{
			switch (contentType)
			{
				default:
					return UnknownIcon;
			}
		}

		public Bitmap GetImageForContentType(string contentType)
		{
			var icon = GetIconForContentType(contentType);
			return Bitmap.FromHicon(icon.Handle);
		}
	}
}
