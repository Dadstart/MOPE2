using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace B4.Mope
{
	public class BitmapSourceCollection
	{
		Dictionary<int, BitmapSource> bitmaps = new Dictionary<int, BitmapSource>();

		public void Add(BitmapSource bitmap, int size)
		{
			bitmaps.Add(size, bitmap);
		}

		public BitmapSource Get(int size)
		{
			if (!bitmaps.TryGetValue(size, out BitmapSource source))
			{
				// fallback to something, but throw if there's no bitmap at all
				source = bitmaps.Values.First();
			}

			return source;
		}
	}
}
