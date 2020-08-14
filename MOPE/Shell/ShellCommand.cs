using System;
using System.Runtime.InteropServices;

namespace B4.Mope.Shell
{
	public class ShellCommand
	{
		public string Application { get; set; }
		public string ProgId { get; set; }
		public string Verb { get; }
		public string FriendlyName { get; }
		public string Command { get; set; }
		public string DDE { get; set; }
		public string DDEApplication { get; set; }
		public string DDETopic { get; set; }

		public string DisplayText
		{
			get
			{
				string displayText = Verb;

				displayText = String.Concat(Verb.Substring(0, 1).ToUpper(), Verb.Substring(1));
				if (FriendlyName != null)
				{
					displayText = FriendlyName;
					int amp = displayText.IndexOf('&');
					if (amp >= 0)
						displayText = displayText.Remove(amp, 1);
				}
				return displayText;
			}
		}

		public ShellCommand(string application, string progId, string verb, string friendlyName, string command, string dde, string ddeApplication, string ddeTopic)
		{
			Application = application;
			ProgId = progId;
			FriendlyName = friendlyName;
			Verb = verb;
			if ((Verb != null) && (Verb.Length > 1))
				Verb = String.Concat(Verb.ToUpper()[0], Verb.Substring(1));
			Command = command;
			DDE = dde;
			DDEApplication = ddeApplication;
			DDETopic = ddeTopic;
		}

		public string GetApplication()
		{
			try
			{
				int i;

				if (Command[0] == '"')
				{
					i = Command.IndexOf('"', 1);
					if (i > 1)
						return Command.Substring(1, i - 1);
				}
				else
				{
					i = Command.IndexOf(' ');
					if (i > 0)
						return Command.Substring(0, i);
				}
			}
			catch { }

			return null;
		}


		[DllImport("shell32.dll", EntryPoint = "ShellExecuteW", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

	}
}
