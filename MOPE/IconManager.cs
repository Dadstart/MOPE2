using B4.Mope.Packaging;
using B4.Mope.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace B4.Mope
{
	public class IconManager
	{
		BitmapSourceCollection UnknownIcon;
		BitmapSourceCollection RelsIcon;
		BitmapSourceCollection ContentTypesIcon;
		BitmapSourceCollection FolderIcon;

		public IconManager()
		{
			// load known icons
			UnknownIcon = LoadKnownIcon("unknown");
			RelsIcon = LoadKnownIcon("rels");
			ContentTypesIcon = LoadKnownIcon("magnifying glass");
			FolderIcon = LoadKnownIcon("folder");
		}

		internal object GetFolderImage(int size)
		{
			return FolderIcon.Get(size);
		}

		/// <summary>
		/// Loads a known icon from the app resources
		/// </summary>
		private BitmapSourceCollection LoadKnownIcon(string id)
		{
			using (var stream = Application.GetResourceStream(new Uri($"pack://application:,,,/MOPE;component/icons/{id}.ico")).Stream)
			{

				// load 16, 32, and 256 sizes
				var collection = new BitmapSourceCollection();

				collection.Add(GetBitmapSourceFromIconStream(stream, 16), 16);
				stream.Seek(0, SeekOrigin.Begin);

				collection.Add(GetBitmapSourceFromIconStream(stream, 32), 32);
				stream.Seek(0, SeekOrigin.Begin);

				collection.Add(GetBitmapSourceFromIconStream(stream, 64), 64);

				return collection;
			}
		}

		/// <summary>
		/// Load an icon from a stream
		/// </summary>
		private Icon GetIconFromStream(Stream stream, int size)
		{
			return new Icon(stream, new System.Drawing.Size(size, size));
		}

		/// <summary>
		/// Creates a BitmapSource from an icon stream
		/// </summary>
		private BitmapSource GetBitmapSourceFromIconStream(Stream stream, int size)
		{
			using (var icon = GetIconFromStream(stream, size))
			{
				return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(size, size));
			}
		}

		/// <summary>
		/// Get image based on content type
		/// </summary>
		public BitmapSource GetImageForContentType(string contentType, int size)
		{
			switch (contentType)
			{
				case "application/vnd.openxmlformats-package.relationships+xml":
					return RelsIcon.Get(size);
				default:
					return UnknownIcon.Get(size);
			}
		}

		/// <summary>
		/// Get image for a part, either a special icon for known parts or based on content-type
		/// </summary>
		public BitmapSource GetImageForPart(Part part, int size)
		{
			// [Content Types].xml is special
			if (string.Equals(part?.Name, "[Content_Types].xml", StringComparison.Ordinal))
				return ContentTypesIcon.Get(size);

			return GetImageForContentType(part?.ContentType, size);
		}
	}
}
