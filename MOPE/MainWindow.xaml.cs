﻿using B4.Mope.Packaging;
using B4.Mope.Shell;
using B4.Mope.UI;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHexaEditor;

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
		public Data Data { get; }
		public static IconManager IconManager { get; private set; } //TODO: remove static

		public MainWindow()
		{
			InitializeComponent();
			UpdateListViewAndMenus(ListViewState.Default);
			IconManager = new IconManager();

			Unloaded += MainWindow_Unloaded;
			Data = new Data();
			DataContext = Data;
		}

		private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
		{
			Data?.Reset();
		}

		private string GetEmbeddedResourceAsText(string folder, string name)
		{
			
			using (var stream = Application.GetResourceStream(new Uri($"pack://application:,,,/MOPE;component/{folder}/{name}")).Stream)
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
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
			Data.Reset();

			var package = new Package(@"C:\temp\lorem2.docx", @"C:\temp\x");
			Data.Init(package);

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
			treeViewZipFiles.ItemsSource = Data.Items;
		}

		private void InitializePartsListView()
		{
			listViewParts.ItemsSource = Data.PartModels.Values;
		}

		private void treeViewZipFiles_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var packageItem = (PackageItem)treeViewZipFiles.SelectedItem;
			
			if (!packageItem.IsFolder())
				SetActivePart(packageItem.Model);
		}

		private void listViewParts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetActivePart((PartModel)listViewParts.SelectedItem);
		}

		private void SetActivePart(PartModel model)
        {
			var part = model.Part;
			var tabItem = GetTabItemWithPart(part);
			if (tabItem == null)
			{
				if (part.CanViewInBrowser())
				{
					var webItem = new WebViewTabItem();
					partsTabControl.Items.Add(webItem);
					webItem.Part = part;
					partsTabControl.SelectedItem = webItem;
				}
				else
				{
					var binaryItem = new BinaryViewTabItem();
					partsTabControl.Items.Add(binaryItem);
					binaryItem.Part = part;
					partsTabControl.SelectedItem = binaryItem;
				}
			}
		}

		private TabItem GetTabItemWithPart(Part part)
        {
			if (part == null)
				return null;

			foreach (TabItem item in partsTabControl.Items)
            {
				var bItem = item as BinaryViewTabItem;
				if ((bItem != null) && (bItem.Part == part))
					return item;

				var webItem = item as WebViewTabItem;
				if ((webItem != null) && (webItem.Part == part))
					return item;
			}

			return null;
        }

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OpenPartInShell(Part part, ShellCommand shellCommand, bool openWith)
		{

		}

		private void ListViewStateMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as ListViewStateMenuItem;

			// unclear how this could happen but if it does just no-op
			if (menuItem == null)
				return;

			UpdateListViewAndMenus(menuItem.ViewState);
		}

		private void UpdateListViewAndMenus(ListViewState viewState)
		{
			UpdateListViewStateMenuItems(toolbarViewMenu, viewState);
			UpdateListViewStateMenuItems(listViewViewMenuItem, viewState);
			UpdateListView(listViewParts, viewState);
		}

		private void UpdateListView(ListView listView, ListViewState viewState)
		{
			switch (viewState)
			{
				case ListViewState.Details:
					listView.View = (ViewBase)listView.FindName("listViewDefaultGridView");
					break;
				case ListViewState.LargeIcons:
					listView.View = null;
					listView.ItemTemplate = (DataTemplate)listView.FindResource("largeIconViewDataTemplate");
					break;
				case ListViewState.SmallIcons:
					listView.View = null;
					listView.ItemTemplate = (DataTemplate)listView.FindResource("smallIconViewDataTemplate");
					break;
				case ListViewState.Tiles:
					listView.View = null;
					listView.ItemTemplate = (DataTemplate)listView.FindResource("tilesViewDataTemplate");
					break;
			}

		}

		private void UpdateListViewStateMenuItems(MenuItem parent, ListViewState viewState)
		{
			foreach (var menuItem in parent.Items)
			{
				// we only care about other ListViewStateMenuItem
				var viewStateMenuItem = menuItem as ListViewStateMenuItem;
				if (viewStateMenuItem == null)
					continue;

				viewStateMenuItem.IsChecked = viewStateMenuItem.ViewState == viewState;
			}
		}
	}
}
