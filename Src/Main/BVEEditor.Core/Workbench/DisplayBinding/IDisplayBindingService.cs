/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/19
 * Time: 16:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// Manages the list of display bindings, and the user's default settings (for Open With dialog)
	/// </summary>
	public interface IDisplayBindingService
	{
		/// <summary>
		/// Attach secondary view contents to the view content.
		/// </summary>
		/// <param name="viewContent">The view content to attach to</param>
		/// <param name="isReattaching">This is a reattaching pass</param>
		//void AttachSubWindows(IViewContent viewContent, bool isReattaching);
		
		/// <summary>
		/// Gets the primary display binding for the specified file name.
		/// </summary>
		IDisplayBinding GetBindingPerFileName(FileName filename);
		
		/// <summary>
		/// Gets the default primary display binding for the specified file name.
		/// </summary>
		DisplayBindingDescriptor GetDefaultCodonPerFileName(FileName filename);
		
		/// <summary>
		/// Sets the default display binding for the specified file extension.
		/// </summary>
		void SetDefaultCodon(string extension, DisplayBindingDescriptor bindingDescriptor);
		
		/// <summary>
		/// Gets list of possible primary display bindings for the specified file name.
		/// </summary>
		IReadOnlyList<DisplayBindingDescriptor> GetCodonsPerFileName(FileName filename);
		
		//DisplayBindingDescriptor AddExternalProcessDisplayBinding(ExternalProcessDisplayBinding binding);
		//void RemoveExternalProcessDisplayBinding(ExternalProcessDisplayBinding binding);
	}
}
