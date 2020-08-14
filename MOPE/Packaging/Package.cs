using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

		public Package(string path, string tempDirPath)
		{
			ZipFile = path;
			TempDirectory = tempDirPath;

			if (Directory.Exists(TempDirectory))
				Directory.Delete(TempDirectory, true);

			var tempDir = Directory.CreateDirectory(TempDirectory);

			var entries = ZipContainer.ExtractTo(ZipFile, TempDirectory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			// read [Content_Types].xml
			ContentTypes = LoadContentTypes();

			// loop through entries and create parts
			foreach (var entry in entries)
			{
				var part = new Part(this, entry.Name, entry.FullName, ContentTypes.GetContentType(entry.FullName), entry.Crc32, entry.Length, entry.CompressedLength);
				Parts.Add(part.Uri, part);
			}

			Relationships = LoadRelationships();
		}

		/// <summary>
		/// Close package and free resources
		/// </summary>
		public void Close()
		{
			Dispose();
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
					part.Relationships = rels;
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

				// free unmanaged resources
				if (Directory.Exists(TempDirectory))
				{
					try
					{
						Directory.Delete(TempDirectory, true);
					}
					catch
					{
						// we did our best, swallow exception
					}
				}

				m_isDisposed = true;
			}
		}

		/// <summary>
		/// Finalizer
		/// </summary>
		~Package()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

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
