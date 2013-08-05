/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/27
 * Time: 16:21
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Services
{
	public class UILanguage
	{
		string name;
		string code;
		string imagePath;
		bool isRightToLeft;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Code {
			get {
				return code;
			}
		}
		
		public string ImagePath {
			get {
				return imagePath;
			}
		}
		
		public bool IsRightToLeft {
			get { return isRightToLeft; }
		}
		
		public UILanguage(string name, string code, string imagePath, bool isRightToLeft)
		{
			this.name       = name;
			this.code       = code;
			this.imagePath  = imagePath;
			this.isRightToLeft = isRightToLeft;
		}
	}
}
