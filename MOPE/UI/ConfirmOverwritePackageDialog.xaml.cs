﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace B4.Mope.UI
{
	/// <summary>
	/// Interaction logic for ConfirmOverwritePackageDialog.xaml
	/// </summary>
	public partial class ConfirmOverwritePackageDialog : Window
	{
		public MessageBoxResult Result { get; private set; }
		public ConfirmOverwritePackageDialog()
		{
			InitializeComponent();
		}

		private void buttonIgnore_Click(object sender, RoutedEventArgs e)
		{
		}

		private void buttonDiscard_Click(object sender, RoutedEventArgs e)
		{
		}

		private void buttonDiff_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
