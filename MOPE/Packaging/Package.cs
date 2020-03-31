using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace B4.Mope.Packaging
{
	public class Package : IDisposable
	{
		private string m_zipFile;
		private string m_tempDir;
		public Dictionary<string, Part> Parts { get; } = new Dictionary<string, Part>();
		public Dictionary<string, string> ContentTypes { get; private set; }

		public Package(string path, string tempDir)
		{
			this.m_zipFile = path;
			this.m_tempDir = tempDir;
		}

		public void Open()
		{
			Directory.CreateDirectory(m_tempDir);
			var entries = ZipContainer.ExtractTo(m_zipFile, m_tempDir, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			// read [Content_Types].xml
			LoadContentTypes();

		}

		/// <summary>
		/// Load content types from xml file
		/// </summary>
		void LoadContentTypes()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(Path.Combine(m_tempDir, "[Content_Types].xml"));

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

			var contentTypes = new Dictionary<string, string>();

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

				string key;
				string contentType;

				switch (elt.LocalName)
				{
					case "Default":
						key = elt.GetAttribute("Extension", Namespaces.ContentTypes);
						contentType = elt.GetAttribute("ContentType", Namespaces.ContentTypes);
						break;
					case "Override":
						key = elt.GetAttribute("PartName", Namespaces.ContentTypes);
						contentType = elt.GetAttribute("ContentType", Namespaces.ContentTypes);
						break;
					default:
						continue;
				}

				if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(contentType))
					throw new Exception("Invalid content type entry");

				contentTypes.Add(key, contentType);
			}

			ContentTypes = contentTypes;
		}

		#region IDisposable Support
		private bool m_isDisposed = false; // To detect redundant calls
		protected virtual void Dispose(bool disposing)
		{
			if (!m_isDisposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				m_isDisposed = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~Package()
		// {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
