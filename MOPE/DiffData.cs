using B4.Mope.Packaging;
using System;
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

		public DiffPackage DiffPackage { get; private set; }

		public bool IsLoaded => (Left != null) && (Right != null);

		public void LoadLeft(string path)
		{
			Left = new Package(path, GetTempDir());

			if (IsLoaded)
				OnLoaded();
		}

		private void OnLoaded()
		{
			DiffPackage = new DiffPackage(Left, Right);
			DiffsLoaded?.Invoke(this, new EventArgs());
		}

		public void LoadRight(string path)
		{
			Right = new Package(path, GetTempDir());

			if (IsLoaded)
				OnLoaded();
		}

		private string GetTempDir()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "MOPE2", Guid.NewGuid().ToString());
			if (!Directory.Exists(tempDir))
				Directory.CreateDirectory(tempDir);
			return tempDir;
		}
	}
}