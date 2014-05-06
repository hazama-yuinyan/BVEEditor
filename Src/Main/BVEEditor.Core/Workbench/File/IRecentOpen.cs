/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/17
 * Time: 14:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	/// <see cref="IFileService.RecentOpen"/>
	public interface IRecentOpen
	{
		IReadOnlyList<FileName> RecentFiles { get; }
		IReadOnlyList<FileName> RecentProjects { get; }
	}
}
