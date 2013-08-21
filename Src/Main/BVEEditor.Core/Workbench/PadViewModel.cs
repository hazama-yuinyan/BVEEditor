/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/07/11
 * Time: 12:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// A PadViewModel takes care of a pad.
	/// </summary>
	public abstract class PadViewModel : PaneViewModel
	{
		public string Name{
			get;
			private set;
		}
		
		public PadViewModel(string name)
		{
			Name = name;
			DisplayName = name;
		}
	}
}
