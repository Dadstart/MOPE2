using B4.Mope.Packaging;
using B4.Mope.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace B4.Mope
{
	/// <summary>
	/// Data holder for app
	/// </summary>
	public class Data : DependencyObject, IDisposable
	{
		private bool m_isDisposed;

		public Package Package { get; private set; }
		public FileSystemWatcher PackageWatcher { get; private set; }
		public WebHost WebHost { get; private set; }

		public IDictionary<string, PartModel> PartModels { get; private set; }

		public ShellOpenWithData OpenWith { get; private set; } = new ShellOpenWithData();
		public IList<string> Applications { get; private set; } = new List<string>();
		public List<PackageItem> Items { get; private set; }
		public Settings Settings { get; } = new Settings();
		public string TempDirectory { get; private set; }


		public bool IsPackageDirty { get; internal set; }

		public bool IgnoringChanges { get; set; }

		public Data()
		{
		}

		public void OnReadOnlyModeChanged()
		{

		}

		public void Reset()
		{
			Package?.Close();
			Package = null;
			WebHost?.Pause();
			PartModels = null;
			Items = null;
			PackageWatcher?.Dispose();
			PackageWatcher = null;
			IgnoringChanges = false;
			// leave Shell related fields the same to use as cache for future opens
		}

		private string GetTempDir()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "MOPE2", Guid.NewGuid().ToString());
			if (!Directory.Exists(tempDir))
				Directory.CreateDirectory(tempDir);
			return tempDir;
		}

		public void Init(string path)
		{
			TempDirectory = GetTempDir();
			Package = new Package(path, TempDirectory);

			// create a copy for possible diffing later
			var fileInfo = new FileInfo(path);
			File.Copy(path, Path.Combine(TempDirectory, fileInfo.Name));

			InitializePackageWatcher();
			InitializeWebHost();
			InitializePartModels();
			InitializeHierarchyTree();
		}

		private void InitializePackageWatcher()
		{
			var fileInfo = new FileInfo(Package.ZipFile);
			PackageWatcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name)
			{
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
				EnableRaisingEvents = true,
			};
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

		private void InitializePartModels()
		{
			PartModels = new Dictionary<string, PartModel>(Package.Parts.Count);
			foreach (Part part in Package.Parts.Values)
			{
				var shellCommands = OpenWith.GetCommandsForPart(part);
				var model = new PartModel(part, shellCommands);
				PartModels.Add(part.Uri.Replace('\\','/'), model);
			}
		}

		private void InitializeHierarchyTree()
		{
			// generate items hierarchy tree
			var rootItems = new SortedList<string, PackageItem>();
			var tempDir = new DirectoryInfo(Package.TempDirectory);
			foreach (var info in tempDir.GetFileSystemInfos())
			{
				var item = PackagePartFromFileSystemInfo(info);
				rootItems.Add(item.Name, item);
			}
			Items = rootItems.Values.ToList();
		}

		private PackageItem PackagePartFromFileSystemInfo(FileSystemInfo info)
		{
			var relativePath = System.IO.Path.GetRelativePath(Package.TempDirectory, info.FullName);
			relativePath = relativePath.Replace('\\', '/');

			var dir = info as DirectoryInfo;

			if (dir == null)
			{
				return new PackageItem(info.Name, info.FullName, PartModels[relativePath]);
			}
			else
			{
				var packageItem = new PackageItem(info.Name, info.FullName, null);
				var childrenItems = new SortedList<string, PackageItem>();
				foreach (var childInfo in dir.GetFileSystemInfos())
				{
					var childItem = PackagePartFromFileSystemInfo(childInfo);
					childrenItems.Add(childItem.Name, childItem);
				}

				packageItem.Children = childrenItems.Values.ToList();
				return packageItem;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_isDisposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					if (!string.IsNullOrEmpty(TempDirectory) && Directory.Exists(TempDirectory))
					{
						TempDirectory = null;
						Directory.Delete(TempDirectory, recursive: true);
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				m_isDisposed = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~Data()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
