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
		public PartModel PartModel { get; }
		public OpenWith OpenWith { get; }

		public ShellCommandMenuModel(ShellCommand shellCommand, PartModel partModel, OpenWith openWith)
		{
			Command = shellCommand ?? throw new ArgumentNullException(nameof(shellCommand));
			PartModel = partModel ?? throw new ArgumentNullException(nameof(partModel));
			OpenWith = openWith ?? throw new ArgumentNullException(nameof(openWith));
		}
	}
}
