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
    /// Interaction logic for CloseButtonTabHeader.xaml
    /// </summary>
    public partial class CloseButtonTabHeader : UserControl
    {
        public CloseButtonTabHeader()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CloseButtonTabHeader), new UIPropertyMetadata(string.Empty));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ViewTypeProperty = DependencyProperty.Register("ViewType", typeof(string), typeof(CloseButtonTabHeader), new UIPropertyMetadata(string.Empty));
        public string ViewType
        {
            get { return (string)GetValue(ViewTypeProperty); }
            set { SetValue(ViewTypeProperty, value); }
        }

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(CloseButtonTabHeader), new UIPropertyMetadata(false));
        public bool IsDirty
		{
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
		}

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = (TabItem)Parent;
            var tabControl = (TabControl)tabItem.Parent;
            tabControl.Items.Remove(tabItem);
        }
    }
}
