using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope.Utility
{
	public static class HashSetExtensionMethods
	{
		/// <summary>
		/// Add multiple values to a hash set
		/// </summary>
		public static void AddValues<T>(this HashSet<T> hashSet, IEnumerable<T> values)
		{
			if (values == null)
				return;

			foreach (var value in values)
				hashSet.Add(value);
		}
	}
}
