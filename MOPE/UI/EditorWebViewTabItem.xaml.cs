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
	public partial class EditorWebViewTabItem : TabItem
	{
		public Data Data { get; }

		public EditorWebViewTabItem(Data data, PartModel model)
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
			if (ContentTypes.IsMonacoSupportedType(part.ContentType))
			{
				uri = new UriBuilder(Data.WebHost.GetUrl(part.GetMonacoEditorUrl()));
				viewType = "⚡";

				if (Data.Settings.EditorDarkMode)
					uri.Query += "&theme=dark";

				if (Data.Settings.EditorReadOnlyMode)
					uri.Query += "&readonly=true";
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
