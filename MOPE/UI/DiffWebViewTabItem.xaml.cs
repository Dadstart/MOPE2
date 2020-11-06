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
	/// Interaction logic for EditorWebViewTabItem.xaml
	/// </summary>
	public partial class DiffWebViewTabItem : TabItem
	{
		public DiffData Data { get; }
		public DiffPackageItem PackageItem { get; }

		public DiffWebViewTabItem(DiffData data, DiffPackageItem packageItem)
		{
			InitializeComponent();
			Data = data ?? throw new ArgumentNullException(nameof(data));
			PackageItem = packageItem ?? throw new ArgumentNullException(nameof(packageItem));

			UpdatePartView();
		}

		void UpdatePartView()
		{
			var uri = new UriBuilder();
			string viewType;
			if (ContentTypes.IsMonacoSupportedType(PackageItem.Part.Left?.ContentType ?? PackageItem.Part.Right?.ContentType))
			{
				uri = new UriBuilder(Data.WebHost.GetUrl(PackageItem.Part.GetMonacoDiffUrl()));
				viewType = "⚡";

				if (Data.Settings.EditorDarkMode)
					uri.Query += "&theme=dark";
			}

			// TODO: other types
			//else if (ContentTypes.IsSupportedAudioType(part.ContentType))
			//{
			//	uri = new UriBuilder(Data.WebHost.GetUrl($"part/{part.Uri}"));
			//	viewType = "🎵";
			//}
			//else if (ContentTypes.IsSupportedImageType(part.ContentType))
			//{
			//	uri = new UriBuilder(Data.WebHost.GetUrl($"part/{part.Uri}"));
			//	viewType = "🎨";
			//}
			//else if (ContentTypes.IsSupportedVideoType(part.ContentType))
			//{
			//	uri = new UriBuilder(Data.WebHost.GetUrl($"part/{part.Uri}"));
			//	viewType = "🖥";
			//}
			else
			{
				uri = new UriBuilder("about:blank");
				viewType = "?";
			}

			var header = ((CloseButtonTabHeader)Header);
			header.Text = $"{PackageItem.Part.Uri}";
			header.ViewType = viewType;

			Browser.Source = uri.Uri;
		}
	}
}
