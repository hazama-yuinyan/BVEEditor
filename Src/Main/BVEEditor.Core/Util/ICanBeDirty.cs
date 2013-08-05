/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/17
 * Time: 12:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor
{
	/// <summary>
	/// Interface for classes that implement the IsDirty property and the DirtyChanged event.
	/// </summary>
	public interface ICanBeDirty
	{
		/// <summary>
		/// If this property returns true the content has changed since
		/// the last load/save operation.
		/// </summary>
		bool IsDirty {
			get;
		}
		
		/// <summary>
		/// Is called when the content is changed after a save/load operation
		/// and this signals that changes could be saved.
		/// </summary>
		event EventHandler IsDirtyChanged;
	}
}
