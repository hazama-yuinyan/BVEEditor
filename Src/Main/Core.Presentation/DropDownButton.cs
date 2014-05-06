/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/26
 * Time: 14:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Core.Presentation
{
	/// <summary>
	/// A button that opens a drop-down menu when clicked.
	/// </summary>
	public class DropDownButton : ButtonBase
	{
		public static readonly DependencyProperty DropDownMenuProperty
			= DependencyProperty.Register("DropDownMenu", typeof(ContextMenu),
			                              typeof(DropDownButton), new FrameworkPropertyMetadata(null));
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		protected static readonly DependencyPropertyKey IsDropDownMenuOpenPropertyKey
			= DependencyProperty.RegisterReadOnly("IsDropDownMenuOpen", typeof(bool),
			                                      typeof(DropDownButton), new FrameworkPropertyMetadata(false));
		
		public static readonly DependencyProperty IsDropDownMenuOpenProperty = IsDropDownMenuOpenPropertyKey.DependencyProperty;
		
		static DropDownButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
		}
		
		public ContextMenu DropDownMenu {
			get { return (ContextMenu)GetValue(DropDownMenuProperty); }
			set { SetValue(DropDownMenuProperty, value); }
		}
		
		public bool IsDropDownMenuOpen {
			get { return (bool)GetValue(IsDropDownMenuOpenProperty); }
			protected set { SetValue(IsDropDownMenuOpenPropertyKey, value); }
		}
		
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (DropDownMenu != null && !IsDropDownMenuOpen) {
				DropDownMenu.Placement = PlacementMode.Bottom;
				DropDownMenu.PlacementTarget = this;
				DropDownMenu.IsOpen = true;
				DropDownMenu.Closed += DropDownMenu_Closed;
				this.IsDropDownMenuOpen = true;
			}
		}
		
		void DropDownMenu_Closed(object sender, RoutedEventArgs e)
		{
			((ContextMenu)sender).Closed -= DropDownMenu_Closed;
			this.IsDropDownMenuOpen = false;
		}
		
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (!IsMouseCaptured) {
				e.Handled = true;
			}
		}
	}
}
