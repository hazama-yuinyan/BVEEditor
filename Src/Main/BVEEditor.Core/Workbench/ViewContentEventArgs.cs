/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/20
 * Time: 14:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Workbench
{
	public class ViewContentEvent
	{
		ViewContentViewModel content;
		
		public ViewContentViewModel Content {
			get {
				return content;
			}
		}
		
		public ViewContentEvent(ViewContentViewModel content)
		{
			if(content == null)
				throw new ArgumentNullException("content");
			
			this.content = content;
		}
	}
}
