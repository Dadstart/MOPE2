using B4.Mope.Packaging;
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
	public partial class OpenWithMenuItem : MenuItem
	{
		public Part Part { get; }

		public OpenWithMenuItem(Part part)
		{
			Part = part ?? throw new ArgumentNullException(nameof(part));

			Header = "Open With";
			var otherMenuItem = new MenuItem()
			{
				Header = "Other"
			};
			otherMenuItem.Click += otherMenuItem_Click;
			Items.Add(otherMenuItem);
			Items.Add(new Separator());
		}

		private void otherMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var fileInfo = Part.GetFileInfo();
			ShellCommand.ShellExecute(IntPtr.Zero, null, "rundll32.exe", string.Concat("shell32.dll,OpenAs_RunDLL ", fileInfo.FullName), null, 0);
		}
	}
}
