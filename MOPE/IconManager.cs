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
		readonly BitmapSourceCollection UnknownIcon;
		readonly BitmapSourceCollection RelsIcon;
		readonly BitmapSourceCollection ContentTypesIcon;
		readonly BitmapSourceCollection FolderIcon;
		readonly BitmapSourceCollection AudioIcon;
		readonly BitmapSourceCollection VideoIcon;
		readonly BitmapSourceCollection ImageIcon;
		readonly BitmapSourceCollection CodeIcon;
		readonly BitmapSourceCollection XmlIcon;
		readonly BitmapSourceCollection DocumentIcon;
		private readonly BitmapSourceCollection WordIcon;
		private readonly BitmapSourceCollection ExcelIcon;
		private readonly BitmapSourceCollection PptIcon;
		private readonly BitmapSourceCollection VisioIcon;

		public IconManager()
		{
			// load known icons
			UnknownIcon = LoadKnownIcon("unknown");
			RelsIcon = LoadKnownIcon("rels");
			ContentTypesIcon = LoadKnownIcon("magnifying glass");
			FolderIcon = LoadKnownIcon("folder");
			AudioIcon = LoadKnownIcon("audio");
			VideoIcon = LoadKnownIcon("video");
			ImageIcon = LoadKnownIcon("image");
			CodeIcon = LoadKnownIcon("code");
			XmlIcon = LoadKnownIcon("xml");
			DocumentIcon = LoadKnownIcon("document");
			WordIcon = LoadKnownIcon("word");
			ExcelIcon = LoadKnownIcon("excel");
			PptIcon = LoadKnownIcon("ppt");
			VisioIcon = LoadKnownIcon("visio");
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
			return GetIconForContentType(contentType).Get(size);
		}

		public BitmapSourceCollection GetIconForContentType(string contentType)
		{
			if (string.Equals(contentType, "application/vnd.openxmlformats-package.relationships+xml", StringComparison.Ordinal))
				return RelsIcon;

			if (ContentTypes.IsSupportedAudioType(contentType))
				return AudioIcon;

			if (ContentTypes.IsSupportedImageType(contentType))
				return ImageIcon;

			if (ContentTypes.IsSupportedVideoType(contentType))
				return VideoIcon;

			if (ContentTypes.IsXmlType(contentType))
				return XmlIcon;

			if (ContentTypes.IsXmlType(contentType))
				return CodeIcon;

			switch (contentType)
			{
				case "application/rtf":
				case "message/rfc822":
					return DocumentIcon;
				case "application/vnd.openxmlformats-officedocument.vmlDrawing":
					return CodeIcon;
				case "application/msword":
				case "application/vnd.ms-word.document.macroEnabled.12":
				case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
					return WordIcon;
				case "application/vnd.ms-powerpoint":
				case "application/vnd.ms-powerpoint.presentation.macroEnabled.12":
				case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
				case "application/vnd.openxmlformats-package.relationships+xml":
				case "application/vnd.ms-powerpoint.slide.macroEnabled.12":
				case "application/vnd.openxmlformats-officedocument.presentationml.slide":
					return PptIcon;
				case "application/vnd.visio":
				case "application/vnd.ms-visio.drawing.macroEnabled":
				case "application/vnd.ms-visio.drawing":
					return VisioIcon;
				case "application/vnd.ms-excel":
				case "application/vnd.ms-excel.sheet.binary.macroEnabled.12":
				case "application/vnd.ms-excel.sheet.macroEnabled.12":
				case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
					return ExcelIcon;
				default:
					return UnknownIcon;
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
