using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Win32;

namespace B4.Mope.Shell
{
	public abstract class ShellCommands
	{
		private static ShellCommand ShellCommandFromVerb(string application, string progId, string shellRegKeyPath, string verb)
		{
			ShellCommand shellCommand = null;
			RegistryKey regKey = null;
			RegistryKey ddeKey = null;
			RegistryKey verbKey = null;

			try
			{
				string command;

				verbKey = Registry.ClassesRoot.OpenSubKey(String.Concat(shellRegKeyPath, '\\', verb));
				if (verbKey == null)
					return null;

				regKey = verbKey.OpenSubKey("command");
				if (regKey == null)
					return null;

				command = RegistryHelper.StringValueFromRegKey(regKey, string.Empty);
				regKey.Close();
				regKey = null;

				if (command == null)
					return null;

				shellCommand = new ShellCommand(application, progId, verb, null, command, null, null, null);

				// get DDE (if it exists)
				ddeKey = verbKey.OpenSubKey(@"ddeexec");

				if (ddeKey != null)
				{
					// get DDE command
					shellCommand.DDE = RegistryHelper.StringValueFromRegKey(ddeKey, string.Empty);

					// get DDE application (if it exists)
					shellCommand.DDEApplication = RegistryHelper.StringValueFromRegKey(ddeKey, @"\application");

					// get DDE topic (if it exists)
					shellCommand.DDETopic = RegistryHelper.StringValueFromRegKey(ddeKey, @"\topic");
				}

				return shellCommand;
			}
			catch { }

			if (regKey != null)
				regKey.Close();

			if (verbKey != null)
				verbKey.Close();

			if (ddeKey != null)
				ddeKey.Close();

			return shellCommand;
		}
		public static ShellCommand ApplicationOpenCommand(string application)
		{
			// HKCR\Applications\[app.exe]\shell\open
			ShellCommand  shellCommand = ShellCommandFromVerb(application, null, String.Concat(@"Applications\", application, @"\shell"), "open");
			if (shellCommand != null)
				return shellCommand;
			// HKCR\Applications\[app.exe]\shell\edit
			return ShellCommandFromVerb(application, null, String.Concat(@"Applications\", application, @"\shell"), "edit");
		}
		public static ShellCommand ProgIdOpenCommand(string progId)
		{
			// HKCR\[progid]\shell\open
			ShellCommand shellCommand = ShellCommandFromVerb(null, progId, String.Concat(progId, @"\shell"), "open");
			if (shellCommand != null)
				return shellCommand;
			// HKCR\[progid]\shell\edit
			return ShellCommandFromVerb(null, progId, String.Concat(progId, @"\shell"), "edit");
		}

	}
}
