using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace B4.Mope.Packaging
{
	public class ZipContainer : IContainer
	{
		private ZipArchive m_archive;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="tempDirectory">Directory to store temporary files</param>
		public ZipContainer(string tempDirectory)
		{
		}

		/// <summary>
		/// Opens the specified path.
		/// </summary>
		/// <param name="file">The path and file name of the package.</param>
		/// <param name="outputDir">Output directory</param>
		/// <param name="mode">The file mode in which to open the package.</param>
		/// <param name="access">The file access in which to open with package.</param>
		/// <param name="share">The sharing access other apps have</param>
		/// <returns>Collection of zip entries indexed by relative URI</returns>
		public static IEnumerable<ZipArchiveEntry> ExtractTo(string file, string outputDir, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.ReadWrite)
		{
			using (var archive = new ZipArchive(File.Open(file, mode, access, share)))
			{
				Directory.CreateDirectory(outputDir);
				foreach (var entry in archive.Entries)
				{
					string relativePath = entry.FullName.Replace('/', '\\');
					string outputFile = Path.Combine(outputDir, relativePath);
					string directory = outputFile.Substring(0, outputFile.Length - entry.Name.Length - 1);
					Directory.CreateDirectory(directory);
					entry.ExtractToFile(outputFile, overwrite: true);
				}

				return archive.Entries;
			}
		}

		/// <summary>
		/// Closes the package.
		/// </summary>
		public void Close()
		{
			m_archive.Dispose();
			m_archive = null;
		}

		/// <summary>
		/// Is this backed by an open archive?
		/// </summary>
		/// <returns></returns>
		public bool IsOpen()
		{
			return m_archive != null;
		}

		/// <summary>
		/// Copy all files in the package to the specified path.
		/// </summary>
		/// <param name="path">The directory path to copy the files to.</param>
		/// <returns>true if success, else false</returns>
		public bool CopyAllFilesTo(string path)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Copy all specified files from the package
		/// </summary>
		/// <param name="uris">The files in the package to copy.</param>
		/// <param name="path">The directory path to copy the files to.</param>
		/// <returns>true if success, else false</returns>
		public bool CopyFilesTo(string[] uris, string path)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Copy the specified file in the package to the path.
		/// </summary>
		/// <param name="path">The path to copy the file to.</param>
		/// <param name="uri">The uri of the file to copy</param>
		/// <returns>true if success, else false</returns>
		public bool CopyFileTo(string path, string uri)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Copy all files from the specified path into the package.
		/// </summary>
		/// <param name="path">The directory path from which to copy files from</param>
		/// <returns>true if success, else false</returns>
		public bool CopyFilesFrom(string path)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Copy all specified files from the package
		/// </summary>
		/// <param name="files">The list of files to copy</param>
		/// <param name="uris">The destination uris in the package</param>
		/// <returns>true if success, else false</returns>
		public bool CopyFilesFrom(string[] files, string[] uris)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Copy the specified file into the package
		/// </summary>
		/// <param name="path">The path to copy the file to.</param>
		/// <param name="uri">The uri of the file to copy</param>
		/// <returns>true if success, else false</returns>
		public bool CopyFileFrom(string path, string uri)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get a list of all files in the package.
		/// </summary>
		/// <returns>An array of file URIs</returns>
		public IEnumerable<string> GetFiles()
		{
			return null;
		}

		/// <summary>
		/// Remove the specified file from the package.
		/// </summary>
		/// <param name="uri">The uri to remove</param>
		/// <returns>true if success, else false</returns>
		public bool RemoveFile(string uri)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Remove the specified files from the package
		/// </summary>
		/// <param name="uris">URIs to remove</param>
		/// <returns>true if success, else false</returns>
		public bool RemoveFiles(string[] uris)
		{
			throw new NotImplementedException();
		}

		#region IDisposable Support
		private bool m_isDisposed = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!m_isDisposed)
			{
				if (disposing)
				{
					if (m_archive != null)
					{
						m_archive.Dispose();
						m_archive = null;
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.

				m_isDisposed = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ZipContainer()
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
