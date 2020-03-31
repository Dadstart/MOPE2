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
		public Relationships()
			: base(StringComparer.Ordinal)
		{

		}

		public static Relationships Load(Stream xml)
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

			var rels = new Relationships();

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
					elt.GetAttribute("Id"),
					elt.GetAttribute("Type"),
					elt.GetAttribute("Target"));

				rels.Add(rel.Id, rel);
			}

			return rels;
		}

	}
}
