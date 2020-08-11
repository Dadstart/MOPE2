using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace B4.Mope.UI
{
    /// <summary>
    /// Interaction logic for WebViewTabItemHeader.xaml
    /// </summary>
    public partial class WebViewTabItemHeader : UserControl
    {
        public WebViewTabItemHeader()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(WebViewTabItemHeader), new UIPropertyMetadata(string.Empty));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = (TabItem)Parent;
            var tabControl = (TabControl)tabItem.Parent;
            tabControl.Items.Remove(tabItem);
        }
    }
}
