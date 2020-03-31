using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace B4.Mope.Packaging
{
	public class Package : IDisposable
	{
		public string ZipFile { get; }
		public string TempDirectory { get; }
		public Dictionary<string, Part> Parts { get; } = new Dictionary<string, Part>();
		public ContentTypes ContentTypes { get; }
		public Relationships Relationships { get; }

		public Package()
		{

		}

		public Package(string path, string tempDir)
		{
			ZipFile = path;
			TempDirectory = tempDir;

			Directory.CreateDirectory(TempDirectory);
			var entries = ZipContainer.ExtractTo(ZipFile, TempDirectory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			// read [Content_Types].xml
			ContentTypes = LoadContentTypes();

			// loop through entries and create parts
			foreach (var entry in entries)
			{
				var part = new Part(this, entry.Name, entry.FullName, ContentTypes.GetContentType(entry.FullName), entry.Crc32);
				Parts.Add(part.Uri, part);
			}
		}

		/// <summary>
		/// Load content types from xml file
		/// </summary>
		ContentTypes LoadContentTypes()
		{
			using (var stream = File.OpenRead(Path.Combine(TempDirectory, "[Content_Types].xml")))
			{
				return ContentTypes.Load(stream);
			}
		}

		/// <summary>
		/// Load all relationships returning root relationships
		/// </summary>
		/// <returns></returns>
		Relationships LoadRelationships()
		{
			Relationships rootRels;

			// first load root
			var rootRelsFile = Parts["_rels/.rels"].GetFileInfo();
			using (var stream = rootRelsFile.OpenRead())
			{
				rootRels = Relationships.Load(this, null, stream);
			}

			// now loop through all .rels parts
			foreach (var part in Parts.Values.Where(p => p.Name.EndsWith(".rels")))
			{
				Relationships rels;
				string source = Relationships.GetRelationshipPartOwner(part.Uri);
				using (var stream = part.GetFileInfo().OpenRead())
				{
					rels = Relationships.Load(this, source, stream);
				}
			}

			return rootRels;
		}

		public FileInfo GetPartFileInfo(string uri)
		{
			if (string.IsNullOrEmpty(uri))
				throw new ArgumentNullException(nameof(uri));

			var part = Parts[uri];
			var fullPath = Path.Combine(TempDirectory, part.Uri.Replace('/', '\\'));
			return new FileInfo(fullPath);
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
