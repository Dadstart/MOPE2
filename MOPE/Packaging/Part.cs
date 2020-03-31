using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace B4.Mope.Packaging
{
	public class Part
	{
		public Package Package { get; }
		public string Name { get; }
		public string Uri { get; }
		public string ContentType { get; }
		public uint Crc32 { get; }
		public Relationships Relationships { get; set; }

		public Part(Package package, string name, string uri, string contentType, uint crc32)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Name = string.IsNullOrEmpty(name) ? throw new ArgumentNullException(nameof(name)) : name;
			Uri = string.IsNullOrEmpty(uri) ? throw new ArgumentNullException(nameof(uri)) : uri;
			ContentType = string.IsNullOrEmpty(contentType) ? throw new ArgumentNullException(nameof(contentType)) : contentType;
			Crc32 = crc32;
		}

		public FileInfo GetFileInfo()
		{
			var fullPath = Path.Combine(Package.TempDirectory, Uri.Replace('/', '\\'));
			return new FileInfo(fullPath);
		}
	}
}
