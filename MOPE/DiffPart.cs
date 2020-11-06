using B4.Mope.Packaging;
using B4.Mope.Shell;
using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope
{
	public class DiffPart
	{
		public string Uri { get; }
		public Part Left { get; }
		public Part Right { get; }
		public IList<ShellCommand> ShellCommands { get; }

		public DiffPart(string uri, Part left, Part right, IList<ShellCommand> shellCommands)
		{
			// must have at least one part
			if ((left == null) && (right == null))
			{
				throw new ArgumentNullException();
			}
			Uri = uri ?? throw new ArgumentNullException(nameof(uri));
			Left = left;
			Right = right;
			ShellCommands = shellCommands ?? throw new ArgumentNullException(nameof(shellCommands));
		}
	}
}
