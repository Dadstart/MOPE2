using B4.Mope.Packaging;
using B4.Mope.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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
		[DllImport("shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern uint ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr hiconLarge, out IntPtr hiconSmall, uint nIcons);

		[DllImport("user32.dll", EntryPoint = "DestroyIcon", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool DestroyIcon(IntPtr hicon);

		public BitmapSourceCollection UnknownIcon { get; }
		public BitmapSourceCollection RelsIcon { get; }
		public BitmapSourceCollection ContentTypesIcon { get; }
		public BitmapSourceCollection FolderIcon { get; }
		public BitmapSourceCollection AudioIcon { get; }
		public BitmapSourceCollection VideoIcon { get; }
		public BitmapSourceCollection ImageIcon { get; }
		public BitmapSourceCollection CodeIcon { get; }
		public BitmapSourceCollection XmlIcon { get; }
		public BitmapSourceCollection DocumentIcon { get; }
		public BitmapSourceCollection WordIcon { get; }
		public BitmapSourceCollection ExcelIcon { get; }
		public BitmapSourceCollection PptIcon { get; }
		public BitmapSourceCollection VisioIcon { get; }
		public IDictionary<string, BitmapSourceCollection> AppIcons { get; } = new Dictionary<string, BitmapSourceCollection>(StringComparer.OrdinalIgnoreCase);

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

		public BitmapSource GetImageForApplicationName(string appFullPath)
		{
			const int iconSize = 16;

			// normalize case and trim quotes
			appFullPath = appFullPath.ToLowerInvariant().Trim('\"', '\'');

			// expand environment variables
			int i = 0;
			while ((i = appFullPath.IndexOf('%', i)) >= 0)
			{
				int j = appFullPath.IndexOf('%', i + 1);

				// uh-oh - registry may be corrupt
				if (j <= i)
					return UnknownIcon.Get(16);

				var envVar = appFullPath.Substring(i + 1, j - 1);
				var envVarValue = Environment.GetEnvironmentVariable(envVar);
				appFullPath = appFullPath.Replace($"%{envVar}%", envVarValue);
			}

			// check cache
			if (AppIcons.TryGetValue(appFullPath, out BitmapSourceCollection bitmaps))
			{
				if (bitmaps.TryGetValue(iconSize, out BitmapSource cached))
					return cached;
			}

			// load from app (dll or exe)
			var bitmapSource = GetBitmapSourceFromApp(appFullPath, small: true);
			if (bitmapSource == null)
				return UnknownIcon.Get(16);

			if (bitmaps == null)
			{
				bitmaps = new BitmapSourceCollection();
				AppIcons.Add(appFullPath, bitmaps);
			}

			// store in cache
			bitmaps.Add(bitmapSource, iconSize);

			return bitmapSource;
		}

		private BitmapSource GetBitmapSourceFromApp(string appFullPath, bool small)
		{
			IntPtr hiconSmall = IntPtr.Zero;
			IntPtr hiconLarge = IntPtr.Zero;
			Icon icon = null;
			try
			{
				if (ExtractIconEx(appFullPath, 0, out hiconLarge, out hiconSmall, 1) == 0)
					return null;

				icon = Icon.FromHandle(small ? hiconSmall : hiconLarge);
				int size = small ? 16 : 32;
				return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(size, size));
			}
			catch
			{
				// yes, we're swallowing exceptions. don't want to crash in this code path
				if (Debugger.IsAttached)
					Debugger.Break();
				return null;
			}
			finally
			{
				if (hiconSmall != IntPtr.Zero)
					DestroyIcon(hiconSmall);
				if (hiconLarge != IntPtr.Zero)
					DestroyIcon(hiconLarge);
				icon?.Dispose();
			}
		}
	}
}
