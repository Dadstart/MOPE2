using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope.Packaging
{
	public class Relationship
	{
		public Package Package { get; }
		public string Id { get; }
		public string Type { get; }
		public string Target { get; }

		public Relationship(Package package, string id, string type, string target)
		{
			//if (string.IsNullOrEmpty(id))
			//	throw new ArgumentException(nameof(id));

			//if (string.IsNullOrEmpty(type))
			//	throw new ArgumentException(nameof(type));

			//if (string.IsNullOrEmpty(target))
			//	throw new ArgumentException(nameof(target));
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Id = string.IsNullOrEmpty(id) ? throw new ArgumentNullException(nameof(id)) : id;
			Type = string.IsNullOrEmpty(type) ? throw new ArgumentNullException(nameof(type)) : type;
			Target = string.IsNullOrEmpty(target) ? throw new ArgumentNullException(nameof(target)) : target;
		}

	}
}
