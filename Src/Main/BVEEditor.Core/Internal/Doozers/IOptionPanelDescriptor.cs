/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 06/30/2013
 * Time: 23:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace BVEEditor
{
    /// <summary>
    /// Interface for OptionPanelDescriptor.
    /// </summary>
	public interface IOptionPanelDescriptor
	{
		/// <value>
		/// Returns the ID of the dialog panel codon
		/// </value>
		string ID {
			get;
		}
		
		/// <value>
		/// Returns the label of the dialog panel
		/// </value>
		string Label { get; }
		
		/// <summary>
		/// The child dialog panels (e.g. for treeviews)
		/// </summary>
		IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors {
			get;
		}
		
		/// <value>
		/// Returns the dialog panel object
		/// </value>
		IOptionPanel OptionPanel {
			get;
		}
		
		/// <summary>
		/// Gets whether the descriptor has an option panel (as opposed to having only child option panels)
		/// </summary>
		bool HasOptionPanel {
			get;
		}
	}
}
