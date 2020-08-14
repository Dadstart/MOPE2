using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace B4.Mope
{
	public class IconManager
	{
		[DllImport("shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern uint ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr hiconLarge, out IntPtr hiconSmall, uint nIcons);

		[DllImport("user32.dll", EntryPoint = "DestroyIcon", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern bool DestroyIcon(IntPtr hicon);

		public BitmapCollection<BitmapSource> UnknownIcon { get; }
		public BitmapCollection<BitmapSource> RelsIcon { get; }
		public BitmapCollection<BitmapSource> ContentTypesIcon { get; }
		public BitmapCollection<BitmapSource> FolderIcon { get; }
		public BitmapCollection<BitmapSource> AudioIcon { get; }
		public BitmapCollection<BitmapSource> VideoIcon { get; }
		public BitmapCollection<BitmapSource> ImageIcon { get; }
		public BitmapCollection<BitmapSource> CodeIcon { get; }
		public BitmapCollection<BitmapSource> XmlIcon { get; }
		public BitmapCollection<BitmapSource> DocumentIcon { get; }
		public BitmapCollection<BitmapSource> WordIcon { get; }
		public BitmapCollection<BitmapSource> ExcelIcon { get; }
		public BitmapCollection<BitmapSource> PptIcon { get; }
		public BitmapCollection<BitmapSource> VisioIcon { get; }
		public IDictionary<string, BitmapCollection<BitmapSource>> AppIcons { get; } = new Dictionary<string, BitmapCollection<BitmapSource>>(StringComparer.OrdinalIgnoreCase);

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
		private BitmapCollection<BitmapSource> LoadKnownIcon(string id)
		{
			using (var stream = Application.GetResourceStream(new Uri($"pack://application:,,,/MOPE;component/icons/{id}.ico")).Stream)
			{

				// load 16, 32, and 256 sizes
				var collection = new BitmapCollection<BitmapSource>();

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

		public BitmapCollection<BitmapSource> GetIconForContentType(string contentType)
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
					return null;

				var envVar = appFullPath.Substring(i + 1, j - 1);
				var envVarValue = Environment.GetEnvironmentVariable(envVar);
				appFullPath = appFullPath.Replace($"%{envVar}%", envVarValue);
			}

			// check cache
			if (AppIcons.TryGetValue(appFullPath, out BitmapCollection<BitmapSource> bitmaps))
			{
				if (bitmaps.TryGetValue(iconSize, out BitmapSource cached))
					return cached;
			}

			// load from app (dll or exe)
			var bitmapImage = GetBitmapImageFromApp(appFullPath, small: true);
			if (bitmapImage == null)
				return null;

			if (bitmaps == null)
			{
				bitmaps = new BitmapCollection<BitmapSource>();
				AppIcons.Add(appFullPath, bitmaps);
			}

			// store in cache
			bitmaps.Add(bitmapImage, iconSize);

			return bitmapImage;
		}

		private BitmapSource GetBitmapImageFromApp(string appFullPath, bool small)
		{
			IntPtr hiconSmall = IntPtr.Zero;
			IntPtr hiconLarge = IntPtr.Zero;
			Icon icon = null;
			try
			{
				if (ExtractIconEx(appFullPath, 0, out hiconLarge, out hiconSmall, 1) == 0)
					return null;

				var hicon = small ? hiconSmall : hiconLarge;
				if (hicon == IntPtr.Zero)
					return null;

				icon = Icon.FromHandle(hicon);
				int size = small ? 16 : 32;
				var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(size, size));
				return bitmapSource;
			}
#pragma warning disable CS0168 // Variable is declared but never used
			catch (Exception exc)
#pragma warning restore CS0168 // Variable is declared but never used
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
