using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace B4.Mope
{
	public class BitmapCollection<T>
	{
		Dictionary<int, T> bitmaps = new Dictionary<int, T>();

		public void Add(T bitmap, int size)
		{
			bitmaps.Add(size, bitmap);
		}

		public bool TryGetValue(int size, out T source)
		{
			return bitmaps.TryGetValue(size, out source);
		}

		public T Get(int size)
		{
			if (!bitmaps.TryGetValue(size, out T source))
			{
				// fallback to something, but throw if there's no bitmap at all
				source = bitmaps.Values.First();
			}

			return source;
		}
	}
}
