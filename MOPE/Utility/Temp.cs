using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B4.Mope.Utility
{
    public static class Temp
    {
		private static Random s_random = new Random();
		public static string GetRootTempDir()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "MOPE2");
			if (!Directory.Exists(tempDir))
				Directory.CreateDirectory(tempDir);

			return tempDir;
		}

		/// <summary>
		/// Generate a unique name in the temp dir that can be used as dir or file name
		/// </summary>
		public static string GetTempName(string prefix, out string path)
		{
			var rootTempDir = GetRootTempDir();
			string name;

			do
			{
				var rand = s_random.Next(0xFFFFFFF);
				name = rand.ToString("X7");
				if (prefix != null)
					name = $"{prefix}.{name}";
				path = Path.Combine(rootTempDir, name);
			} while (Directory.Exists(path) || File.Exists(path));

			return name;
		}

		/// <summary>
		/// Get a temp directory
		/// </summary>
		public static string GetTempDir(string prefix = null)
		{
			GetTempName(prefix, out string path);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}

	}
}
