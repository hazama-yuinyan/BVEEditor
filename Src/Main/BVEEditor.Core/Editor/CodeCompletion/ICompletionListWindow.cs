/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/19
 * Time: 15:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Editor.CodeCompletion
{
	/// <summary>
	/// Represents the completion window showing a ICompletionItemList.
	/// </summary>
	public interface ICompletionListWindow : ICompletionWindow
	{
		/// <summary>
		/// Gets/Sets the currently selected item.
		/// </summary>
		ICompletionItem SelectedItem { get; set; }
	}
}
