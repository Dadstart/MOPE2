using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
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

		public WebViewTabItem(Data data)
		{
			InitializeComponent();
			Data = data;
		}

		private Part m_part;
		public Part Part
		{
			get { return m_part; }
			set { m_part = value; UpdatePartView(); }
		}

		void UpdatePartView()
		{
			var uri = new UriBuilder();
			string viewType;
			if (m_part != null)
			{
				if (ContentTypes.IsXmlType(m_part.ContentType))
				{
					uri = new UriBuilder(Data.WebHost.GetUrl(m_part.GetMonacoUrl()));
					viewType = "⚡";
				}
				else if (ContentTypes.IsSupportedAudioType(m_part.ContentType))
				{
					uri = new UriBuilder(Data.WebHost.GetUrl($"part/{m_part.Uri}"));
					viewType = "🎵";
				}
				else if (ContentTypes.IsSupportedImageType(m_part.ContentType))
				{
					uri = new UriBuilder(Data.WebHost.GetUrl($"part/{m_part.Uri}"));
					viewType = "🎨";
				}
				else if (ContentTypes.IsSupportedVideoType(m_part.ContentType))
				{
					uri = new UriBuilder(Data.WebHost.GetUrl($"part/{m_part.Uri}"));
					viewType = "🖥";
				}
				else
				{
					uri = new UriBuilder("about:blank");
					viewType = "?";
				}

				if (Data.Settings.UseDarkMode)
					uri.Query += "&theme=dark";

				var header = ((CloseButtonTabHeader)Header);
				header.Text = $"{m_part.Uri}";
				header.ViewType = viewType;
			}
			else
			{
				((CloseButtonTabHeader)Header).Text = "???";
				uri = new UriBuilder("about:blank");
			}

			Browser.Source = uri.Uri;
		}
	}
}
