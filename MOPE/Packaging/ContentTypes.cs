using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace B4.Mope.Packaging
{
	public class ContentTypes
	{
		public Dictionary<string, string> Defaults { get; } = new Dictionary<string, string>(StringComparer.Ordinal);
		public Dictionary<string, string> Overrides { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

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

			Defaults.Add(partName, contentType)''
		}

		public static ContentTypes Load(string filename)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filename);

			XmlElement typesRoot = null;

			// get Types root element
			foreach (var node in xmlDoc.ChildNodes)
			{
				// ignore if not an element
				var elt = node as XmlElement;
				if (elt == null)
					continue;

				if (string.Equals(elt.NamespaceURI, Namespaces.ContentTypes, StringComparison.Ordinal))
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
							elt.GetAttribute("Extension", Namespaces.ContentTypes),
							elt.GetAttribute("ContentType", Namespaces.ContentTypes));
						break;
					case "Override":
						contentTypes.AddOverride(
							elt.GetAttribute("PartName", Namespaces.ContentTypes),
							elt.GetAttribute("ContentType", Namespaces.ContentTypes));
						break;
					default:
						continue;
				}
			}

			return contentTypes;
		}
	}
}
