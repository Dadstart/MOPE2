using Microsoft.Win32;
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

namespace B4.Mope
{
	/// <summary>
	/// Interaction logic for DiffWindow.xaml
	/// </summary>
	public partial class DiffWindow : Window
	{
		public DiffData Data { get; } = new DiffData();

		public DiffWindow(string left, string right)
		{
			Data.DiffsLoaded += Data_DiffsLoaded;
			InitializeComponent();
			if (!string.IsNullOrEmpty(left))
				OpenLeftPackage(left);
			if (!string.IsNullOrEmpty(right))
				OpenRightPackage(right);
		}

		private void Data_DiffsLoaded(object sender, EventArgs e)
		{
			partsTabControl.Items.Clear();
			Title = $"MOPE Diff: {Data.Left.ZipFile} <=> {Data.Right.ZipFile}";
		}

		private void listViewParts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}

		private void bigOpenLeftButton_Click(object sender, RoutedEventArgs e)
		{
			var path = ShowOpenDialog("Open Left Package");
			if (path == null)
				return;

			OpenLeftPackage(path);
		}

		private void OpenLeftPackage(string path)
		{
			Data.LoadLeft(path);
			bigOpenLeftButton.Visibility = Visibility.Collapsed;
			bigLeftLabel.Content = path;
			bigLeftLabel.Visibility = Visibility.Visible;
		}

		private void bigOpenRightButton_Click(object sender, RoutedEventArgs e)
		{
			var path = ShowOpenDialog("Open Right Package");
			if (path == null)
				return;

			OpenRightPackage(path);
		}

		private void OpenRightPackage(string path)
		{
			Data.LoadRight(path);
			bigOpenRightButton.Visibility = Visibility.Collapsed;
			bigRightLabel.Content = path;
			bigRightLabel.Visibility = Visibility.Visible;
		}

		private string ShowOpenDialog(string caption)
		{
			var dlg = new OpenFileDialog()
			{
				Title = caption,
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = "Office Files|*.docx;*.docm;*.dotx;*.dotm;*.xlsx;*.xlsm;*.pptx;*.pptm;*.odt;*.ods;*.odp|All Files|*.*",
			};

			return dlg.ShowDialog(this) == true ? dlg.FileName : null;
		}
	}
}
