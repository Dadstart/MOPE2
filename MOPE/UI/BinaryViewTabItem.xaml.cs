using System;
using System.Collections.Generic;
using System.IO;
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
	public partial class BinaryViewTabItem : TabItem
	{
		private Stream m_stream;

		public BinaryViewTabItem(Data data, PartModel partModel)
		{
			InitializeComponent();
			Data = data ?? throw new ArgumentNullException(nameof(data));
			PartModel = partModel ?? throw new ArgumentNullException(nameof(partModel));
			UpdatePartView();
		}

		private void UpdatePartView()
		{
			var header = ((CloseButtonTabHeader)Header);
			header.Text = $"{PartModel.Part.Uri}";
			header.ViewType = "🧩";
			using (var fileStream = PartModel.Part.GetFileInfo().OpenRead())
			{
				m_stream?.Dispose();
				m_stream = new MemoryStream();
				fileStream.CopyTo(m_stream);
				m_stream.Seek(0, SeekOrigin.Begin);

			}
			Editor.Stream = m_stream;
			//Editor.FileName = PartModel.Part.GetFileInfo().FullName;
		}

		public Data Data { get; }
		public PartModel PartModel { get; }
	}
}
