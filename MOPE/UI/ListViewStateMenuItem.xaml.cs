using System;
using System.Collections.Generic;
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

namespace B4.Mope.UI
{
	/// <summary>
	/// Interaction logic for ListViewStateMenuItem.xaml
	/// </summary>
	public partial class ListViewStateMenuItem : MenuItem
	{
		public static readonly DependencyProperty ViewStateProperty = DependencyProperty.Register("ViewState", typeof(ListViewState), typeof(ListViewStateMenuItem), new UIPropertyMetadata(ListViewState.Default));
		public ListViewState ViewState
		{
			get { return (ListViewState)GetValue(ViewStateProperty); }
			set { SetValue(ViewStateProperty, value); }
		}

		public ListViewStateMenuItem()
		{
			InitializeComponent();
			IsCheckable = true;
		}
	}
}
