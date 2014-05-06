/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 06/30/2013
 * Time: 23:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.AvalonEdit.Snippets;

namespace BVEEditor.Editor
{
	/// <summary>
	/// Used in "/BVEEditor/ViewContent/AvalonEdit/SnippetElementProviders" to allow AddIns to provide custom snippet elements.
	/// </summary>
	public interface ISnippetElementProvider
	{
		SnippetElement GetElement(SnippetInfo snippetInfo);
	}
	
	public class SnippetInfo
	{
		public readonly string Tag;
		public readonly string SnippetText;
		public readonly int Position;
		
		public SnippetInfo(string tag, string snippetText, int position)
		{
			this.Tag = tag;
			this.SnippetText = snippetText;
			this.Position = position;
		}
	}
}
