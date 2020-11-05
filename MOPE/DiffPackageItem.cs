using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope
{
	public class DiffPackageItem
	{
		public DiffPackage Package { get; }
		public string Path { get; }
		public IList<DiffPackageItem> Children { get; set; }
		public DiffPart Part { get; }

		public bool IsFolder() => Part == null;

		public DiffPackageItem(DiffPackage package, string path, DiffPart part)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Path = path ?? throw new ArgumentNullException(nameof(path));
			Part = part;
		}
	}
}
