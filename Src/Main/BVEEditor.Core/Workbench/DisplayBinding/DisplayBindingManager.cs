/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 07/10/2013
 * Time: 15:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// The DisplayBindingManager will choose an appropriate view content style based on the type of given ViewModel.
	/// </summary>
	public class DisplayBindingManager
	{
		const string ProviderPath = "BVEEditor/Workbench/DisplayBindings";
		List<DisplayBindingDescriptor> descriptors;
		
		public DisplayBindingManager()
		{
			descriptors = AddInTree.BuildItems<DisplayBindingDescriptor>(ProviderPath, this, false);
		}
		
		/*public DataTemplate GetContentTemplate(FileName fileName)
		{
			foreach(var desc in GetDescriptorsForName(fileName)){
				if(desc.Binding.CanHandle(fileName) && desc.Binding.IsPreferredBindingForFile(fileName))
					return desc.Binding.ContentTemplate;
			}
			
			return null;
		}*/
		
		/// <summary>
		/// Gets all DisplaybindingDescriptors that match the file name.
		/// </summary>
		/// <param name="fileName">The file name to search for.</param>
		/// <returns></returns>
		IEnumerable<DisplayBindingDescriptor> GetDescriptorsForName(FileName fileName)
		{
			return descriptors.Where((d) => Regex.IsMatch(fileName.GetFileName(), d.FileNameRegex));
		}
	}
}
