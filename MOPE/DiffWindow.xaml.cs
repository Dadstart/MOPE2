using B4.Mope.Packaging;
using B4.Mope.Shell;
using B4.Mope.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
		private readonly IconManager m_iconManager;

		public DiffData Data { get; }

		public DiffWindow(string left, string right, ShellOpenWithData openWith, IconManager iconManager)
		{
			Data = new DiffData(openWith);
			Data.DiffsLoaded += Data_DiffsLoaded;

			InitializeComponent();
			if (!string.IsNullOrEmpty(left))
				OpenLeftPackage(left);
			if (!string.IsNullOrEmpty(right))
				OpenRightPackage(right);

			m_iconManager = iconManager ?? throw new ArgumentNullException(nameof(iconManager));

#if DEBUG
			menuMain.Items.Add(FindResource("debugMenu"));
#endif
		}

		private void Data_DiffsLoaded(object sender, EventArgs e)
		{
			partsTabControl.Items.Clear();
			Title = $"MOPE Diff: {Data.Left.ZipFile} <=> {Data.Right.ZipFile}";
			treeViewFolders.ItemsSource = Data.Folders.Values;
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

		private void AddOpenWithContextMenu(ContextMenu contextMenu, Part part, string header)
		{
			// part may be null if only in left or right package
			if (part == null)
				return;

			var openWithMenu = new OpenWithMenuItem(part);
			openWithMenu.Header = header;
			contextMenu.Items.Add(openWithMenu);


			foreach (var command in Data.OpenWith.GetCommandsForPart(part))
			{
				var menuItem = new ShellCommandMenuItem(new ShellCommandMenuModel(command, part, Data.OpenWith), m_iconManager);
				openWithMenu.Items.Add(menuItem);
			}
		}

		private void ShowTreeViewContextMenu(DiffPackageItem packageItem)
		{
			if ((packageItem == null) || packageItem.IsFolder())
				return;

			var contextMenu = new ContextMenu();
			AddOpenWithContextMenu(contextMenu, packageItem.Part.Left, "Open Left With");
			AddOpenWithContextMenu(contextMenu, packageItem.Part.Right, "Open Right With");

			contextMenu.IsOpen = true;
		}

		private void treeViewFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var packageItem = (DiffPackageItem)treeViewFolders.SelectedItem;

			if (packageItem.IsFolder())
				return;

			SetActivePart(packageItem);
		}

		private void SetActivePart(DiffPackageItem packageItem)
		{
			if (packageItem.IsFolder())
				return;

			// TODO: find existing tab item
			bool canDiff = (packageItem.Part.Left != null) ? packageItem.Part.Left.IsAnyTextType() : packageItem.Part.Right.IsAnyTextType();
			if (canDiff)
			{
				var webItem = new DiffWebViewTabItem(Data, packageItem);
				partsTabControl.Items.Add(webItem);
				partsTabControl.SelectedItem = webItem;
			}
			else
			{
				// TODO
			}
		}

		private void treeViewFolders_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource == null)
				return;

			DiffPackageItem packageItem = null;

			// search up the tree for the element with the data context
			var elt = e.OriginalSource as FrameworkElement;
			while (elt != null)
			{
				packageItem = elt.DataContext as DiffPackageItem;
				if (packageItem != null)
					break;

				elt = elt.Parent as FrameworkElement;
			}

			// setting IsSelected to true causes weird display issue so don't do that for now
			//var treeViewItem = treeViewZipFiles.ContainerFromElement(elt) as TreeViewItem;
			//if (treeViewItem != null)
			//	treeViewItem.IsSelected = true;

			e.Handled = true;

			// populate and show context menu
			ShowTreeViewContextMenu(packageItem);
		}

		private void treeViewFolders_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Apps)
			{
				ShowTreeViewContextMenu(treeViewFolders.SelectedItem as DiffPackageItem);
				e.Handled = true;
			}
		}

		private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void debugInjectJsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//var currentWebView = partsTabControl.SelectedItem as WebViewTabItem;
			//if (currentWebView == null)
			//	return;


			//currentWebView.Browser.ExecuteScriptAsync("window.alert('hello')");
		}

		private void debugBreak_Click(object sender, RoutedEventArgs e)
		{
			Debugger.Break();
		}

	}
}
