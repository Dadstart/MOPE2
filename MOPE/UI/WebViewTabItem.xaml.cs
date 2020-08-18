using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using B4.Mope.Packaging;
using Microsoft.Web.WebView2.Wpf;

namespace B4.Mope.UI
{
	/// <summary>
	/// Interaction logic for WebViewTabItem.xaml
	/// </summary>
	public partial class WebViewTabItem : TabItem
	{
		public Data Data { get; }

		public WebViewTabItem(Data data, PartModel model)
		{
			InitializeComponent();
			Data = data ?? throw new ArgumentNullException(nameof(data));
			PartModel = model ?? throw new ArgumentNullException(nameof(model));
			UpdatePartView();
		}

		public PartModel PartModel { get; }

		void UpdatePartView()
		{
			var uri = new UriBuilder();
			string viewType;
			PartModel.DirtyChanged += PartModel_DirtyChanged;
			var part = PartModel.Part;
			if (ContentTypes.IsXmlType(part.ContentType))
			{
				uri = new UriBuilder(Data.WebHost.GetUrl(part.GetMonacoUrl()));
				viewType = "⚡";

				if (Data.Settings.UseDarkMode)
					uri.Query += "&theme=dark";
			}
			else if (ContentTypes.IsSupportedAudioType(part.ContentType))
			{
				uri = new UriBuilder(Data.WebHost.GetUrl($"part/{part.Uri}"));
				viewType = "🎵";
			}
			else if (ContentTypes.IsSupportedImageType(part.ContentType))
			{
				uri = new UriBuilder(Data.WebHost.GetUrl($"part/{part.Uri}"));
				viewType = "🎨";
			}
			else if (ContentTypes.IsSupportedVideoType(part.ContentType))
			{
				uri = new UriBuilder(Data.WebHost.GetUrl($"part/{part.Uri}"));
				viewType = "🖥";
			}
			else
			{
				uri = new UriBuilder("about:blank");
				viewType = "?";
			}

			var header = ((CloseButtonTabHeader)Header);
			header.Text = $"{part.Uri}";
			header.ViewType = viewType;

			Browser.Source = uri.Uri;
		}

		private void PartModel_DirtyChanged(object sender, EventArgs e)
		{
			Dispatcher.BeginInvoke(new ThreadStart(() => ((CloseButtonTabHeader)Header).IsDirty = PartModel.IsDirty));
		}
	}
}
