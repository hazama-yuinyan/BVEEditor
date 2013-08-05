/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/16
 * Time: 12:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Core.Presentation
{
	/// <summary>
	/// Description of IStatusUpdatable.
	/// </summary>
	public interface IStatusUpdatable
	{
		void UpdateText();
		void UpdateStatus();
	}
}
