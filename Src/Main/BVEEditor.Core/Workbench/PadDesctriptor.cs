/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/20
 * Time: 14:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using ICSharpCode.Core;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// Indicates the default position for a pad.
	/// This is a bit-flag enum, Hidden can be combined with the directions.
	/// </summary>
	[Flags]
	public enum DefaultPadPositions
	{
		None = 0,
		Right = 1,
		Left = 2,
		Bottom = 4,
		Top = 8,
		Hidden = 16
	}
	
	/// <summary>
	/// Describes a pad.
	/// </summary>
	public class PadDescriptor
	{
		string viewmodel_name;
		string title;
		string icon;
		string category;
		string shortcut;
		string template_name;
		
		AddIn add_in;
		Type pad_type;
		
		/// <summary>
		/// Creates a new pad descriptor from the AddIn tree.
		/// </summary>
		public PadDescriptor(Codon codon)
		{
			if(codon == null)
				throw new ArgumentNullException("codon");
			
			add_in = codon.AddIn;
			shortcut = codon.Properties["shortcut"];
			category = codon.Properties["category"];
			icon = codon.Properties["icon"];
			title = codon.Properties["title"];
			viewmodel_name = codon.Properties["viewModel"];
			template_name = codon.Properties["templateName"];
			if(!string.IsNullOrEmpty(codon.Properties["defaultPosition"]))
				DefaultPosition = (DefaultPadPositions)Enum.Parse(typeof(DefaultPadPositions), codon.Properties["defaultPosition"]);
		}
		
		/// <summary>
		/// Creates a pad descriptor for the specified pad type.
		/// </summary>
		public PadDescriptor(Type padType, string title, string icon)
		{
			if(padType == null)
				throw new ArgumentNullException("padType");
			
			if(title == null)
				throw new ArgumentNullException("title");
			
			if(icon == null)
				throw new ArgumentNullException("icon");
			
			this.pad_type = padType;
			this.viewmodel_name = padType.FullName;
			this.title = title;
			this.icon = icon;
			this.category = "none";
			this.shortcut = "";
		}
		
		/// <summary>
		/// Returns the title of the pad.
		/// </summary>
		public string Title {
			get {
				return title;
			}
		}
		
		/// <summary>
		/// Returns the icon bitmap resource name of the pad. May be an empty string
		/// if the pad has no icon defined.
		/// </summary>
		public string Icon {
			get {
				return icon;
			}
		}
		
		/// <summary>
		/// Returns the category (this is used for defining where the menu item to
		/// this pad goes)
		/// </summary>
		public string Category {
			get {
				return category;
			}
			set {
				if(value == null)
					throw new ArgumentNullException("value");
				
				category = value;
			}
		}
		
		/// <summary>
		/// Returns the menu shortcut for the view menu item.
		/// </summary>
		public string Shortcut {
			get {
				return shortcut;
			}
			set {
				if(value == null)
					throw new ArgumentNullException("value");
				
				shortcut = value;
			}
		}
		
		/// <summary>
		/// Gets the name of the pad view model.
		/// </summary>
		public string ViewModelName {
			get {
				return viewmodel_name;
			}
		}
		
		/// <summary>
		/// Gets/sets the default position of the pad.
		/// </summary>
		public DefaultPadPositions DefaultPosition { get; set; }
		
		public PadViewModel CreateViewModel()
		{
			return (PadViewModel)add_in.CreateObject(viewmodel_name);
		}
		
		public DataTemplate GetDataTemplate()
		{
			return (DataTemplate)add_in.CreateObject(template_name);
		}
		
		public override string ToString()
		{
			return "[PadDescriptor " + this.ViewModelName + "]";
		}
	}
}
