using System;
using System.Collections.Generic;
using System.Text;

namespace B4.Mope.Packaging
{
	public class Relationship
	{
		/// <summary>
		/// Package
		/// </summary>
		public Package Package { get; }

		/// <summary>
		/// Source part Uri
		/// </summary>
		public string Source { get; }

		/// <summary>
		/// Relationship Id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Relationship Type
		/// </summary>
		public string Type { get; }

		/// <summary>
		/// Target part Uri
		/// </summary>
		public string Target { get; }

		/// <summary>
		/// External or internal relationship
		/// </summary>
		public bool External { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		public Relationship(Package package, string source, string id, string type, string target, bool external = false)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Source = source;
			Id = string.IsNullOrEmpty(id) ? throw new ArgumentNullException(nameof(id)) : id;
			Type = string.IsNullOrEmpty(type) ? throw new ArgumentNullException(nameof(type)) : type;
			Target = string.IsNullOrEmpty(target) ? throw new ArgumentNullException(nameof(target)) : target;
			External = external;
		}

		/// <summary>
		/// Get source part
		/// </summary>
		public Part GetSource()
		{
			// root relationships have no source
			if (Source == null)
				return null;

			return Package.Parts[Source];
		}

		/// <summary>
		/// Get target part
		/// </summary>
		public Part GetTarget()
		{
			return Package.Parts[Target];
		}

	}
}
