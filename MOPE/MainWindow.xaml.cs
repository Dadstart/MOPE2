using B4.Mope.Packaging;
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
			IconManager = new IconManager();

			Unloaded += MainWindow_Unloaded;
			Data = new Data();
			Data.WebHost = new WebHost(Data);
			Data.WebHost.ListenOnThread();
			DataContext = Data;
		}

		private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
		{
			Data?.WebHost?.Stop();
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
			Data.Package?.Close();
			Data.Package = new Package(@"C:\temp\lorem.docx", @"C:\temp\x");
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
			treeViewZipFiles.ItemsSource = Data.Package.Items;
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

		private void treeViewZipFiles_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var packageItem = (PackageItem)treeViewZipFiles.SelectedItem;
			SetActivePart(packageItem.Part);
		}

		private void listViewParts_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetActivePart((Part)listViewParts.SelectedItem);
		}

		private void SetActivePart(Part part)
        {
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
    }
}
