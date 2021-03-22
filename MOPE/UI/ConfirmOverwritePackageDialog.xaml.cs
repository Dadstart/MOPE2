using System;
using System.Collections.Generic;
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
	public partial class ConfirmOverwritePackageDialog : Window
	{
		public bool OvewriteResult { get; set; }
		public bool AlwaysOverwritePackage
		{
			get { return alwaysOvewriteCheckbox.IsChecked == true; }
		}

		public ConfirmOverwritePackageDialog(bool alwaysOvewritePackage)
		{
			InitializeComponent();
			alwaysOvewriteCheckbox.IsChecked = alwaysOvewritePackage;
		}

		private void buttonYes_Click(object sender, RoutedEventArgs e)
		{
			OvewriteResult = true;
			DialogResult = true;
			Close();
		}

		private void buttonNo_Click(object sender, RoutedEventArgs e)
		{
			OvewriteResult = false;
			DialogResult = true;
			Close();
		}
	}
}
