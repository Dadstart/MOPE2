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
		public WebViewTabItem()
		{
			InitializeComponent();
		}

		private Part m_part;
		public Part Part
		{
			get { return m_part; }
			set { m_part = value; UpdatePartView(); }
		}

		void UpdatePartView()
		{
			string url;
			string viewType;
			if (m_part != null)
			{
				var data = (Data)Window.GetWindow(this).DataContext;
				if (ContentTypes.IsXmlType(m_part.ContentType))
				{
					url = data.WebHost.GetUrl(m_part.GetMonacoUrl());
					viewType = "⚡";
				}
				else if (ContentTypes.IsSupportedAudioType(m_part.ContentType))
				{
					url = data.WebHost.GetUrl($"part/{m_part.Uri}");
					viewType = "🎵";
				}
				else if (ContentTypes.IsSupportedImageType(m_part.ContentType))
				{
					url = data.WebHost.GetUrl($"part/{m_part.Uri}");
					viewType = "🎨";
				}
				else if (ContentTypes.IsSupportedVideoType(m_part.ContentType))
				{
					url = data.WebHost.GetUrl($"part/{m_part.Uri}");
					viewType = "🖥";
				}
				else
				{
					url = "about:blank";
					viewType = "?";
				}

				var header = ((CloseButtonTabHeader)Header);
				header.Text = $"{m_part.Uri}";
				header.ViewType = viewType;

			}
			else
			{
				((CloseButtonTabHeader)Header).Text = "???";
				url = "about:blank";
			}

			Browser.Source = new Uri(url);
		}
	}
}
