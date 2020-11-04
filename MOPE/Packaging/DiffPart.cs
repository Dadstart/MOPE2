using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope.Packaging
{
	public class DiffPart
	{
		public Part Left { get; }
		public Part Right { get; }

		public DiffPart(Part left, Part right)
		{
			// must have at least one part
			if ((left == null) && (right == null))
			{
				throw new ArgumentNullException();
			}

			Left = left;
			Right = right;
		}
	}
}
