/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 06/30/2013
 * Time: 20:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace BVEEditor.Options
{
	public interface IOptionPanel
	{
        string Title{get; set;}
        IList<OptionPanelViewModel> Children{get;}
		void LoadOptions();
		bool SaveOptions();
	}
}
