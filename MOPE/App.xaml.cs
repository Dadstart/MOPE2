using B4.Mope.Utility;
using System.Runtime.InteropServices;
using System.Windows;

namespace B4.Mope
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		WebViewHost m_webViewHost;

		[DllImport("WebView2Loader.dll", CallingConvention =CallingConvention.StdCall)]
		static extern int GetAvailableCoreWebView2BrowserVersionString(string browserExecutableFolder, out string version);
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			if (!m_webViewHost.CheckWebView2())
				Shutdown();
		}


		private string GetArgOrDefault(string[] args, int index)
		{
			if (args.Length <= index)
				return null;

			return args[index];
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			m_webViewHost = new WebViewHost();
			string file = GetArgOrDefault(e.Args, 0);
			(new MainWindow(file, m_webViewHost)).Show();
		}
	}
}
