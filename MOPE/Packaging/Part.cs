using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace B4.Mope.Packaging
{
	public class Part
	{
		public Package Package { get; }
		public Uri Uri { get; }
		public string ContentType { get; }
		public uint Crc32 { get; }

		public Part(Package package, string uri, string contentType, uint crc32)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			ContentType = string.IsNullOrEmpty(contentType) ? throw new ArgumentNullException(nameof(contentType)) : contentType;
			Uri = string.IsNullOrEmpty(uri) ? throw new ArgumentNullException(nameof(uri)) : new Uri(uri);
			Crc32 = crc32;
		}

	}
}
