/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 06/30/2013
 * Time: 23:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace BVEEditor.Options
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

        /// <summary>
        /// Creates a new instance of the view model specified in the node.
        /// </summary>
        /// <returns></returns>
        OptionPanelViewModel CreateViewModel();
	}
}
