using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace B4.Mope.UI
{
	public static class CustomCommands
	{
		public static readonly RoutedUICommand SaveAs = new RoutedUICommand(
			"Save As",
			"SaveAs",
			typeof(CustomCommands),
			new InputGestureCollection() { new KeyGesture(Key.F12) });

		public static readonly RoutedUICommand SavePackage = new RoutedUICommand(
			"Save Package",
			"SavePackage",
			typeof(CustomCommands),
			new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });

		public static readonly RoutedUICommand SavePackageAs = new RoutedUICommand(
			"Save Package As",
			"SavePackageAs",
			typeof(CustomCommands),
			new InputGestureCollection() { new KeyGesture(Key.F12, ModifierKeys.Control | ModifierKeys.Shift) });
	}
}
