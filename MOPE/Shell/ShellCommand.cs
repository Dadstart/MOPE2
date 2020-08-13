using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace B4.Mope.Shell
{
    public class ShellCommand
    {
        private string verb;
        private string friendlyName;
        private string command;
        private string dde;
        private string ddeApplication;
        private string ddeTopic;
		private string application;
		private string progId;

		public string Application { get { return application; } set { application = value; } }
		public string ProgId { get { return progId; } set { progId = value; } }
		public string Verb { get { return verb; } }
        public string FriendlyName { get { return friendlyName; } }
        public string Command { get { return command; } set { command = value; } }
        public string DDE { get { return dde; } set { dde = value; } }
        public string DDEApplication { get { return ddeApplication; } set { ddeApplication = value; } }
        public string DDETopic { get { return ddeTopic; } set { ddeTopic = value; } }

        public string DisplayText
        {
            get
            {
                string displayText = verb;

                displayText = String.Concat(verb.Substring(0, 1).ToUpper(), verb.Substring(1));
                if (friendlyName != null)
                {
                    displayText = friendlyName;
                    int amp = displayText.IndexOf('&');
                    if (amp >= 0)
                        displayText = displayText.Remove(amp, 1);
                }
                return displayText;


            }
        }
        public ShellCommand(string application, string progId, string verb, string friendlyName, string command, string dde, string ddeApplication, string ddeTopic)
        {
			this.application = application;
			this.progId = progId;
            this.friendlyName = friendlyName;
            this.verb = verb;
            if ((this.verb != null) && (this.verb.Length > 1))
                this.verb = String.Concat(this.verb.ToUpper()[0], this.verb.Substring(1));
            this.command = command;
            this.dde = dde;
            this.ddeApplication = ddeApplication;
            this.ddeTopic = ddeTopic;
        }

		public string GetApplication()
		{
			try
			{
				int i;

				if (command[0] == '"')
				{
					i = command.IndexOf('"', 1);
					if (i > 1)
						return command.Substring(1, i - 1);
				}
				else
				{
					i = command.IndexOf(' ');
					if (i > 0)
						return command.Substring(0, i);
				}
			}
			catch { }

			return null;
		}


		[DllImport("shell32.dll", EntryPoint = "ShellExecuteW", CharSet = CharSet.Unicode, ExactSpelling = true)]
		public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

    }
}
