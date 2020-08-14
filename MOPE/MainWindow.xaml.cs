using B4.Mope.Packaging;
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
using System.Windows.Controls.Primitives;
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

			UpdateContextMenu(listViewOpenWithMenuItem, model);
		}

		private void UpdateContextMenu(MenuItem parentMenuItem, PartModel model)
		{
			// remove all existing ShellCommandMenuItems
			var itemsToRemove = new List<object>();
			foreach (var item in parentMenuItem.Items)
			{
				if (item is ShellCommandMenuItem)
					itemsToRemove.Add(item);
			}

			foreach (var item in itemsToRemove)
			{
				parentMenuItem.Items.Remove(item);
			}

			// add new ShellCommandMenuItems
			foreach (var command in model.ShellCommands)
			{
				var menuItem = new ShellCommandMenuItem(new ShellCommandMenuModel(command, model, Data.OpenWith), IconManager);
				parentMenuItem.Items.Add(menuItem);
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

		private void partOpenMenuItem_Click(object sender, RoutedEventArgs e)
		{
			OpenPart(listViewParts.SelectedItem as PartModel);
		}

		private void OpenPart(PartModel part)
		{
			if (part?.Part == null)
				return;

			var fileInfo = part.Part.GetFileInfo();
			ShellCommand.ShellExecute(IntPtr.Zero, null, "rundll32.exe", string.Concat("shell32.dll,OpenAs_RunDLL ", fileInfo.FullName), null, 0);
		}

		private void treeViewZipFiles_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource == null)
				return;

			PackageItem packageItem = null;

			// search up the tree for the element with the data context
			var elt = e.OriginalSource as FrameworkElement;
			while (elt != null)
			{
				packageItem = elt.DataContext as PackageItem;
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

		private void ShowTreeViewContextMenu(PackageItem packageItem)
		{
			if ((packageItem == null) || packageItem.IsFolder())
				return;

			var contextMenu = (ContextMenu)FindResource("openWithContextMenu");
			var menuItem = (MenuItem)LogicalTreeHelper.FindLogicalNode(contextMenu, "openWithMenuItem");
			UpdateContextMenu(menuItem, packageItem.Model);
			contextMenu.IsOpen = true;
		}

		private void treeViewZipFiles_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Apps)
			{
				ShowTreeViewContextMenu(treeViewZipFiles.SelectedItem as PackageItem);
				e.Handled = true;
			}
		}
	}
}
