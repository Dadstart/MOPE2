using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope
{
	public class DiffPackage
	{
		public Package Left { get; }
		public Package Right { get; }

		public IDictionary<string, DiffPackageItem> Folders { get; } = new Dictionary<string, DiffPackageItem>();
		public IDictionary<string, DiffPart> Parts { get; } = new Dictionary<string, DiffPart>();

		public DiffPackage(Package left, Package right)
		{
			Left = left ?? throw new ArgumentNullException(nameof(left));
			Right = right ?? throw new ArgumentNullException(nameof(right));

			ReadPackages();
		}

		private void ReadPackages()
		{
			// add all parts in Left and matching parts in Right
			foreach (var leftPart in Left.Parts.Values)
			{
				Part rightPart = null;

				Right.Parts.TryGetValue(leftPart.Uri, out rightPart);
				var diffPart = new DiffPart(leftPart.Uri, leftPart, rightPart);
				Parts.Add(diffPart.Uri, diffPart);
			}

			// ad all parts in Right not already added
			foreach (var rightPart in Right.Parts.Values)
			{
				// skip parts already added
				if (Parts.ContainsKey(rightPart.Uri))
					continue;

				var diffPart = new DiffPart(rightPart.Uri, left: null, right: rightPart);
				Parts.Add(diffPart.Uri, diffPart);
			}

			CreateFolders();
		}

		private void CreateFolders()
		{
			foreach (var diffPart in Parts.Values)
			{
				var path = diffPart.Uri;

				// format is folder/partfilename.ext
				var folderEnd = path.LastIndexOf('/');
				string folderPath;
				if (folderEnd == -1)
				{
					folderPath = string.Empty;
				}
				else
				{
					folderPath = path.Substring(0, folderEnd);
				}
				var partPath = path.Substring(folderEnd + 1, path.Length - (folderEnd + 1));

				// get existing folder or create new one
				DiffPackageItem folder;
				if (!Folders.TryGetValue(folderPath, out folder))
				{
					folder = new DiffPackageItem(this, folderPath, null);
					Folders.Add(folderPath, folder);
				}

				folder.AddItem(new DiffPackageItem(this, partPath, diffPart));
			}
		}
	}
}
