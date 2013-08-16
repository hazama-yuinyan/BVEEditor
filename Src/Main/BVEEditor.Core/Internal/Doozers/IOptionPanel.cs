/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 06/30/2013
 * Time: 20:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor
{
	public interface IOptionPanel
	{
		/// <summary>
		/// Gets/sets the owner (the context object used when building the option panels
		/// from the addin-tree). This is null for IDE options or the IProject instance for project options.
		/// </summary>
		object Owner { get; set; }
		
		object Control {
			get;
		}
		
		void LoadOptions();
		bool SaveOptions();
	}
}
