using B4.Mope.Packaging;
using B4.Mope.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace B4.Mope
{
	public class PartModel
	{
		public Part Part { get; }
		public bool IsDirty { get; private set; } = false;
		public IList<ShellCommand> ShellCommands { get; }

		public PartModel(Part part, IList<ShellCommand> shellCommands)
		{
			Part = part ?? throw new ArgumentNullException(nameof(part));
			ShellCommands = shellCommands ?? throw new ArgumentNullException(nameof(shellCommands));

			// only app path shell commands have been validated - only keep those
			ShellCommands = shellCommands.Where(c => !string.IsNullOrEmpty(c.Application)).ToList();
		}

		public delegate void DirtyChangedEventHandler(object sender, EventArgs e);
		public event DirtyChangedEventHandler DirtyChanged;
		public void SetDirty(bool isDirty)
		{
			if (IsDirty == isDirty)
				return;

			IsDirty = isDirty;
			DirtyChanged?.Invoke(this, new EventArgs());
		}
	}
}
