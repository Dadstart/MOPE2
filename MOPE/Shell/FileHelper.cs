using Microsoft.Win32;
using System;
using System.IO;

namespace B4.Mope.Shell
{
	public static class FileHelper
	{
		public static FileInfo FindFile(string filename, string searchPath, bool recursive)
		{
			try
			{
				FileInfo[] files;
				DirectoryInfo di;
				DirectoryInfo[] subDirs;

				di = new DirectoryInfo(searchPath);
				if (!di.Exists)
					return null;

				files = di.GetFiles(filename);

				if ((files != null) && (files.Length > 0))
				{
					return files[0];
				}

				if (!recursive)
					return null;

				subDirs = di.GetDirectories();
				if ((subDirs == null) || (subDirs.Length == 0))
					return null;

				foreach (DirectoryInfo subDir in di.GetDirectories())
				{
					FileInfo fi = FindFile(filename, subDir.FullName, true);
					if ((fi != null) && fi.Exists)
						return fi;
				}
			}
			catch { }

			return null;
		}

		public static string FindApplication(string applicationName, bool searchAll)
		{
			FileInfo application = null;
			string paths;
			string path;
			string windowsDir = Environment.GetEnvironmentVariable("windir");

			paths = Environment.GetEnvironmentVariable("path");

			while ((paths != null) && !paths.Equals(string.Empty))
			{
				int i = paths.IndexOf(";");
				if (i < 0)
					i = paths.Length;

				path = paths.Substring(0, i);

				// don't search windows directory, we'll do that manually later
				if ((windowsDir == null) || (!path.StartsWith(windowsDir)))
				{

					application = FindFile(applicationName, path, true);
					if ((application != null) && application.Exists)
						return application.FullName.ToLowerInvariant();
				}
				if (i + 1 < paths.Length)
					paths = paths.Substring(i + 1);
				else
					paths = null;
			}

			// look in windows directory
			if ((windowsDir != null) && !windowsDir.Equals(string.Empty))
			{
				application = FindFile(applicationName, windowsDir, true);
				if ((application != null) && application.Exists)
					return application.FullName.ToLowerInvariant();
			}

			// look in common program files directory
			application = FindFile(applicationName, Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), true);
			if ((application != null) && application.Exists)
				return application.FullName.ToLowerInvariant();

			// look in  program files directory
			application = FindFile(applicationName, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), true);
			if ((application != null) && application.Exists)
				return application.FullName.ToLowerInvariant();

			// look in my computer directory
			if (searchAll)
			{
				return null;
			}

			// no joy
			return null;
		}

		public static string DisplayNameForApplication(string application, string appFullPath)
		{
			try
			{
				// check the cache in the registry
				if (application != null)
				{
					string name = RegistryHelper.StringValueFromRegKey(Registry.CurrentUser, @"HKEY_CURRENT_USER\Software\Microsoft\Windows\ShellNoRoam\MUICache", application);
					if (name != null)
						return name;
				}

				FileInfo fi = new FileInfo(appFullPath);
				if (fi.Exists)
				{
					System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(appFullPath);
					return fvi.FileDescription;
				}
				else
				{
					return fi.Name;
				}
			}
			catch { }
			return application;
		}
	}
}
