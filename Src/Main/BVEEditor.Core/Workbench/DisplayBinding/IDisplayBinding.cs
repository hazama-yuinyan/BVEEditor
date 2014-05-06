/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/17
 * Time: 14:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Windows;
using ICSharpCode.Core;
using Xceed.Wpf.AvalonDock.Layout;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// This class defines the BVEEditor display binding interface, it is a factory
	/// structure, which creates a view model.
	/// </summary>
	public interface IDisplayBinding
	{
		bool IsPreferredBindingForFile(FileName fileName);
		
		/// <remarks>
		/// This function determines, if this display binding is able to create
		/// a ViewContent for the file given by fileName.
		/// </remarks>
		/// <returns>
		/// true, if this display binding is able to create
		/// a ViewContent for the file given by fileName.
		/// false otherwise
		/// </returns>
		bool CanHandle(FileName fileName);
		
		double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType);
		
		/// <remarks>
		/// Creates a new ViewDocumentViewModel object for the file fileName
		/// </remarks>
		/// <returns>
		/// A newly created ViewDocumentViewModel object with ViewContent already set up.
		/// </returns>
		ViewDocumentViewModel CreateViewModelForFile(FileName path);
	}
}
