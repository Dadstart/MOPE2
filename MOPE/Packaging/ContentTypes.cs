using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;

namespace B4.Mope.Packaging
{
	public class ContentTypes
	{
		public Dictionary<string, string> Defaults { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		public Dictionary<string, string> Overrides { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public void AddDefault(string ext, string contentType)
		{
			if (string.IsNullOrEmpty(ext))
				throw new ArgumentNullException(nameof(ext), "Invalid default content type extension");
			if (string.IsNullOrEmpty(contentType))
				throw new ArgumentNullException(nameof(contentType), "Invalid default content type");

			Defaults.Add(ext, contentType);
		}

		public void AddOverride(string partName, string contentType)
		{
			if (string.IsNullOrEmpty(partName))
				throw new ArgumentNullException(nameof(partName), "Invalid override content type part name");
			if (string.IsNullOrEmpty(contentType))
				throw new ArgumentNullException(nameof(contentType), "Invalid override content type");

			Overrides.Add(partName, contentType);
		}

		public static ContentTypes Load(Stream xml)
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

				if (string.Equals(elt.NamespaceURI, Namespaces.ContentTypes, StringComparison.Ordinal)
					&& string.Equals(elt.LocalName, "Types"))
				{
					typesRoot = elt;
					break;
				}
			}

			if (typesRoot == null)
				throw new Exception("Invalid [Content_Types].xml. Missing Types root element.");

			var contentTypes = new ContentTypes();

			// now parse content types
			foreach (XmlNode node in typesRoot.ChildNodes)
			{
				var elt = node as XmlElement;
				// ignore if not an element
				if (elt == null)
					continue;

				// ignore if not the right namespace
				if (!string.Equals(elt.NamespaceURI, Namespaces.ContentTypes, StringComparison.Ordinal))
					continue;

				switch (elt.LocalName)
				{
					case "Default":
						contentTypes.AddDefault(
							elt.GetAttribute("Extension"),
							elt.GetAttribute("ContentType"));
						break;
					case "Override":
						contentTypes.AddOverride(
							elt.GetAttribute("PartName"),
							elt.GetAttribute("ContentType"));
						break;
					default:
						continue;
				}
			}

			return contentTypes;
		}

		public string GetContentType(string uri)
		{
			if (string.IsNullOrEmpty(uri))
				throw new ArgumentNullException(nameof(uri));

			if (Overrides.ContainsKey(uri))
				return Overrides[uri];

			var ext = Path.GetExtension(uri).Substring(1);
			if (Defaults.ContainsKey(ext))
				return Defaults[ext];

			throw new Exception($"No known content type for {ext}");
		}
	}
}
