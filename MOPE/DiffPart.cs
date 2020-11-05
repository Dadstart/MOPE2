using B4.Mope.Packaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope
{
	public class DiffPart
	{
		public string Uri { get; }
		public Part Left { get; }
		public Part Right { get; }

		public DiffPart(string uri, Part left, Part right)
		{
			// must have at least one part
			if ((left == null) && (right == null))
			{
				throw new ArgumentNullException();
			}
			Uri = uri ?? throw new ArgumentNullException(nameof(uri));
			Left = left;
			Right = right;
		}
	}
}
