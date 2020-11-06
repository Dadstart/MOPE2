using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace B4.Mope
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		[DllImport("WebView2Loader.dll", CallingConvention =CallingConvention.StdCall)]
		static extern int GetAvailableCoreWebView2BrowserVersionString(string browserExecutableFolder, out string version);
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// check if the Edge/WebView2 control is available
			if ((GetAvailableCoreWebView2BrowserVersionString(null, out string edgeVersion) != 0) || (edgeVersion == null))
			{
				var result = MessageBox.Show("A compatible version of Microsoft Edge is not installed.\r\nClick OK to launch your browser and download and run the 'WebView2 Runtime installer'.\r\n\r\nNote: This will not affect your current Edge installation.", "Incompatible Version of Edge", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.OK);
				if (result == MessageBoxResult.OK)
				{
					var processStartInfo = new ProcessStartInfo("https://go.microsoft.com/fwlink/p/?LinkId=2124703");
					processStartInfo.UseShellExecute = true;
					Process.Start(processStartInfo);
				}
				Shutdown();

			}

		}

		private string GetArgOrDefault(string[] args, int index)
		{
			if (args.Length <= index)
				return null;

			return args[index];
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			bool diffWindow = false;
			string file1 = null, file2 = null;
			if (e.Args.Length > 0)
			{
				if (e.Args[0].ToUpperInvariant().Equals("DIFF"))
				{
					diffWindow = true;
					file1 = GetArgOrDefault(e.Args, 1);
					file2 = GetArgOrDefault(e.Args, 2);
				}
				else
				{
					file1 = GetArgOrDefault(e.Args, 0);
					file2 = GetArgOrDefault(e.Args, 1);

					if (file2 != null)
						diffWindow = true;
				}
			}


			if ((file1 != null) && !File.Exists(file1))
			{
				MessageBox.Show($"File not found {file1}", "File not found", MessageBoxButton.OK, MessageBoxImage.Error);
				Shutdown();
				return;
			}
			if ((file2 != null) && !File.Exists(file2))
			{
				MessageBox.Show($"File not found {file2}", "File not found", MessageBoxButton.OK, MessageBoxImage.Error);
				Shutdown();
				return;
			}

			if (diffWindow)
			{
				(new DiffWindow(file1, file2)).Show();
			}
			else
			{
				(new MainWindow(file1)).Show();
			}
		}
	}
}
