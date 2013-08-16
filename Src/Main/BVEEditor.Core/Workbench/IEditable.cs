/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 06/30/2013
 * Time: 21:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// Denotes derived classes that it can create a snapshot which represents the content at the time the method is called.
    /// </summary>
	public interface IEditable
	{
		/// <summary>
		/// Creates a snapshot of the editor content.
		/// </summary>
		ITextSource CreateSnapshot();
		
		/// <summary>
		/// Gets the text in the view content.
		/// </summary>
		string Text {
			get;
		}
	}
}
