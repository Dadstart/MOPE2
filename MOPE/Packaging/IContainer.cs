using System;
using System.Collections.Generic;

namespace B4.Mope.Packaging
{
	public interface IContainer : IDisposable
	{
		bool IsOpen();

		/// <summary>
		/// Closes the package.
		/// </summary>
		void Close();

		/// <summary>
		/// Get a list of all files in the package.
		/// </summary>
		/// <returns>An array of file URIs</returns>
		IEnumerable<string> GetFiles();

		/// <summary>
		/// Copy all files in the package to the specified path.
		/// </summary>
		/// <param name="path">The directory path to copy the files to.</param>
		/// <returns>true if success, else false</returns>
		bool CopyAllFilesTo(string path);

		/// <summary>
		/// Copy all specified files from the package
		/// </summary>
		/// <param name="uris">The files in the package to copy.</param>
		/// <param name="path">The directory path to copy the files to.</param>
		/// <returns>true if success, else false</returns>
		bool CopyFilesTo(string[] uris, string path);

		/// <summary>
		/// Copy the specified file in the package to the path.
		/// </summary>
		/// <param name="path">The path to copy the file to.</param>
		/// <param name="uri">The uri of the file to copy</param>
		/// <returns>true if success, else false</returns>
		bool CopyFileTo(string path, string uri);

		/// <summary>
		/// Copy all files from the specified path into the package.
		/// </summary>
		/// <param name="path">The directory path from which to copy files from</param>
		/// <returns>true if success, else false</returns>
		bool CopyFilesFrom(string path);

		/// <summary>
		/// Copy all specified files from the package
		/// </summary>
		/// <param name="files">The list of files to copy</param>
		/// <param name="uris">The destination uris in the package</param>
		/// <returns>true if success, else false</returns>
		bool CopyFilesFrom(string[] files, string[] uris);

		/// <summary>
		/// Copy the specified file into the package
		/// </summary>
		/// <param name="path">The path to copy the file to.</param>
		/// <param name="uri">The uri of the file to copy</param>
		/// <returns>true if success, else false</returns>
		bool CopyFileFrom(string path, string uri);

		/// <summary>
		/// Remove the specified file from the package.
		/// </summary>
		/// <param name="uri">The uri to remove</param>
		/// <returns>true if success, else false</returns>
		bool RemoveFile(string uri);

		/// <summary>
		/// Remove the specified files from the package
		/// </summary>
		/// <param name="uris">URIs to remove</param>
		/// <returns>true if success, else false</returns>
		bool RemoveFiles(string[] uris);
	}
}
