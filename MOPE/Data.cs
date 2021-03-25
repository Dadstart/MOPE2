using B4.Mope.Packaging;
using B4.Mope.Shell;
using B4.Mope.Utility;
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
		public string BackupCopy { get; private set; }
		public bool BackupCopyOwned { get; set; }

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
			DeleteTempFiles();
			// leave Shell related fields the same to use as cache for future opens
		}

		private void CreateBackupCopy()
		{
			var original = new FileInfo(Package.ZipFile);
			var tempName = Temp.GetTempName("zip", out string tempFullPath);

			int extIndex = original.Name.LastIndexOf('.');
			var baseName = original.Name.Substring(0, extIndex);
			
			BackupCopy = Path.Combine(Temp.GetRootTempDir(), $"{baseName} ({tempName}){original.Extension}");
			File.Copy(Package.ZipFile, BackupCopy);
			BackupCopyOwned = true;
		}

		public void Init(string path)
		{
			TempDirectory = Temp.GetTempDir();
			Package = new Package(path, TempDirectory);

			// create a copy for possible diffing later
			CreateBackupCopy();

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
					// dispose managed state (managed objects)
					Package?.Close();
					DeleteTempFiles();
					WebHost.Dispose();
				}

				m_isDisposed = true;
			}
		}

		private void DeleteTempFiles()
		{
			if (!string.IsNullOrEmpty(TempDirectory) && Directory.Exists(TempDirectory))
			{
				Directory.Delete(TempDirectory, recursive: true);
				TempDirectory = null;
			}

			if (BackupCopyOwned && File.Exists(BackupCopy))
			{
				BackupCopyOwned = false;
				File.Delete(BackupCopy);
				BackupCopy = null;
			}
		}

		public void SaveAs(string filename)
		{
			PackageWatcher.EnableRaisingEvents = false;

			Package.SaveAs(filename);

			IsPackageDirty = false;

			PackageWatcher.EnableRaisingEvents = true;
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
