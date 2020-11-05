using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;

namespace B4.Mope
{
	public class DiffPackageItem
	{
		public DiffPackage Package { get; }
		public string Path { get; }

		private Dictionary<string, DiffPackageItem> m_children;
		public IReadOnlyCollection<DiffPackageItem> Children => m_children?.Values;
		public DiffPart Part { get; }

		public bool IsFolder() => Part == null;

		public DiffPackageItem(DiffPackage package, string path, DiffPart part)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Path = path ?? throw new ArgumentNullException(nameof(path));
			Part = part;
		}

		public void AddItem(DiffPackageItem item)
		{
			if (!IsFolder())
				throw new InvalidOperationException("Can't add children to parts");

			if (m_children == null)
				m_children = new Dictionary<string, DiffPackageItem>(StringComparer.OrdinalIgnoreCase);

			m_children.Add(item.Path, item);
		}
	}
}
