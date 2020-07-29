using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope.Packaging
{
	public class PackageItem
	{
		public string Name { get; }
		public string FullName { get; }
		public Part Part { get; }
		public IList<PackageItem> Children { get; set; }

		public PackageItem(string name, string fullName, Part part)
		{
			Name = name;
			FullName = fullName;
			Part = part;
		}

		public bool IsFolder() => Part == null;
	}
}
