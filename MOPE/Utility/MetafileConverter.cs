using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace B4.Mope.Utility
{
	public static class MetafileConverter
	{
		public static void CopyToImage(Stream source, Stream target, ImageFormat format)
		{
			using (var metafile = new Metafile(source))
			using (var bitmap = new Bitmap(metafile))
			{
				bitmap.Save(target, format);
			}
		}
	}
}
