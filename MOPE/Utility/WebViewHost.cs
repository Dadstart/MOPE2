using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace B4.Mope.Utility
{
	public class WebViewHost
	{
		[DllImport("WebView2Loader.dll", CallingConvention = CallingConvention.StdCall)]
		static extern int GetAvailableCoreWebView2BrowserVersionString(string browserExecutableFolder, out string version);

		public CoreWebView2Environment Environment { get; init; }

		public WebViewHost()
		{
			string rootDir = Temp.GetRootTempDir();
			string dataDir = Path.Combine(rootDir, "WebView2");
			if (!Directory.Exists(dataDir))
			{
				Directory.CreateDirectory(dataDir);
			}
			Environment = CoreWebView2Environment.CreateAsync(null, dataDir, null).Result;
		}

		public bool CheckWebView2()
		{
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

				return false;
			}

			return true;
		}
	}
}
