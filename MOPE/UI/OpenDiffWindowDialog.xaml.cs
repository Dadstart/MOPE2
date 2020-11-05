using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace B4.Mope.UI
{
	/// <summary>
	/// Interaction logic for OpenDiffWindowDialog.xaml
	/// </summary>
	public partial class OpenDiffWindowDialog : Window
	{
		public enum OpenDiffWindowDialogResult
		{
			Unknown,
			Left,
			Neither,
			Right,
		}

		public OpenDiffWindowDialogResult Result { get; private set; } = OpenDiffWindowDialogResult.Unknown;

		public OpenDiffWindowDialog()
		{
			InitializeComponent();
		}

		public static OpenDiffWindowDialogResult ShowModal(Window owner)
		{
			var result = OpenDiffWindowDialogResult.Unknown;
			owner.Dispatcher.Invoke(() =>
			{
				var dlg = new OpenDiffWindowDialog();
				dlg.Owner = owner;
				dlg.ShowDialog();
				result = dlg.Result;
			});

			return result;
		}

		private void buttonLeft_Click(object sender, RoutedEventArgs e)
		{
			Result = OpenDiffWindowDialogResult.Left;
			Close();
		}

		private void buttonRight_Click(object sender, RoutedEventArgs e)
		{
			Result = OpenDiffWindowDialogResult.Right;
			Close();
		}

		private void buttonNeither_Click(object sender, RoutedEventArgs e)
		{
			Result = OpenDiffWindowDialogResult.Neither;
			Close();
		}
	}
}
