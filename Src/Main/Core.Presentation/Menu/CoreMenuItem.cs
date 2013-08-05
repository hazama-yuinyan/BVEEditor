/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/16
 * Time: 19:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;

namespace Core.Presentation
{
	/// <summary>
	/// A menu item representing an AddIn-Tree element.
	/// </summary>
	class CoreMenuItem : MenuItem, IStatusUpdatable
	{
		protected readonly Codon codon;
		protected readonly object caller;
		protected readonly IReadOnlyCollection<ICondition> conditions;
		
		/// <summary>
		/// If true, UpdateStatus() sets the enabled flag.
		/// Used for type=Menu, but not for type=MenuItem - for menu items, Enabled is controlled by the WPF ICommand.
		/// </summary>
		internal bool SetEnabled;
		
		public CoreMenuItem(Codon codon, object caller, IReadOnlyCollection<ICondition> conditions)
		{
			this.codon = codon;
			this.caller = caller;
			this.conditions = conditions;
			
			if (codon.Properties.Contains("icon")) {
				try {
					var image = new Image();
					image.Source = PresentationResourceService.GetBitmapSource(codon.Properties["icon"]);
					image.Height = 16;
					this.Icon = image;
				} catch (ResourceNotFoundException) {}
			}
			UpdateText();
		}

		public void UpdateText()
		{
			if (codon != null) {
				Header = MenuService.ConvertLabel(StringParser.Parse(codon.Properties["label"]));
			}
		}
		
		public virtual void UpdateStatus()
		{
			ConditionFailedAction result = ICSharpCode.Core.Condition.GetFailedAction(conditions, caller);
			if (result == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
			if (SetEnabled)
				this.IsEnabled = result == ConditionFailedAction.Nothing;
		}
	}
}
