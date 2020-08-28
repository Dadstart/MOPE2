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
	public partial class ExternalPackageChangeDialog : Window
	{
		public MessageBoxResult Result { get; private set; }
		public bool DontShowDialogAgain
		{
			get { return dontShowThisCheckbox.IsChecked == true; }
		}

		public ExternalPackageChangeDialog()
		{
			InitializeComponent();
		}

		private void buttonYes_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.Yes;
			Close();
		}

		private void buttonNo_Click(object sender, RoutedEventArgs e)
		{
			Result = MessageBoxResult.No;
			Close();
		}
	}
}
