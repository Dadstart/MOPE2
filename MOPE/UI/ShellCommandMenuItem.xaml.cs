using B4.Mope.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace B4.Mope.UI
{
	/// <summary>
	/// Interaction logic for ListViewStateMenuItem.xaml
	/// </summary>
	public partial class ShellCommandMenuItem : MenuItem
	{
		private readonly IconManager m_iconManager;

		public ShellCommandMenuModel Model { get; }

		public ShellCommandMenuItem(ShellCommandMenuModel model, IconManager iconManager)
		{
			Model = model ?? throw new ArgumentNullException(nameof(model));
			m_iconManager = iconManager ?? throw new ArgumentNullException(nameof(iconManager));
			InitializeComponent();
			Click += ShellCommandMenuItem_Click;

			var command = Model.Command;
			var appFullPath = Model.OpenWith.ApplicationFullPaths[command.Application];
			Header = command.FriendlyName ?? FileHelper.DisplayNameForApplication(command.Application, appFullPath);

			var icon = m_iconManager.GetImageForApplicationName(appFullPath);
			var image = new Image() { Source = icon };
			Icon = image;
		}

		private void ShellCommandMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var process = new Process();
			var command = Model.Command;
			var fileInfo = Model.PartModel.Part.GetFileInfo();

			if (command.DDE != null)
			{
				process.StartInfo.FileName = command.GetApplication();
				process.StartInfo.Arguments = String.Concat('"', fileInfo.FullName, '"');
			}
			else
			{
				process.StartInfo.FileName = command.GetApplication();
				process.StartInfo.Arguments = String.Concat('"', fileInfo.FullName, '"');
			}

			process.Start();
		}
	}
}
