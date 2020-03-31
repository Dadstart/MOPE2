using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace B4.Mope
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void CommandBinding_HelpCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void CommandBinding_HelpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
		}

		private void CommandBinding_OpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void CommandBinding_OpenExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			using (var z = new ZipContainer("foo"))
			{
				var package = new Package(@"C:\temp\1.docx", @"C:\temp\x");
				package.Open();
			}
		}

		private void CommandBinding_SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			// TODO: Check dirty state
			e.CanExecute = true;
		}

		private void CommandBinding_SaveExecuted(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void CommandBinding_SaveAsCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			// TODO: Check for open file
			e.CanExecute = true;
		}

		private void CommandBinding_SaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
		{

		}
	}
}
