/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/30
 * Time: 16:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Core.Presentation
{
	/// <summary>
	/// Provides access to objects containing OptionBindings, such as OptionPanels.
	/// </summary>
	public interface IOptionBindingContainer
	{
		void AddBinding(OptionBinding binding);
	}
}
