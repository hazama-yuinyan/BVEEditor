/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/27
 * Time: 12:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// If an IViewContent object is from the type IPositionable, it signals
	/// that it's a texteditor which could set the caret to a position inside
	/// a file.
	/// </summary>
	public interface IPositionable
	{
		/// <summary>
		/// Sets the 'caret' to the position pos, where pos.Y is the line (starting from 1)
		/// and pos.X is the column (starting from 1 too).
		/// </summary>
		void JumpTo(int line, int column);
		
		/// <summary>
		/// gets the 'caret' position line (starting from 1)
		/// </summary>
		int Line {
			get;
		}

		/// <summary>
		/// gets the 'caret' position column (starting from 1)
		/// </summary>
		int Column {
			get;
		}
	}
}
