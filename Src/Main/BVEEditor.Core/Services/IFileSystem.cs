/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/19
 * Time: 16:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.Core;

namespace BVEEditor.Services
{
	/// <summary>
	/// A wrapper around commonly used System.IO methods
	/// for accessing the file system. Allows mocking file system access in unit tests.
	/// </summary>
	public interface IFileSystem : IReadOnlyFileSystem
	{
		/// <inheritdoc cref="System.IO.File.Delete"/>
		void Delete(FileName path);
		
		/// <inheritdoc cref="System.IO.File.CopyFile"/>
		void CopyFile(FileName source, FileName destination, bool overwrite = false);
		
		/// <inheritdoc cref="System.IO.Directory.Delete"/>
		void CreateDirectory(DirectoryName path);
		
		/// <inheritdoc cref="System.IO.File.OpenWrite"/>
		Stream OpenWrite(FileName fileName);

        /// <inheritdoc cref="System.IO.File.WriteAllText"/>
        void WriteAllText(FileName path, string content);

        /// <inheritdoc cref="System.IO.File.AppendAllText"/>
        void AppendAllText(FileName path, string content);
	}
	
	public interface IReadOnlyFileSystem
	{
		/// <inheritdoc cref="System.IO.File.Exists"/>
		bool FileExists(FileName path);
		
		/// <inheritdoc cref="System.IO.Directory.Exists"/>
		bool DirectoryExists(DirectoryName path);
		
		/// <inheritdoc cref="System.IO.File.OpenRead"/>
		Stream OpenRead(FileName fileName);
		
		/// <inheritdoc cref="System.IO.File.OpenText"/>
		TextReader OpenText(FileName fileName);
		
		IEnumerable<FileName> GetFiles(DirectoryName directory, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

        Encoding DefaultFileEncoding{get;}

        EncodingInfo DefaultFileEncodingInfo{get; set;}

        IReadOnlyList<EncodingInfo> AllEncodings{get;}
	}
	
	public static class FileSystemExtensions
	{
		public static string ReadAllText(this IReadOnlyFileSystem fileSystem, FileName fileName)
		{
			using (var reader = fileSystem.OpenText(fileName)) {
				return reader.ReadToEnd();
			}
		}
	}
}
