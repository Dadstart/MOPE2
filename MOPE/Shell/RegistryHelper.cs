using Microsoft.Win32;

namespace B4.Mope.Shell
{
	public class RegistryHelper
	{
		public static string[] GetSubKeyList(RegistryKey parentKey, string subPath)
		{
			try
			{
				using (var regKey = parentKey.OpenSubKey(subPath))
				{
					string[] subKeys;
					if (regKey == null)
						return null;
					subKeys = regKey.GetSubKeyNames();
					regKey.Close();

					return subKeys;
				}

			}
			catch { }

			return new string[0];
		}

		public static string[] GetValueNames(RegistryKey parentKey, string subPath)
		{
			try
			{
				using (var regKey = parentKey.OpenSubKey(subPath))
				{
					string[] names;
					if (regKey == null)
						return null;
					names = regKey.GetValueNames();
					regKey.Close();

					return names;
				}

			}
			catch { }

			return new string[0];
		}

		public static string StringValueFromRegKey(RegistryKey parentKey, string subPath, string valueName)
		{
			object obj = ValueFromRegKey(parentKey, subPath, valueName);

			if (obj != null)
				return (string)obj;

			return null;
		}

		public static string StringValueFromRegKey(RegistryKey regKey, string valueName)
		{
			object obj = ValueFromRegKey(regKey, valueName);

			if (obj != null)
				return (string)obj;

			return null;
		}

		public static object ValueFromRegKey(RegistryKey regKey, string valueName)
		{
			try
			{
				object val;

				if (regKey == null)
					return null;

				val = regKey.GetValue(valueName);

				return val;
			}
			catch { }

			return null;
		}

		public static object ValueFromRegKey(RegistryKey parentKey, string subPath, string valueName)
		{
			RegistryKey regKey = null;

			try
			{
				object val;

				if (parentKey == null)
					return null;

				regKey = parentKey.OpenSubKey(subPath);
				if (regKey == null)
					return null;

				val = regKey.GetValue(valueName);

				return val;
			}
			catch { }

			if (regKey != null)
				regKey.Close();

			return null;
		}

		public static string ProgIdFromExtension(string extension)
		{
			object val = ValueFromRegKey(Registry.ClassesRoot, extension, string.Empty);

			if (val == null)
				return null;

			return val.ToString();
		}
	}
}
