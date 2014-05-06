/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 06/30/2013
 * Time: 23:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ICSharpCode.Core;

namespace BVEEditor.Editor
{
	/// <summary>
	/// Provides the BASE-version for a given file - the original unmodified
	/// copy from the last commit.
	/// This interface is implemented by the version control AddIns.
	/// </summary>
	public interface IDocumentVersionProvider
	{
		/// <summary>
		/// Provides the BASE-Version for a file. This can be either the file saved
		/// to disk or a base version provided by any VCS.
		/// </summary>
		Task<Stream> OpenBaseVersionAsync(FileName fileName);
		
		/// <summary>
		/// Starts watching for changes to the BASE-version of the specified file.
		/// The callback delegate gets called whenever the BASE-version has changed.
		/// </summary>
		/// <returns>Returns a disposable that can be used to stop watching for changes.
		/// You must dispose the disposable to prevent a memory leak, the GC will
		/// not help out in this case!</returns>
		IDisposable WatchBaseVersionChanges(FileName fileName, EventHandler callback);
	}
	
	public class VersioningServices
	{
		public static readonly VersioningServices Instance = new VersioningServices();
		
		List<IDocumentVersionProvider> baseVersionProviders;
		
		public List<IDocumentVersionProvider> DocumentVersionProviders {
			get {
				if (baseVersionProviders == null)
					baseVersionProviders = AddInTree.BuildItems<IDocumentVersionProvider>("/Workspace/DocumentVersionProviders", this, false);
				
				return baseVersionProviders;
			}
		}
	}
}
