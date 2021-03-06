﻿using System;
using System.Diagnostics;
using System.IO;

namespace B4.Mope.Packaging
{
	[DebuggerDisplay("{Name} ({Uri})")]
	public class Part
	{
		public Package Package { get; }
		public string Name { get; }
		public string Uri { get; }
		public string ContentType { get; }
		public uint Crc32 { get; }
		public long Size { get; }
		public long CompressedSize { get; }
		public Relationships Relationships { get; set; }

		public Part(Package package, string name, string uri, string contentType, uint crc32, long size, long compressedSize)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Name = string.IsNullOrEmpty(name) ? throw new ArgumentNullException(nameof(name)) : name;
			Uri = string.IsNullOrEmpty(uri) ? throw new ArgumentNullException(nameof(uri)) : uri;
			ContentType = string.IsNullOrEmpty(contentType) ? throw new ArgumentNullException(nameof(contentType)) : contentType;
			Crc32 = crc32;
			Size = size;
			CompressedSize = compressedSize;
		}

		public FileInfo GetFileInfo()
		{
			var fullPath = Path.Combine(Package.TempDirectory, Uri.Replace('/', '\\'));
			return new FileInfo(fullPath);
		}
	}
}
