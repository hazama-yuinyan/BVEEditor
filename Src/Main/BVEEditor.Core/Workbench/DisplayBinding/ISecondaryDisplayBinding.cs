/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/20
 * Time: 14:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// This class defines the SharpDevelop display binding interface, it is a factory
	/// structure, which creates IViewContents.
	/// </summary>
	public interface ISecondaryDisplayBinding
	{
		/// <summary>
		/// Gets if the display binding can attach to the specified view content.
		/// </summary>
		bool CanAttachTo(ViewDocumentViewModel content);
		
		/// <summary>
		/// When you return true for this property, the CreateSecondaryViewContent method
		/// is called again after the LoadSolutionProjects thread has finished.
		/// </summary>
		bool ReattachWhenParserServiceIsReady { get; }
		
		/// <summary>
		/// Creates the secondary view contents for the given view content.
		/// If ReattachWhenParserServiceIsReady is used, the implementation is responsible
		/// for checking that no duplicate secondary view contents are added.
		/// </summary>
		ViewContentViewModel[] CreateSecondaryViewContent(ViewDocumentViewModel viewDocument);
		
		DataTemplate GetContentStyle();
	}
}
