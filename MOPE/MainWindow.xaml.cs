﻿using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace B4.Mope
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// App data
		/// </summary>
		public Data Data { get; private set; } = new Data();
		public static IconManager IconManager { get; private set; } //TODO: remove static

		public MainWindow()
		{
			InitializeComponent();
			IconManager = new IconManager();
		}

		private void CommandBinding_HelpCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void CommandBinding_HelpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
		}

		private void CommandBinding_OpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void CommandBinding_OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Data.Package?.Close();
			Data.Package = new Package(@"C:\temp\1.docx", @"C:\temp\x");
			InitializeViews();
		}

		private void CommandBinding_SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			// TODO: Check dirty state
			e.CanExecute = true;
		}

		private void CommandBinding_SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void CommandBinding_SaveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			// TODO: Check for open file
			e.CanExecute = true;
		}

		private void CommandBinding_SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{

		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Data?.Package?.Close();
		}

		private void InitializeViews()
		{
			using (Dispatcher.DisableProcessing())
			{
				InitializePartsListView();
				InitializeZipFilesTreeView();
			}
		}

		private void InitializeZipFilesTreeView()
		{
			treeViewZipFiles.Items.Clear();
			var dir = new DirectoryInfo(Data.Package.TempDirectory);
			foreach (var info in dir.GetFileSystemInfos())
			{
				treeViewZipFiles.Items.Add(TreeViewItemFromFileSystemInfo(info));
			}
		}

		private TreeViewItem TreeViewItemFromFileSystemInfo(FileSystemInfo info)
		{
			var item = new TreeViewItem() { Header = info.Name };
			var dir = info as DirectoryInfo;
			if (dir != null)
			{
				foreach (var childInfo in dir.GetFileSystemInfos())
				{
					item.Items.Add(TreeViewItemFromFileSystemInfo(childInfo));
				}
			}

			return item;
		}

		private void InitializePartsListView()
		{
			listViewParts.ItemsSource = Data.Package.Parts.Values;
		}

		private void ToggleMenuCheckedStates(MenuItem itemToCheck)
		{
			foreach (MenuItem mi in listViewParts.ContextMenu.Items)
			{
				mi.IsChecked = itemToCheck == mi;
			}
		}

		private void ListViewMenuTilesClick(object sender, RoutedEventArgs e)
		{
			ToggleMenuCheckedStates(listViewMenuTiles);
			listViewParts.View = null;
			listViewParts.ItemTemplate = (DataTemplate)listViewParts.FindResource("tilesViewDataTemplate");
		}

		private void ListViewMenuLargeIconsClick(object sender, RoutedEventArgs e)
		{
			ToggleMenuCheckedStates(listViewMenuLargeIcons);
			listViewParts.View = null;
			listViewParts.ItemTemplate = (DataTemplate)listViewParts.FindResource("largeIconViewDataTemplate");
		}

		private void ListViewMenuSmallIconsClick(object sender, RoutedEventArgs e)
		{
			ToggleMenuCheckedStates(listViewMenuSmallIcons);
			listViewParts.View = null;
			listViewParts.ItemTemplate = (DataTemplate)listViewParts.FindResource("smallIconViewDataTemplate");
		}

		private void ListViewDetailsClick(object sender, RoutedEventArgs e)
		{
			ToggleMenuCheckedStates(listViewMenuDetails);
			listViewParts.View = (ViewBase)listViewParts.FindName("listViewDefaultGridView");
		}
	}
}
