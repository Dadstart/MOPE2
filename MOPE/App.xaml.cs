using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
				var result = MessageBox.Show("A compatible version of Microsoft Edge is not installed.\r\nClick OK to visit a web page where you can download and run the 'WebView2 Runtime installer'.\r\n\r\nNote: This will not affect your current Edge installation.", "Incompatible Version of Edge", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.OK);
				if (result == MessageBoxResult.OK)
				{
					var processStartInfo = new ProcessStartInfo("https://developer.microsoft.com/en-us/microsoft-edge/webview2/");
					processStartInfo.UseShellExecute = true;
					Process.Start(processStartInfo);
				}
				Shutdown();

			}

		}
	}
}
