using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace B4.Mope
{
	public class Settings : DependencyObject
	{
		private readonly AppSettings m_appSettings = new AppSettings();

		public delegate void BooleanPropertyChangedEventHandler(object sender, BooleanPropertyChangedEventArgs e);

		#region EditorReadOnlyMode
		public event BooleanPropertyChangedEventHandler EditorReadOnlyModeChanged;
		private static readonly DependencyProperty EditorReadOnlyModeProperty = DependencyProperty.Register("EditorReadOnlyMode", typeof(bool), typeof(Settings), new PropertyMetadata(false, EditorReadOnlyPropertyChanged));
		public bool EditorReadOnlyMode
		{
			get { return (bool)GetValue(EditorReadOnlyModeProperty); }
			set { SetValue(EditorReadOnlyModeProperty, value); }
		}

		private static void EditorReadOnlyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var settings = (Settings)obj;
			settings.m_appSettings.EditorReadOnlyMode = (bool)e.NewValue;
			settings.m_appSettings.Save();
			settings.EditorReadOnlyModeChanged?.Invoke(settings, new BooleanPropertyChangedEventArgs((bool)e.OldValue, (bool)e.NewValue));
		}
		#endregion

		#region EditorDarkMode
		public event BooleanPropertyChangedEventHandler EditorDarkModeChanged;
		private static readonly DependencyProperty EditorDarkModeProperty = DependencyProperty.Register("EditorDarkMode", typeof(bool), typeof(Settings), new PropertyMetadata(false, EditorDarkModePropertyChanged));
		public bool EditorDarkMode
		{
			get { return (bool)GetValue(EditorDarkModeProperty); }
			set { SetValue(EditorDarkModeProperty, value); }
		}
		private static void EditorDarkModePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var settings = (Settings)obj;
			settings.m_appSettings.EditorUseDarkMode = (bool)e.NewValue;
			settings.m_appSettings.Save();
			settings.EditorDarkModeChanged?.Invoke(settings, new BooleanPropertyChangedEventArgs((bool)e.OldValue, (bool)e.NewValue));
		}
		#endregion

		#region ConfirmOverwritePackage
		private static readonly DependencyProperty ConfirmOverwritePackageProperty = DependencyProperty.Register("ConfirmOverwritePackage", typeof(bool), typeof(Settings), new PropertyMetadata(false, ConfirmOverwritePackagePropertyChanged));
		public bool ConfirmOverwritePackage
		{
			get { return (bool)GetValue(ConfirmOverwritePackageProperty); }
			set { SetValue(ConfirmOverwritePackageProperty, value); }
		}
		private static void ConfirmOverwritePackagePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var settings = (Settings)obj;
			settings.m_appSettings.ConfirmOverwritePackage = (bool)e.NewValue;
			settings.m_appSettings.Save();
		}
		#endregion

		#region EditorFormatXmlOnLoad
		private static readonly DependencyProperty EditorFormatXmlOnLoadProperty = DependencyProperty.Register("EditorFormatXmlOnLoad", typeof(bool), typeof(Settings), new PropertyMetadata(false, EditorFormatXmlOnLoadPropertyChanged));

		public bool EditorFormatXmlOnLoad
		{
			get { return (bool)GetValue(EditorFormatXmlOnLoadProperty); }
			set { SetValue(EditorFormatXmlOnLoadProperty, value); }
		}
		private static void EditorFormatXmlOnLoadPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var settings = (Settings)obj;
			settings.m_appSettings.EditorFormatXmlOnLoad = (bool)e.NewValue;
			settings.m_appSettings.Save();
		}
		#endregion

		public Settings()
		{
			EditorReadOnlyMode = m_appSettings.EditorReadOnlyMode;
			EditorDarkMode = m_appSettings.EditorUseDarkMode;
			ConfirmOverwritePackage = m_appSettings.ConfirmOverwritePackage;
			EditorFormatXmlOnLoad = m_appSettings.EditorFormatXmlOnLoad;
		}
	}
}
