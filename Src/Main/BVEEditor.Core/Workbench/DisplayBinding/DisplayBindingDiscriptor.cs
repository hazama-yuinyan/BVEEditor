/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/17
 * Time: 14:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// Describes a DisplayBinding.
    /// </summary>
	public class DisplayBindingDescriptor
	{
		IDisplayBinding binding;
		Codon codon;
		
		public IDisplayBinding Binding {
			get {
				if(codon != null && binding == null)
					binding = (IDisplayBinding)codon.AddIn.CreateObject(codon.Properties["class"]);
				
				return binding;
			}
		}
		
		public string Id { get; set; }
		public string Title { get; set; }
		public string FileNameRegex { get; set; }
		
		public DisplayBindingDescriptor(Codon codon)
		{
			if(codon == null)
				throw new ArgumentNullException("codon");
			
			this.codon = codon;
			this.Id = codon.Id;
			
			string title = codon.Properties["title"];
			if(string.IsNullOrEmpty(title))
				this.Title = codon.Id;
			else
				this.Title = title;
			
			this.FileNameRegex = codon.Properties["fileNamePattern"];
		}
		
		public DisplayBindingDescriptor(IDisplayBinding binding)
		{
			if(binding == null)
				throw new ArgumentNullException("binding");
			
			this.binding = binding;
		}
		
		/// <summary>
		/// Gets if the display binding can possibly open the file.
		/// If this method returns false, it cannot open it; if the method returns
		/// true, it *might* open it.
		/// Call Binding.CanCreateContentForFile() to know for sure if the binding
		/// will open the file.
		/// </summary>
		/// <remarks>
		/// This method is used to skip loading addins like the ResourceEditor which cannot
		/// attach to a certain file name for sure.
		/// </remarks>
		public bool CanOpenFile(string fileName)
		{
			string fileNameRegex = StringParser.Parse(this.FileNameRegex);
			if(fileNameRegex == null || fileNameRegex.Length == 0) // no regex specified
				return true;
			if(fileName == null) // regex specified but file has no name
				return false;
			return Regex.IsMatch(fileName, fileNameRegex, RegexOptions.IgnoreCase);
		}
		
		public override string ToString()
		{
			return string.Format("[DisplayBindingDescriptor Id={1} Binding={0}]", binding, Id);
		}
	}
}
