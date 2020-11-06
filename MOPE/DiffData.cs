using B4.Mope.Packaging;
using B4.Mope.Shell;
using System;
using System.Collections.Generic;
using System.IO;

namespace B4.Mope
{
	// TODO: De-dup with Data class
	public class DiffData
	{
		public delegate void LoadedEventHandler(object sender, EventArgs e);
		public event LoadedEventHandler DiffsLoaded;

		public Package Left { get; private set; }
		public Package Right { get; private set; }
		public IDictionary<string, DiffPackageItem> Folders { get; } = new Dictionary<string, DiffPackageItem>();
		public IDictionary<string, DiffPart> Parts { get; } = new Dictionary<string, DiffPart>();
		public Settings Settings { get; } = new Settings();
		public WebHost WebHost { get; private set; }

		public bool IsLoaded => (Left != null) && (Right != null);

		public ShellOpenWithData OpenWith { get; }

		public DiffData(ShellOpenWithData openWith)
		{
			OpenWith = openWith;
		}

		public void LoadLeft(string path)
		{
			Left = new Package(path, GetTempDir());

			if (IsLoaded)
				OnLoaded();
		}

		public void LoadRight(string path)
		{
			Right = new Package(path, GetTempDir());

			if (IsLoaded)
				OnLoaded();
		}

		private void OnLoaded()
		{
			ReadPackages();
			DiffsLoaded?.Invoke(this, new EventArgs());

			InitializeWebHost();
		}

		private void ReadPackages()
		{
			// add all parts in Left and matching parts in Right
			foreach (var leftPart in Left.Parts.Values)
			{
				Part rightPart = null;

				Right.Parts.TryGetValue(leftPart.Uri, out rightPart);
				var diffPart = new DiffPart(leftPart.Uri, leftPart, rightPart, OpenWith.GetCommandsForPart(leftPart));
				Parts.Add(diffPart.Uri, diffPart);
			}

			// ad all parts in Right not already added
			foreach (var rightPart in Right.Parts.Values)
			{
				// skip parts already added
				if (Parts.ContainsKey(rightPart.Uri))
					continue;

				var diffPart = new DiffPart(rightPart.Uri, left: null, right: rightPart, OpenWith.GetCommandsForPart(rightPart));
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

		private string GetTempDir()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "MOPE2", Guid.NewGuid().ToString());
			if (!Directory.Exists(tempDir))
				Directory.CreateDirectory(tempDir);
			return tempDir;
		}

		private void InitializeWebHost()
		{
			if (WebHost == null)
			{
				WebHost = new WebHost(this);
				WebHost.ListenOnThread();
			}
			else
			{
				WebHost.Resume();
			}
		}

	}
}