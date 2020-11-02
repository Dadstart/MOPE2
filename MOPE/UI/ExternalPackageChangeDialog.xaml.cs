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
	/// Interaction logic for ConfirmOverwritePackageDialog.xaml
	/// </summary>
	public partial class ExternalPackageChangeDialog : Window
	{
		public enum PackageChangeDialogResult
		{
			Unknown,
			IgnoreChanges,
			DiscardAndReload,
			DiffChanges,
		}

		public PackageChangeDialogResult Result { get; private set; } = PackageChangeDialogResult.Unknown;

		public ExternalPackageChangeDialog()
		{
			InitializeComponent();
		}

		private void buttonIgnore_Click(object sender, RoutedEventArgs e)
		{
			Result = PackageChangeDialogResult.IgnoreChanges;
			Close();
		}

		private void buttonDiscard_Click(object sender, RoutedEventArgs e)
		{
			Result = PackageChangeDialogResult.DiscardAndReload;
			Close();
		}

		private void buttonDiff_Click(object sender, RoutedEventArgs e)
		{
			Result = PackageChangeDialogResult.DiffChanges;
			Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			// user must choose an action
			if (Result == PackageChangeDialogResult.Unknown)
			{
				e.Cancel = true;
			}
		}

		public static PackageChangeDialogResult ShowModal(Window owner)
		{
			var result = PackageChangeDialogResult.Unknown;
			owner.Dispatcher.Invoke(() =>
			{
				var dlg = new ExternalPackageChangeDialog();
				dlg.Owner = owner;
				dlg.ShowDialog();
				result = dlg.Result;
			});

			return result;
		}
	}
}
