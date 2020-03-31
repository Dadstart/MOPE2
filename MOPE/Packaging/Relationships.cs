using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace B4.Mope.Packaging
{
	public class Relationships : Dictionary<string, Relationship>
	{
		public Package Package { get; }
		public string Source { get; }

		public Relationships(Package package, string source)
			: base(StringComparer.Ordinal)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Source = source;
		}

		public static Relationships Load(Package package, string source, Stream xml)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xml);

			XmlElement typesRoot = null;

			// get Types root element
			foreach (var node in xmlDoc.ChildNodes)
			{
				// ignore if not an element
				var elt = node as XmlElement;
				if (elt == null)
					continue;

				if (string.Equals(elt.NamespaceURI, Namespaces.Relationships, StringComparison.Ordinal)
					&& string.Equals(elt.LocalName, "Relationships"))
				{
					typesRoot = elt;
					break;
				}
			}

			if (typesRoot == null)
				throw new Exception("Invalid [Content_Types].xml. Missing Types root element.");

			var rels = new Relationships(package, source);

			// now parse content types
			foreach (XmlNode node in typesRoot.ChildNodes)
			{
				var elt = node as XmlElement;
				// ignore if not an element
				if (elt == null)
					continue;

				// ignore if not the right namespace
				if (!string.Equals(elt.NamespaceURI, Namespaces.Relationships, StringComparison.Ordinal)
					|| !string.Equals(elt.LocalName, "Relationship", StringComparison.Ordinal))
					continue;

				var rel = new Relationship(
					package,
					elt.GetAttribute("Id"),
					elt.GetAttribute("Type"),
					elt.GetAttribute("Target"));

				rels.Add(rel.Id, rel);
			}

			return rels;
		}

		public static string GetRelationshipPartOwner(string relsPartUri)
		{
			// find location of _rels/
			var relsDir = relsPartUri.LastIndexOf("_rels/");

			// find location of .rels
			var relsExt = relsPartUri.LastIndexOf(".rels");

			var uriPath = relsPartUri.Substring(0, relsDir);

			// check for root .rels (there will be no root path)
			if (uriPath.Length == 0)
				return null;

			var uriFileName = relsPartUri.Substring(relsDir + 6, relsExt - relsDir - 6);
			return string.Concat(uriPath, uriFileName);
		}

	}
}
