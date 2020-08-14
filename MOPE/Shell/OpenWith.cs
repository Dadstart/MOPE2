using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Specialized;

namespace B4.Mope.Shell
{
	public class OpenWith
	{
		public IDictionary<string, string> ApplicationFullPaths { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		public IDictionary<string, string> ProgIdApplications { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		public IDictionary<string, ShellCommand> ApplicationCommands { get; } = new Dictionary<string, ShellCommand>(StringComparer.OrdinalIgnoreCase);
		public IDictionary<string, ShellCommand> ProgIdCommands { get; } = new Dictionary<string, ShellCommand>(StringComparer.OrdinalIgnoreCase);


		public NameValueCollection ExtensionProgIds { get; } = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
		public NameValueCollection ExtensionApplications { get; } = new NameValueCollection(StringComparer.OrdinalIgnoreCase);

		public void LoadShellCommandsForExtension(string extension)
		{
			extension = extension.ToLowerInvariant();

			var apps = ExtensionApplications.GetValues(extension);

			// if we've already loaded values for this extension move on
			if (apps != null)
				return;

			LoadApplicationShellCommands(extension);
		}

		private void LoadProgIdShellCommands(string extension)
		{
			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(string.Concat(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\", extension, @"\OpenWithProgIds"));

			if ((regKey == null) || (regKey.ValueCount == 0))
			{
				ExtensionProgIds.Add(extension, string.Empty);
				return;
			}

			if (regKey.ValueCount == 0)
				return;

			foreach (string progId in regKey.GetValueNames())
			{
				ShellCommand command;
				bool foundApp = false;
				string app = null;

				ExtensionProgIds.Add(extension, progId);

				if (ProgIdCommands.ContainsKey(progId))
				{
					// commands for this progId were already loaded
					continue;
				}

				command = ShellCommands.ProgIdOpenCommand(progId);
				if (command == null)
					continue;

				if (!ProgIdApplications.ContainsKey(progId))
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
						ProgIdApplications.Add(progId, app);
				}

				ProgIdCommands.Add(progId, command);
			}

			return;
		}

		private void LoadApplicationShellCommands(string extension)
		{
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
			if (subKeys != null)
			{
				foreach (var subKey in subKeys)
					apps.Add(subKey);
			}

			if (apps.Count == 0)
			{
				ExtensionApplications.Add(extension, string.Empty);
				return;
			}

			// now we have a list of all potentials
			foreach (string app in apps)
			{
				ShellCommand command;
				string applicationName;
				string applicationFullName = null;

				applicationName = app.ToLowerInvariant();
				if (applicationName.Equals(string.Empty))
					continue;

				// duplicate?
				if ((ExtensionApplications[extension] != null) && ExtensionApplications[extension].Contains(applicationName))
					continue;

				ExtensionApplications.Add(extension, applicationName);

				// check if commands for this app were already loaded
				if (ApplicationCommands.ContainsKey(applicationName))
					continue;

				command = ShellCommands.ApplicationOpenCommand(applicationName);

				if (command == null)
					continue;

				ApplicationCommands.Add(applicationName, command);

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
	}
}
