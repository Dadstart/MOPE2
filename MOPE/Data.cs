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
		public Package Package { get; private set; }
		public FileSystemWatcher PackageWatcher { get; private set; }
		public WebHost WebHost { get; private set; }

		public IDictionary<string, PartModel> PartModels { get; private set; }

		public OpenWith OpenWith { get; private set; } = new OpenWith();
		public IList<string> Applications { get; private set; } = new List<string>();
		public List<PackageItem> Items { get; private set; }
		internal AppSettings Settings { get; } = new AppSettings();
		public string TempDirectory { get; private set; }

		public delegate void BooleanPropertyChangedEventHandler(object sender, BooleanPropertyChangedEventArgs e);

		public event BooleanPropertyChangedEventHandler EditorReadOnlyModeChanged;
		private static readonly DependencyProperty EditorReadOnlyModeProperty = DependencyProperty.Register("EditorReadOnlyMode", typeof(bool), typeof(Data), new PropertyMetadata(false, EditorReadOnlyPropertyChanged));
		public bool EditorReadOnlyMode
		{
			get { return (bool)GetValue(EditorReadOnlyModeProperty); }
			set { SetValue(EditorReadOnlyModeProperty, value); }
		}

		private static void EditorReadOnlyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var data = (Data)obj;
			data.Settings.EditorReadOnlyMode = (bool)e.NewValue;
			data.Settings.Save();
			data.EditorReadOnlyModeChanged?.Invoke(data, new BooleanPropertyChangedEventArgs((bool)e.OldValue, (bool)e.NewValue));
		}

		public event BooleanPropertyChangedEventHandler EditorDarkModeChanged;
		private static readonly DependencyProperty EditorDarkModeProperty = DependencyProperty.Register("EditorDarkMode", typeof(bool), typeof(Data), new PropertyMetadata(false, EditorDarkPropertyChanged));
		public bool EditorDarkMode
		{
			get { return (bool)GetValue(EditorDarkModeProperty); }
			set { SetValue(EditorDarkModeProperty, value); }
		}
		private static void EditorDarkPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var data = (Data)obj;
			data.Settings.EditorUseDarkMode = (bool)e.NewValue;
			data.Settings.Save();
			data.EditorDarkModeChanged?.Invoke(data, new BooleanPropertyChangedEventArgs((bool)e.OldValue, (bool)e.NewValue));
		}

		private static readonly DependencyProperty ConfirmOverwritePackageProperty = DependencyProperty.Register("ConfirmOverwritePackage", typeof(bool), typeof(Data), new PropertyMetadata(false, ConfirmOverwritePackagePropertyChanged));
		public bool ConfirmOverwritePackage
		{
			get { return (bool)GetValue(ConfirmOverwritePackageProperty); }
			set { SetValue(ConfirmOverwritePackageProperty, value); }
		}
		private static void ConfirmOverwritePackagePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var data = (Data)obj;
			data.Settings.ConfirmOverwritePackage = (bool)e.NewValue;
			data.Settings.Save();
		}

		private static readonly DependencyProperty EditorFormatXmlOnLoadProperty = DependencyProperty.Register("EditorFormatXmlOnLoad", typeof(bool), typeof(Data), new PropertyMetadata(false, EditorFormatXmlOnLoadPropertyChanged));
		private bool m_isDisposed;

		public bool EditorFormatXmlOnLoad
		{
			get { return (bool)GetValue(EditorFormatXmlOnLoadProperty); }
			set { SetValue(EditorFormatXmlOnLoadProperty, value); }
		}
		private static void EditorFormatXmlOnLoadPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var data = (Data)obj;
			data.Settings.EditorFormatXmlOnLoad = (bool)e.NewValue;
			data.Settings.Save();
		}

		public bool IsPackageDirty { get; internal set; }

		public Data()
		{
			EditorReadOnlyMode = Settings.EditorReadOnlyMode;
			EditorDarkMode = Settings.EditorUseDarkMode;
			ConfirmOverwritePackage = Settings.ConfirmOverwritePackage;
			EditorFormatXmlOnLoad = Settings.EditorFormatXmlOnLoad;
		}

		public void OnReadOnlyModeChanged()
		{

		}

		public void Reset()
		{
			Package?.Close();
			Package = null;
			WebHost?.Stop();
			PartModels = null;
			Items = null;
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
				WebHost = new WebHost(this);
			WebHost.ListenOnThread();
		}

		private void InitializePartModels()
		{
			PartModels = new Dictionary<string, PartModel>(Package.Parts.Count);
			foreach (Part part in Package.Parts.Values)
			{
				var shellCommands = LoadShellCommandsForPart(part);
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


		private IList<ShellCommand> LoadShellCommandsForPart(Part part)
		{
			var shellCommands = new List<ShellCommand>();

			// load shell commands based on part's extension
			var extension = part.GetFileInfo().Extension;

			// override if it's an xml file
			if (ContentTypes.IsXmlType(part.ContentType))
				extension = ".xml";

			OpenWith.LoadShellCommandsForExtension(extension);

			// iterate through apps list based on this part's extension
			var partApps = OpenWith.ExtensionApplications.GetValues(extension);
			if (partApps != null)
			{
				foreach (var app in partApps)
				{
					if (string.IsNullOrEmpty(app))
						continue;

					if (!OpenWith.ApplicationFullPaths.TryGetValue(app, out string appFullPath) || string.IsNullOrEmpty(appFullPath))
						continue;

					if (!OpenWith.ApplicationCommands.TryGetValue(app, out ShellCommand commmand))
						continue;

					shellCommands.Add(commmand);
				}
			}

			// iterate through prog ids registered for this part's extension
			var progIds = OpenWith.ExtensionProgIds.GetValues(extension);
			if (progIds != null)
			{
				foreach (var progId in progIds)
				{
					if (string.IsNullOrEmpty(progId))
						continue;

					if (!OpenWith.ProgIdCommands.TryGetValue(progId, out ShellCommand command))
						continue;

					shellCommands.Add(command);
				}
			}

			return shellCommands;
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
