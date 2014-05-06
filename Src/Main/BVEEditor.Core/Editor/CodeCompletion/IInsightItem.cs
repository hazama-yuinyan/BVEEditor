/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/19
 * Time: 15:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Editor.CodeCompletion
{
	/// <summary>
	/// An item in the insight window.
	/// </summary>
	public interface IInsightItem
	{
		object Header { get; }
		object Content { get; }
	}
}
