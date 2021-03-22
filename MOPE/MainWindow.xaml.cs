using B4.Mope.Packaging;
using B4.Mope.UI;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

		public MainWindow(string file)
		{
			InitializeComponent();

			UpdateListViewAndMenus(ListViewState.Default);
			IconManager = new IconManager();

			Unloaded += MainWindow_Unloaded;
			Data = new Data();
			DataContext = Data;

			Data.Settings.EditorReadOnlyModeChanged += Data_EditorReadOnlyModeChanged;
			Data.Settings.EditorDarkModeChanged += Data_EditorDarkModeChanged;

#if DEBUG
			menuMain.Items.Add(FindResource("debugMenu"));
#endif

			if (file != null)
				OpenPackage(file);
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
			var dlg = new OpenFileDialog()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Filter = "Office Files|*.docx;*.docm;*.dotx;*.dotm;*.xlsx;*.xlsm;*.pptx;*.pptm;*.odt;*.ods;*.odp|All Files|*.*",
			};

			if (dlg.ShowDialog(this) != true)
				return;

			OpenPackage(dlg.FileName);
		}

		private void OpenPackage(string fileName)
		{
			Dispatcher.Invoke(() =>
			{
				// REVIEW: might not need to call reset?
				if (Data.Package != null)
					Data.Reset();
				partsTabControl.Items.Clear();

				Data.Init(fileName);
				Data.PackageWatcher.Changed += PackageWatcher_Changed;
				InitializeViews();
			});
		}

		private void PackageWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (Data.IgnoringChanges)
				return;

			var result = ExternalPackageChangeDialog.ShowModal(this);

			switch (result)
			{
				case ExternalPackageChangeDialog.PackageChangeDialogResult.Unknown:
				case ExternalPackageChangeDialog.PackageChangeDialogResult.IgnoreChanges:
					Data.IgnoringChanges = true;
					break;
				case ExternalPackageChangeDialog.PackageChangeDialogResult.DiscardAndReload:
					var file = Data.Package.ZipFile;
					OpenPackage(file);
					break;
				case ExternalPackageChangeDialog.PackageChangeDialogResult.DiffChanges:
					Data.BackupCopyOwned = false;
					ShowDiffWindow(Data.BackupCopy, right: Data.Package.ZipFile);
					break;
			}
		}

		private void CommandBinding_SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			// TODO: Check dirty state
			e.CanExecute = !Data.Settings.EditorReadOnlyMode;
		}

		private void CommandBinding_SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// save current part
			var webView = partsTabControl.SelectedItem as EditorWebViewTabItem;
			if ((webView != null) && (webView.PartModel.IsDirty))
				webView.Browser.ExecuteScriptAsync($"postFile()");
		}

		private void CommandBinding_SaveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (Data.Package != null) && (partsTabControl.SelectedItem != null);
		}

		private void CommandBinding_SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if ((Data.Package == null) || (partsTabControl.SelectedItem == null))
				return;

			var partModel = GetPartModelFromTabView(partsTabControl.SelectedItem);

			var dlg = new SaveFileDialog()
			{
				FileName = partModel.Part.GetFileInfo().FullName
			};

			dlg.ShowDialog();
		}

		private void CommandBinding_SavePackageCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (Data.Package != null) && Data.IsPackageDirty;
		}

		private void CommandBinding_SavePackageExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// confirm
			if (Data.Settings.ConfirmOverwritePackage)
			{
				var confirmDialog = new ConfirmOverwritePackageDialog(!Data.Settings.ConfirmOverwritePackage);
				if (confirmDialog.ShowDialog() != true || !confirmDialog.OvewriteResult)
					return;

				if (confirmDialog.AlwaysOverwritePackage)
				{
					Data.Settings.ConfirmOverwritePackage = false;
				}
			}

			SavePackageAs(Data.Package.ZipFile);
			Data.IsPackageDirty = false;
		}

		private void SaveDirtyParts()
		{
			// save any dirty parts
			foreach (var tabViewItem in partsTabControl.Items)
			{
				var webView = tabViewItem as EditorWebViewTabItem;
				if ((webView != null) && (webView.PartModel.IsDirty))
					webView.Browser.ExecuteScriptAsync($"postFile()");
			}
		}

		private void SavePackageAs(string filename)
		{
			SaveDirtyParts();

			try
			{
				Data.SaveAs(filename);
				Title = $"MOPE: {Data.Package.ZipFile}";
			}
			catch (Exception exc)
			{
				//if (exc.HResult == -2147024864)
				//{
					// file is in use
					//MessageBox.Show(this, $"Cannot overwrite {filename}. File is locked or in use by another application.\r\n\r\nChoose Yes to save as a different filename or cancel to cancel ", MessageBoxButton.)
				//}
				//else
				{
					MessageBox.Show(this, $"Error saving to {filename}\r\n\r\n{exc}", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void CommandBinding_SavePackageAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Data.Package != null;
		}

		private void CommandBinding_SavePackageAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				FileName = Data.Package.ZipFile
			};

			if (saveFileDialog.ShowDialog() == true)
				SavePackageAs(saveFileDialog.FileName);
		}

		private PartModel GetPartModelFromTabView(object tabViewItem)
		{
			var webView = tabViewItem as EditorWebViewTabItem;
			if (webView != null)
				return webView.PartModel;

			var binView = tabViewItem as BinaryViewTabItem;
			if (binView != null)
				return binView.PartModel;

			return null;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Data?.Package?.Close();
		}

		private void InitializeViews()
		{
			Dispatcher.Invoke(() =>
			{
				using (Dispatcher.DisableProcessing())
				{
					Title = $"MOPE: {Data.Package.ZipFile}";
					InitializePartsListView();
					InitializeZipFilesTreeView();
				}
			});
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
			if (model == null)
				return;

			var part = model.Part;
			var tabItem = GetTabItemWithPart(part);
			if (tabItem == null)
			{
				if (part.CanViewInBrowser())
				{
					var webItem = new EditorWebViewTabItem(Data, model);
					partsTabControl.Items.Add(webItem);
					partsTabControl.SelectedItem = webItem;
				}
				else
				{
					var binaryItem = new BinaryViewTabItem(Data, model);
					partsTabControl.Items.Add(binaryItem);
					partsTabControl.SelectedItem = binaryItem;
				}
			}

			// remove existing OpenWithMenu
			if (listViewParts.ContextMenu.Items[0] is OpenWithMenuItem)
				listViewParts.ContextMenu.Items.RemoveAt(0);

			var openWithMenuItem = new OpenWithMenuItem(model.Part);
			listViewParts.ContextMenu.Items.Insert(0, openWithMenuItem);
			UpdateContextMenu(openWithMenuItem, model);
		}

		private void UpdateContextMenu(MenuItem parentMenuItem, PartModel model)
		{
			foreach (var command in model.ShellCommands)
			{
				var menuItem = new ShellCommandMenuItem(new ShellCommandMenuModel(command, model.Part, Data.OpenWith), IconManager);
				parentMenuItem.Items.Add(menuItem);
			}
		}

		private TabItem GetTabItemWithPart(Part part)
		{
			if (part == null)
				return null;

			foreach (TabItem item in partsTabControl.Items)
			{
				var partModel = GetPartModelFromTabView(item);
				if (partModel.Part == part)
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

			var contextMenu = new ContextMenu();
			var openWithMenu = new OpenWithMenuItem(packageItem.Model.Part);
			contextMenu.Items.Add(openWithMenu);

			UpdateContextMenu(openWithMenu, packageItem.Model);
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

		private void debugInjectJsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var currentWebView = partsTabControl.SelectedItem as EditorWebViewTabItem;
			if (currentWebView == null)
				return;

			
			currentWebView.Browser.ExecuteScriptAsync("window.alert('hello')");
		}

		private void debugBreak_Click(object sender, RoutedEventArgs e)
		{
			Debugger.Break();
		}

		private void debugShowConfirmOverwritePackageDialog_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ConfirmOverwritePackageDialog(!Data.Settings.ConfirmOverwritePackage);
			var dialogResult = dialog.ShowDialog();

			MessageBox.Show($"ShowDialog Result: {dialogResult}\r\n\r\nOverwrite Result:{dialog.OvewriteResult}\r\n\r\nAlwaysOvewrite:{dialog.AlwaysOverwritePackage}");
		}

		private void debugShowExternalPackageChangeDialog_Click(object sender, RoutedEventArgs e)
		{
			var result = ExternalPackageChangeDialog.ShowModal(this);

			MessageBox.Show($"Dialog Result: {result}");
		}

		private void debugShowOpenDiffWindowDialog_Click(object sender, RoutedEventArgs e)
		{
			var result = OpenDiffWindowDialog.ShowModal(this);

			MessageBox.Show($"Dialog Result: {result}");
		}

		private void Data_EditorDarkModeChanged(object sender, BooleanPropertyChangedEventArgs e)
		{
			if (partsTabControl == null)
			{
				// this can happen on init
				return;
			}

			// update all open browsers
			foreach (var partView in partsTabControl.Items)
			{
				var webView = partView as EditorWebViewTabItem;
				if (webView == null)
					continue;

				var param = e.NewValue ? "true" : "false";
				webView.Browser.ExecuteScriptAsync($"updateTheme({param})");
			}
		}

		private void Data_EditorReadOnlyModeChanged(object sender, BooleanPropertyChangedEventArgs e)
		{
			if (partsTabControl == null)
			{
				// this can happen on init
				return;
			}

			// update all open browsers
			foreach (var partView in partsTabControl.Items)
			{
				var webView = partView as EditorWebViewTabItem;
				if (webView == null)
					continue;

				var param = e.NewValue ? "true" : "false";
				webView.Browser.ExecuteScriptAsync($"setReadOnly({param})");
			}
		}

		private void bigOpenButton_Click(object sender, RoutedEventArgs e)
		{
			CommandBinding_OpenExecuted(this, e: null);
		}


		private void DiffMenuItem_Click(object sender, RoutedEventArgs e)
		{
			string left = null;
			string right = null;

			// if a package is open show the dialog prompting how to use current package
			if (Data?.Package != null)
			{
				var result = OpenDiffWindowDialog.ShowModal(this);
				switch (result)
				{
					case OpenDiffWindowDialog.OpenDiffWindowDialogResult.Left:
						left = Data.Package.ZipFile;
						break;
					case OpenDiffWindowDialog.OpenDiffWindowDialogResult.Right:
						right = Data.Package.ZipFile;
						break;
					case OpenDiffWindowDialog.OpenDiffWindowDialogResult.Neither:
						//no-op
						break;
					case OpenDiffWindowDialog.OpenDiffWindowDialogResult.Unknown:
						// canceled
						return;
				}
			}

			ShowDiffWindow(left, right);
		}

		private void ShowDiffWindow(string left, string right)
		{
			Dispatcher.Invoke(() =>
			{
				var diffWindow = new DiffWindow(left, right)
				{
					ShowActivated = true,
					ShowInTaskbar = true,
				};

				diffWindow.Show();

				// if no package is open close current window
				if (Data?.Package == null)
					Close();
			});
		}

		private void helpAboutMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("MOPE (Microsoft Office Package Editor).\r\n\r\nEmail abishop@microsoft.com or visit https://github.com/Dadstart/MOPE2.\r\n\r\nGo to GitHub now?", "About MOPE", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
				LaunchGitHub();
		}

		private void helpGitHubMenuItem_Click(object sender, RoutedEventArgs e)
		{
			LaunchGitHub();
		}

		private void LaunchGitHub()
		{
			var processStartInfo = new ProcessStartInfo("https://github.com/Dadstart/MOPE2")
			{
				UseShellExecute = true
			};

			Process.Start(processStartInfo);
		}
	}
}
