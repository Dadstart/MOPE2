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
		public ContentTypes ContentTypes { get; private set; }

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
			using (var stream = File.OpenRead(Path.Combine(m_tempDir, "[Content_Types].xml")))
			{
				ContentTypes = ContentTypes.Load(stream);
			}
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
