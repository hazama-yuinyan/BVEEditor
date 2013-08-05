/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/16
 * Time: 19:32
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;

namespace Core.Presentation.Menu
{
	/// <summary>
	/// Used to include a dynamically built list of menu items.
	/// </summary>
	public interface IMenuItemBuilder
	{
		ICollection BuildItems(ICSharpCode.Core.Codon codon, object owner);
	}
}
