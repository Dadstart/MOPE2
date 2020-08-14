using System.Collections.Generic;

namespace B4.Mope
{
	public class PackageItem
	{
		public string Name { get; }
		public string FullName { get; }
		public PartModel Model { get; }
		public IList<PackageItem> Children { get; set; }

		public PackageItem(string name, string fullName, PartModel model)
		{
			Name = name;
			FullName = fullName;
			Model = model;
		}

		public bool IsFolder() => Model?.Part == null;
	}
}
