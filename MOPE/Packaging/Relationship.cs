using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope.Packaging
{
	public class Relationship
	{
		public string Id { get; }
		public string Type { get; }
		public string Target { get; }

		public Relationship(string id, string type, string target)
		{
			if (string.IsNullOrEmpty(id))
				throw new ArgumentException(nameof(id));

			if (string.IsNullOrEmpty(type))
				throw new ArgumentException(nameof(type));

			if (string.IsNullOrEmpty(target))
				throw new ArgumentException(nameof(target));

			Id = id;
			Type = type;
			Target = target;
		}

	}
}
