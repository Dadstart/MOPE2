using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;

namespace B4.Mope
{
	public class DiffPackageItem
	{
		public DiffData DiffData{ get; }
		public string Path { get; }

		private Dictionary<string, DiffPackageItem> m_children;
		public IReadOnlyCollection<DiffPackageItem> Children => m_children?.Values;
		public DiffPart Part { get; }

		public bool IsFolder() => Part == null;

		public DiffPackageItem(DiffData diffData, string path, DiffPart part)
		{
			DiffData = diffData ?? throw new ArgumentNullException(nameof(diffData));
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
