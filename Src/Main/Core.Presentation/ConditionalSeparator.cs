﻿/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/16
 * Time: 19:41
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
	/// A Separator that is invisible when it is excluded by a condition.
	/// </summary>
	sealed class ConditionalSeparator : Separator, IStatusUpdatable
	{
		readonly Codon codon;
		readonly object caller;
		readonly IEnumerable<ICondition> conditions;
		
		public ConditionalSeparator(Codon codon, object caller, bool inToolbar, IEnumerable<ICondition> conditions)
		{
			this.codon = codon;
			this.caller = caller;
			this.conditions = conditions;
			
			if(inToolbar)
				SetResourceReference(FrameworkElement.StyleProperty, ToolBar.SeparatorStyleKey);
		}
		
		public void UpdateText()
		{
		}
		
		public void UpdateStatus()
		{
			if(ICSharpCode.Core.Condition.GetFailedAction(conditions, caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
	}
}
