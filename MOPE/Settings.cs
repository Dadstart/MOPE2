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
			get { return GetValueOnMainThread<bool>(EditorReadOnlyModeProperty); }
			set { SetValueOnMainThread(EditorReadOnlyModeProperty, value); }
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
			get { return GetValueOnMainThread<bool>(EditorDarkModeProperty); }
			set { SetValueOnMainThread(EditorDarkModeProperty, value); }
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
			get { return GetValueOnMainThread<bool>(ConfirmOverwritePackageProperty); }
			set { SetValueOnMainThread(ConfirmOverwritePackageProperty, value); }
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
			get { return GetValueOnMainThread<bool>(EditorFormatXmlOnLoadProperty); }
			set { SetValueOnMainThread(EditorFormatXmlOnLoadProperty, value); }
		}
		private static void EditorFormatXmlOnLoadPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var settings = (Settings)obj;
			settings.m_appSettings.EditorFormatXmlOnLoad = (bool)e.NewValue;
			settings.m_appSettings.Save();
		}
		#endregion

		private void SetValueOnMainThread<T>(DependencyProperty prop, T value)
		{
			Dispatcher.Invoke(() => SetValue(prop, value));
		}

		private T GetValueOnMainThread<T>(DependencyProperty prop)
		{
			T value = default(T);
			Dispatcher.Invoke(() => value = (T)GetValue(prop));

			return value;
		}

		public Settings()
		{
			EditorReadOnlyMode = m_appSettings.EditorReadOnlyMode;
			EditorDarkMode = m_appSettings.EditorUseDarkMode;
			ConfirmOverwritePackage = m_appSettings.ConfirmOverwritePackage;
			EditorFormatXmlOnLoad = m_appSettings.EditorFormatXmlOnLoad;
		}
	}
}
