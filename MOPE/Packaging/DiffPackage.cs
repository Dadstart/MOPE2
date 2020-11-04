using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope.Packaging
{
	public class DiffPackage
	{
		public Package Left { get; }
		public Package Right { get; }

		public DiffPackage(Package left, Package right)
		{
			Left = left ?? throw new ArgumentNullException(nameof(left));
			Right = right ?? throw new ArgumentNullException(nameof(right));
		}
	}
}
