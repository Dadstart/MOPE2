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
        /// <summary>
        /// Shell Commands - index is app name or progId
        /// </summary>
		private Dictionary<string, ShellCommand> applicationCommands;
		private Dictionary<string, ShellCommand> progIdCommands;
		private NameValueCollection extensionProgIds;
        private NameValueCollection extensionApplications;
        private StringDictionary applicationFullPaths;
		private StringDictionary progIdApplications;

        public StringDictionary ApplicationFullPaths { get { return applicationFullPaths; } }
		public StringDictionary ProgIdApplications { get { return progIdApplications; } }

		public Dictionary<string, ShellCommand> ApplicationCommands
        {
            get
            {
                return applicationCommands;
            }
        }
		public Dictionary<string, ShellCommand> ProgIdCommands
		{
			get
			{
				return progIdCommands;
			}
		}

		public NameValueCollection ExtensionProgIds
        {
            get
            {
				return extensionProgIds;
            }

        }

		public NameValueCollection ExtensionApplications
        {
            get
            {
                return extensionApplications;
            }

        }


        public OpenWith()
        {
			progIdCommands = new Dictionary<string, ShellCommand>();
			applicationCommands = new Dictionary<string, ShellCommand>();
			applicationFullPaths = new StringDictionary();
			progIdApplications = new StringDictionary();
            extensionProgIds = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            extensionApplications = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
        }

        public void LoadShellCommandsForExtension(string extension)
        {
			if (extensionApplications.Get(extension) == null)
				LoadApplicationShellCommands(extension.ToLowerInvariant());
            //if (extensionProgIds.Get(extension) == null)
			//	LoadProgIdShellCommands(extension.ToLowerInvariant());
        }

		private void LoadProgIdShellCommands(string extension)
		{
			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(String.Concat(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\", extension, @"\OpenWithProgIds"));

			if ((regKey == null) || (regKey.ValueCount == 0))
			{
				extensionProgIds.Add(extension, String.Empty);
				return;
			}

			if (regKey.ValueCount == 0)
				return;

			foreach (string progId in regKey.GetValueNames())
			{
				ShellCommand command;
				bool foundApp = false;
				string app = null;

				extensionProgIds.Add(extension, progId);

				if (progIdCommands.ContainsKey(progId))
				{
					// commands for this progId were already loaded
					continue;
				}

				command = ShellCommands.ProgIdOpenCommand(progId);
				if (command == null)
					continue;

				if (!progIdApplications.ContainsKey(progId))
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
							app = RegistryHelper.StringValueFromRegKey(Registry.LocalMachine, String.Concat(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\", app), null);
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
						progIdApplications[progId] = app;
				}

				progIdCommands.Add(progId, command);
			}

			return;
		}

		private void LoadApplicationShellCommands(string extension)
		{
			List<string> apps = new List<string>();

			RegistryKey regKey = Registry.CurrentUser.OpenSubKey(String.Concat(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\", extension, @"\OpenWithList"));

			if ((regKey != null) && (regKey.ValueCount > 0))
			{
				foreach (string name in regKey.GetValueNames())
				{
					string value;
					if (name.Equals("MRUList"))
						continue;

					if (regKey.GetValueKind(name) != RegistryValueKind.String)
						continue;

					value = (string)regKey.GetValue(name, String.Empty);
					if (value.Equals(String.Empty))
						continue;

					apps.Add(value);
				}
			}

			string[] subKeys = RegistryHelper.GetSubKeyList(Registry.ClassesRoot, String.Concat(extension, @"\OpenWithList"));
			if ((subKeys != null) && (subKeys.Length > 0))
			{
				apps.AddRange(subKeys);
			}

			if (apps.Count == 0)
			{
				extensionApplications.Add(extension, String.Empty);
				return;
			}

			// now we have a list of all potentials
			foreach (string app in apps)
			{
				ShellCommand command;
				string applicationName;
				string applicationFullName = null;

				applicationName = app.ToLowerInvariant();
				if (applicationName.Equals(String.Empty))
					continue;

				// duplicate?
				if ((extensionApplications[extension] != null) && extensionApplications[extension].Contains(applicationName))
					continue;

				extensionApplications.Add(extension, applicationName);

				// check if commands for this app were already loaded
				if (applicationCommands.ContainsKey(applicationName))
					continue;

				command = ShellCommands.ApplicationOpenCommand(applicationName);

				if (command == null)
					continue;

				applicationCommands.Add(applicationName, command);

				if (applicationFullPaths.ContainsKey(applicationName))
					continue;

				// search for app in registry
				applicationFullName = RegistryHelper.StringValueFromRegKey(Registry.LocalMachine, String.Concat(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\", applicationName), null);
				if (applicationFullName != null)
				{
					applicationFullPaths[applicationName] = applicationFullName.ToLowerInvariant();
					continue;
				}

				// look in command
				applicationFullName = command.GetApplication();
				if (applicationFullName != null)
				{
					applicationFullPaths[applicationName] = applicationFullName.ToLowerInvariant();
					continue;
				}
			}

			return;
		}

		/*		private void LoadShellCommandApplicationsRegistry(string extension)
        {
            string[] openWithApplications = RegistryHelper.GetSubKeyList(Registry.ClassesRoot, String.Concat(extension, @"\OpenWithList"));
            if ((openWithApplications == null) || (openWithApplications.Length == 0))
                return;

			for (int k = 0; k < openWithApplications.Length; k++)
			{
				string applicationName = openWithApplications[k].ToLowerInvariant();
                ShellCommands shellCommandsApp = new ShellCommands();
                bool foundApp = false;
                string applicationFullName = null;

                if (this.applicationFullPaths.ContainsKey(applicationName)
                    && this.appShellCommands.ContainsKey(this.applicationFullPaths[applicationName]))
                {
                    // commands for this app were already loaded
                    this.extensionApplications.Add(extension, applicationName);
                    continue;
                }


                shellCommandsApp.LoadForApplication(applicationName);

                // get the application full name and path
                if (shellCommandsApp.Commands.Count == 0)
                    continue;

                foreach (ShellCommand shellCommand in shellCommandsApp.Commands)
                {
                    if (this.applicationFullPaths.ContainsKey(applicationName))
                    {
						applicationFullName = this.applicationFullPaths[applicationName];
                        foundApp = true;
                    }
                    else
                    {
						applicationFullName = shellCommand.GetApplication();
						if (applicationFullName == null)
                            continue;

						this.applicationFullPaths[applicationName] = applicationFullName;
                        foundApp = true;
                        break;
                    }
                }

                for (int i = 0; i < shellCommandsApp.Commands.Count; i++)
                {
                    int j = i + 1;
                    ShellCommand shci = shellCommandsApp.Commands[i];

                    while (j < shellCommandsApp.Commands.Count)
                    {
                        ShellCommand shcj = shellCommandsApp.Commands[j];

                        if (shci.Command.ToLower().Equals(shcj.Command.ToLower()))
                            shellCommandsApp.Commands.RemoveAt(j);
                        else
                            j++;
                    }
                }

                extensionApplications.Add(extension, applicationName);

                if (!foundApp)
                {
					applicationFullName = FileHelper.FindApplication(applicationName, true);
					if (applicationFullName == null)
                        continue;
                }

                this.applicationFullPaths[applicationName] = applicationFullName;
                shellCommandsApp.Application = applicationFullName;

                this.appShellCommands.Add(shellCommandsApp.Application, shellCommandsApp);
            }

            return;
        }
		*/

/*		private void LoadShellCommandProgIdsExplorer(string extension)
		{
			RegistryKey regKey;
			StringCollection progIds;

			regKey = Registry.CurrentUser.OpenSubKey(String.Concat(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\", extension, @"\OpenWithProgIds"));

			if (regKey == null)
				return;

			if (regKey.ValueCount == 0)
				return;

			progIds = new StringCollection();

			foreach (string progId in regKey.GetValueNames())
			{
				// only add valid progIds
				if (Registry.ClassesRoot.OpenSubKey(progId) == null)
					continue;

				progIds.Add(progId);
			}


			foreach (string progId in progIds)
			{
				ShellCommands shellCommandsProgId = new ShellCommands();

				shellCommandsProgId.LoadForProgId(progId);

				if ((shellCommandsProgId == null) || (shellCommandsProgId.Commands.Count == 0))
					continue;

				// iterate through each shellCommand and save it
				foreach (ShellCommand shellCommand in shellCommandsProgId.Commands)
				{
					string applicationName;
					string applicationFullName;
					ShellCommands shellCommandsT;
					bool exists = false;
					bool dupe = false;
					string[] extensionApps;

					applicationFullName = shellCommand.GetApplication();
					if (applicationFullName == null)
						continue;
					applicationName = new FileInfo(applicationFullName).Name.ToLowerInvariant();

					extensionApps = this.extensionApplications.GetValues(extension);
					if (extensionApps != null)
					{
						bool found = false;
						foreach (string app in extensionApps)
						{
							if (app.Equals(applicationName))
							{
								found = true;
								break;
							}
						}
						if (!found)
							extensionApplications.Add(extension, applicationName);
					}
					else
					{
						extensionApplications.Add(extension, applicationName);
					}

					// save shellCommands under shellCommands indexed by application
					if (applicationFullName != null)
					{
						this.applicationFullPaths[applicationName] = applicationFullName;

						if (this.appShellCommands.ContainsKey(applicationFullName))
						{
							shellCommandsT = this.appShellCommands[applicationFullName];
							exists = true;
						}
						else
						{
							shellCommandsT = new ShellCommands();
							shellCommandsT.ProgId = progId;
							shellCommandsT.Application = applicationFullName;
							exists = false;
						}

						// check for a dupe shellCommand
						dupe = false;
						if (shellCommandsT.Commands.Count > 0)
						{
							foreach (ShellCommand shellCommandT in shellCommandsT.Commands)
							{
								if (shellCommandT.Command.ToLower().Equals(shellCommand.Command.ToLower()))
								{
									dupe = true;
									break;
								}
							}
						}

						if (dupe)
							continue;

						// save this shellCommand
						shellCommandsT.Commands.Add(shellCommand);

						// we just createed the ShellCommands for the app, now add it to our list of shell commands indexed by application
						if (!exists)
							this.appShellCommands.Add(applicationFullName, shellCommandsT);
					}

					// save shellCommands under shellCommands indexed by progId

					if (this.progIdShellCommands.ContainsKey(progId))
					{
						shellCommandsT = this.progIdShellCommands[progId];
						exists = true;
					}
					else
					{
						shellCommandsT = new ShellCommands();
						shellCommandsT.ProgId = progId;
						if (applicationFullName != null)
						{
							this.applicationFullPaths[applicationName] = applicationFullName;
							shellCommandsT.Application = applicationFullName;
						}
						exists = false;
					}

					// check for a dupe shellCommand
					dupe = false;
					if (shellCommandsT.Commands.Count > 0)
					{
						foreach (ShellCommand shellCommandT in shellCommandsT.Commands)
						{
							if (shellCommandT.Command.ToLower().Equals(shellCommand.Command.ToLower()))
							{
								dupe = true;
								break;
							}
						}
					}

					if (dupe)
						continue;

					// save this shellCommand
					shellCommandsT.Commands.Add(shellCommand);

					// we just create the ShellCommands for the progId, add it to our list of shell commands indexed by progId
					if (!exists)
						this.progIdShellCommands.Add(progId, shellCommandsT);

					extensionProgIds.Add(extension, progId);
				}
			}

			return;
		}
		*/
		
		/*		private void LoadShellCommandProgIdsRegistry(string extension)
        {
            StringCollection allProgIds;
            string[] progIds;
            string extensionProgId = RegistryHelper.ProgIdFromExtension(extension);

            allProgIds = new StringCollection();

            progIds = RegistryHelper.GetSubKeyList(Registry.ClassesRoot, String.Concat(extension, @"\OpenWithProgIds"));

			if ((progIds != null) && (progIds.Length > 0))
				allProgIds.AddRange(progIds);
			
			if ((extensionProgId != null) && (!extensionProgId.Equals(String.Empty) && !allProgIds.Contains(extensionProgId)))
                allProgIds.Add(extensionProgId);

            progIds = RegistryHelper.GetSubKeyList(Registry.ClassesRoot, String.Concat(extension));
            foreach (string progId in progIds)
            {
                // not all the subkeys of HKCR\[.ext] are progIds (ex. OpenWithList, ShellNew, etc.)
                // only add valid progIds
                if (Registry.ClassesRoot.OpenSubKey(progId) == null)
                    continue;

                if (!allProgIds.Contains(progId))
                    allProgIds.Add(progId);
            }


            foreach (string progId in allProgIds)
            {
                ShellCommands shellCommandsProgId = new ShellCommands();

                shellCommandsProgId.LoadForProgId(progId);

                if ((shellCommandsProgId == null) || (shellCommandsProgId.Commands.Count == 0))
                    continue;

                // iterate through each shellCommand and save it
                foreach (ShellCommand shellCommand in shellCommandsProgId.Commands)
                {
                    string applicationName;
                    string applicationFullName;
                    ShellCommands shellCommandsT;
                    bool exists = false;
                    bool dupe = false;
                    string[] extensionApps;

                    applicationFullName = shellCommand.GetApplication();
                    applicationName = (new FileInfo(applicationFullName)).Name.ToLowerInvariant();

                    extensionApps = this.extensionApplications.GetValues(extension);
                    if (extensionApps != null)
                    {
                        bool found = false;
                        foreach (string app in extensionApps)
                        {
                            if (app.Equals(applicationName))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            extensionApplications.Add(extension, applicationName);
                    }
                    else
                    {
                        extensionApplications.Add(extension, applicationName);
                    }

                    // save shellCommands under shellCommands indexed by application
					if (applicationFullName != null)
                    {
                        this.applicationFullPaths[applicationName] = applicationFullName;

                        if (this.appShellCommands.ContainsKey(applicationFullName))
                        {
                            shellCommandsT = this.appShellCommands[applicationFullName];
                            exists = true;
                        }
                        else
                        {
                            shellCommandsT = new ShellCommands();
                            shellCommandsT.ProgId = progId;
                            shellCommandsT.Application = applicationFullName;
                            exists = false;
                        }

                        // check for a dupe shellCommand
                        dupe = false;
                        if (shellCommandsT.Commands.Count > 0)
                        {
                            foreach (ShellCommand shellCommandT in shellCommandsT.Commands)
                            {
                                if (shellCommandT.Command.ToLower().Equals(shellCommand.Command.ToLower()))
                                {
                                    dupe = true;
                                    break;
                                }
                            }
                        }

                        if (dupe)
                            continue;

                        // save this shellCommand
                        shellCommandsT.Commands.Add(shellCommand);

                        // we just createed the ShellCommands for the app, now add it to our list of shell commands indexed by application
                        if (!exists)
                            this.appShellCommands.Add(applicationFullName, shellCommandsT);
                    }

                    // save shellCommands under shellCommands indexed by progId

                    if (this.progIdShellCommands.ContainsKey(progId))
                    {
                        shellCommandsT = this.progIdShellCommands[progId];
                        exists = true;
                    }
                    else
                    {
                        shellCommandsT = new ShellCommands();
                        shellCommandsT.ProgId = progId;
						if (applicationFullName != null)
                        {
                            this.applicationFullPaths[applicationName] = applicationFullName;
                            shellCommandsT.Application = applicationFullName;
                        }
                        exists = false;
                    }

                    // check for a dupe shellCommand
                    dupe = false;
                    if (shellCommandsT.Commands.Count > 0)
                    {
                        foreach (ShellCommand shellCommandT in shellCommandsT.Commands)
                        {
                            if (shellCommandT.Command.ToLower().Equals(shellCommand.Command.ToLower()))
                            {
                                dupe = true;
                                break;
                            }
                        }
                    }

                    if (dupe)
                        continue;

                    // save this shellCommand
                    shellCommandsT.Commands.Add(shellCommand);

                    // we just create the ShellCommands for the progId, add it to our list of shell commands indexed by progId
                    if (!exists)
                        this.progIdShellCommands.Add(progId, shellCommandsT);

                    extensionProgIds.Add(extension, progId);
                }
            }

            return;
        } */

    }
}
