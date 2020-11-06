using B4.Mope.Packaging;
using B4.Mope.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace B4.Mope.Shell
{
	/// <summary>
	/// Shell data for opening files
	/// </summary>
	public class ShellOpenWithData
	{
		private NameValueCollection m_extensionApplications = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
		private IDictionary<string, ShellCommand> m_applicationCommands = new Dictionary<string, ShellCommand>(StringComparer.OrdinalIgnoreCase);
		private IDictionary<string, string> m_progIdApplications = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private IDictionary<string, ShellCommand> m_progIdCommands = new Dictionary<string, ShellCommand>(StringComparer.OrdinalIgnoreCase);
		private NameValueCollection m_extensionProgIds = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

		public IDictionary<string, string> ApplicationFullPaths { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private void LoadShellCommandsForExtension(string extension)
		{
			extension = extension.ToLowerInvariant();

			LoadApplicationShellCommands(extension);
			LoadProgIdShellCommands(extension);
		}

		/// <summary>
		/// Load shell commands for applications registered for this extension
		/// </summary>
		/// <param name="extension"></param>
		private void LoadApplicationShellCommands(string extension)
		{
			// see if we've already loaded commands for this extension
			if (m_extensionApplications.GetValues(extension) != null)
				return;

			var apps = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(string.Concat(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\", extension, @"\OpenWithList"));

			if ((regKey != null) && (regKey.ValueCount > 0))
			{
				foreach (string name in regKey.GetValueNames())
				{
					string value;
					if (name.Equals("MRUList"))
						continue;

					if (regKey.GetValueKind(name) != RegistryValueKind.String)
						continue;

					value = (string)regKey.GetValue(name, string.Empty);
					if (value.Equals(string.Empty))
						continue;

					apps.Add(value);
				}
			}

			string[] subKeys = RegistryHelper.GetSubKeyList(Registry.ClassesRoot, string.Concat(extension, @"\OpenWithList"));
			apps.AddValues(subKeys);

			if (apps.Count == 0)
			{
				m_extensionApplications.Add(extension, string.Empty);
				return;
			}

			// look up shell command for each potential app
			foreach (string app in apps)
			{
				ShellCommand command;
				string applicationName;
				string applicationFullName = null;

				applicationName = app.ToLowerInvariant();
				if (applicationName.Equals(string.Empty))
					continue;

				// duplicate?
				if ((m_extensionApplications[extension] != null) && m_extensionApplications[extension].Contains(applicationName))
					continue;

				m_extensionApplications.Add(extension, applicationName);

				// check if commands for this app were already loaded
				if (m_applicationCommands.ContainsKey(applicationName))
					continue;

				command = ShellCommands.ApplicationOpenCommand(applicationName);

				if (command == null)
					continue;

				m_applicationCommands.Add(applicationName, command);

				if (ApplicationFullPaths.ContainsKey(applicationName))
					continue;

				// search for app in registry
				applicationFullName = RegistryHelper.StringValueFromRegKey(Registry.LocalMachine, string.Concat(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\", applicationName), null);
				if (applicationFullName != null)
				{
					ApplicationFullPaths.Add(applicationName, applicationFullName.ToLowerInvariant());
					continue;
				}

				// look in command
				applicationFullName = command.GetApplication();
				if (applicationFullName != null)
				{
					ApplicationFullPaths.Add(applicationName, applicationFullName.ToLowerInvariant());
					continue;
				}
			}

			return;
		}

		/// <summary>
		/// Get all shell commands for a part
		/// </summary>
		public IList<ShellCommand> GetCommandsForPart(Part part)
		{
			var shellCommands = new List<ShellCommand>();

			// load shell commands based on part's extension
			var extension = GetPartExtension(part);

			// ensure shell commands have been loaded
			LoadShellCommandsForExtension(extension);

			// iterate through apps list based on this part's extension
			var partApps = m_extensionApplications.GetValues(extension);
			if (partApps != null)
			{
				foreach (var app in partApps)
				{
					if (string.IsNullOrEmpty(app))
						continue;

					if (!ApplicationFullPaths.TryGetValue(app, out string appFullPath) || string.IsNullOrEmpty(appFullPath))
						continue;

					if (!m_applicationCommands.TryGetValue(app, out ShellCommand commmand))
						continue;

					shellCommands.Add(commmand);
				}
			}

			// iterate through prog ids registered for this part's extension
			var progIds = m_extensionProgIds.GetValues(extension);
			if (progIds != null)
			{
				foreach (var progId in progIds)
				{
					if (string.IsNullOrEmpty(progId))
						continue;

					if (!m_progIdCommands.TryGetValue(progId, out ShellCommand command))
						continue;

					shellCommands.Add(command);
				}
			}

			return shellCommands;
		}

		/// <summary>
		/// Get the effective extension for a part
		/// </summary>
		private string GetPartExtension(Part part)
		{
			if (ContentTypes.IsXmlType(part.ContentType))
				return ".xml";
			else
				return part.GetFileInfo().Extension;
		}

		/// <summary>
		/// Load shell commands for progids registered for this extension
		/// </summary>
		/// <param name="extension"></param>
		private void LoadProgIdShellCommands(string extension)
		{
			// see if we've already loaded commands for this extension
			if (m_extensionProgIds[extension] != null)
				return;

			var progIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			// first search HKCU explorer registered file extensions
			var subKeys = RegistryHelper.GetValueNames(Registry.CurrentUser, string.Concat(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\", extension, @"\OpenWithProgIds"));
			progIds.AddValues(subKeys);

			// check HKCR registered file extensions
			subKeys = RegistryHelper.GetValueNames(Registry.ClassesRoot, string.Concat(extension, @"\OpenWithProgids"));
			progIds.AddValues(subKeys);

			if (progIds.Count == 0)
			{
				m_extensionProgIds.Add(extension, string.Empty);
				return;
			}

			// look up shell command for each potential progid
			foreach (string progId in progIds)
			{
				ShellCommand command;
				bool foundApp = false;
				string app = null;

				m_extensionProgIds.Add(extension, progId);

				if (m_progIdCommands.ContainsKey(progId))
				{
					// commands for this progId were already loaded
					continue;
				}

				command = ShellCommands.ProgIdOpenCommand(progId);
				if (command == null)
					continue;

				if (!m_progIdApplications.ContainsKey(progId))
				{
					// find application for this progId
					app = command.GetApplication();

					if (app != null)
					{
						FileInfo fi = new FileInfo(app);
						if (fi.Exists)
						{
							app = fi.FullName.ToLowerInvariant();
							foundApp = true;
						}

						if (!foundApp)
						{
							// search for app in registry
							app = RegistryHelper.StringValueFromRegKey(Registry.LocalMachine, string.Concat(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\", app), null);
							if (app == null)
							{
								// now do a brute force search on the hard disk
								app = FileHelper.FindApplication(command.GetApplication(), true);
								if (app == null)
									continue;
							}
							app = app.ToLowerInvariant();
							foundApp = true;
						}
					}

					if (foundApp)
						m_progIdApplications.Add(progId, app);
				}

				m_progIdCommands.Add(progId, command);
			}

			return;
		}
	}
}
