using B4.Mope.Packaging;
using B4.Mope.Shell;
using System;

namespace B4.Mope
{
	/// <summary>
	/// Model object for shell command menus
	/// </summary>
	public class ShellCommandMenuModel
	{
		public ShellCommand Command { get; }
		public Part Part { get; }
		public ShellOpenWithData OpenWith { get; }

		public ShellCommandMenuModel(ShellCommand shellCommand, Part part, ShellOpenWithData openWith)
		{
			Command = shellCommand ?? throw new ArgumentNullException(nameof(shellCommand));
			Part = part ?? throw new ArgumentNullException(nameof(part));
			OpenWith = openWith ?? throw new ArgumentNullException(nameof(openWith));
		}
	}
}
