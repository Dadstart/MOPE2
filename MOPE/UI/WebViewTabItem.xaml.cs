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
	public partial class WebViewTabItem : UserControl
	{
		public WebViewTabItem()
		{
			InitializeComponent();
		}

		private Part m_part;
		public Part Part
		{
			get { return m_part; }
			set
			{
				m_part = value;
				var data = (Data)Window.GetWindow(this).DataContext;
				var url = data.WebHost.GetUrl(m_part.GetMonacoUrl());
				Browser.Source = new Uri(url);
			}
		}


	}
}
