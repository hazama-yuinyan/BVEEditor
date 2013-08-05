﻿/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/16
 * Time: 11:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.Core;

namespace Core.Presentation
{
	/// <summary>
	/// Creates WPF menu controls from the AddIn Tree.
	/// </summary>
	public static class MenuService
	{
		internal sealed class MenuCreateContext
		{
			public UIElement InputBindingOwner;
			public string ActivationMethod;
			public bool ImmediatelyExpandMenuBuildersForShortcuts;
		}
		
		static Dictionary<string, System.Windows.Input.ICommand> knownCommands = LoadDefaultKnownCommands();
		
		static Dictionary<string, System.Windows.Input.ICommand> LoadDefaultKnownCommands()
		{
			var known_commands = new Dictionary<string, System.Windows.Input.ICommand>();
			foreach(Type t in new Type[] { typeof(ApplicationCommands), typeof(NavigationCommands) }){
				foreach(PropertyInfo p in t.GetProperties())
					known_commands.Add(p.Name, (System.Windows.Input.ICommand)p.GetValue(null, null));
			}
			return known_commands;
		}
		
		/// <summary>
		/// Gets a known WPF command.
		/// </summary>
		/// <param name="commandName">The name of the command, e.g. "Copy".</param>
		/// <returns>The WPF ICommand with the given name, or null if the command was not found.</returns>
		public static System.Windows.Input.ICommand GetKnownCommand(string commandName)
		{
			if (commandName == null)
				throw new ArgumentNullException("commandName");
			System.Windows.Input.ICommand command;
			lock (knownCommands) {
				if (knownCommands.TryGetValue(commandName, out command))
					return command;
			}
			return null;
		}
		
		/// <summary>
		/// Registers a WPF command for use with the &lt;MenuItem command="name"&gt; syntax.
		/// </summary>
		public static void RegisterKnownCommand(string name, System.Windows.Input.ICommand command)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			if(command == null)
				throw new ArgumentNullException("command");
			
			lock (knownCommands) {
				knownCommands.Add(name, command);
			}
		}
		
		public static void UpdateStatus(IEnumerable menuItems)
		{
			if(menuItems == null)
				return;
			
			foreach(object o in menuItems){
				var cmi = o as IStatusUpdatable;
				if(cmi != null)
					cmi.UpdateStatus();
			}
		}
		
		public static void UpdateText(IEnumerable menuItems)
		{
			if(menuItems == null)
				return;
			
			foreach(object o in menuItems){
				var cmi = o as IStatusUpdatable;
				if(cmi != null)
					cmi.UpdateText();
			}
		}
		
		public static ContextMenu CreateContextMenu(object owner, string addInTreePath)
		{
			IList items = CreateUnexpandedMenuItems(
				new MenuCreateContext { ActivationMethod = "ContextMenu" },
				AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
			return CreateContextMenu(items);
		}
		
		public static ContextMenu ShowContextMenu(UIElement parent, object owner, string addInTreePath)
		{
			ContextMenu menu = new ContextMenu();
			menu.ItemsSource = CreateMenuItems(menu, owner, addInTreePath, "ContextMenu");
			menu.PlacementTarget = parent;
			menu.IsOpen = true;
			return menu;
		}
		
		internal static ContextMenu CreateContextMenu(IList subItems)
		{
			var contextMenu = new ContextMenu() {
				ItemsSource = new object[1]
			};
			contextMenu.Opened += (sender, args) => {
				contextMenu.ItemsSource = ExpandMenuBuilders(subItems, true);
				args.Handled = true;
			};
			return contextMenu;
		}
		
		public static IList CreateMenuItems(UIElement inputBindingOwner, object owner, string addInTreePath, string activationMethod = null, bool immediatelyExpandMenuBuildersForShortcuts = false)
		{
			IList items = CreateUnexpandedMenuItems(
				new MenuCreateContext {
					InputBindingOwner = inputBindingOwner,
					ActivationMethod = activationMethod,
					ImmediatelyExpandMenuBuildersForShortcuts =immediatelyExpandMenuBuildersForShortcuts
				},
				AddInTree.BuildItems<MenuItemDescriptor>(addInTreePath, owner, false));
			return ExpandMenuBuilders(items, false);
		}
		
		sealed class MenuItemBuilderPlaceholder
		{
			readonly IMenuItemBuilder builder;
			readonly Codon codon;
			readonly object caller;
			
			public MenuItemBuilderPlaceholder(IMenuItemBuilder builder, Codon codon, object caller)
			{
				this.builder = builder;
				this.codon = codon;
				this.caller = caller;
			}
			
			public IEnumerable<object> BuildItems()
			{
				return builder.BuildItems(codon, caller);
			}
		}
		
		internal static IList CreateUnexpandedMenuItems(MenuCreateContext context, IEnumerable descriptors)
		{
			ArrayList result = new ArrayList();
			if(descriptors != null){
				foreach(MenuItemDescriptor descriptor in descriptors)
					result.Add(CreateMenuItemFromDescriptor(context, descriptor));
			}
			return result;
		}
		
		static IList ExpandMenuBuilders(ICollection input, bool addDummyEntryIfMenuEmpty)
		{
			List<object> result = new List<object>(input.Count);
			foreach(object o in input){
				MenuItemBuilderPlaceholder p = o as MenuItemBuilderPlaceholder;
				if(p != null){
					IEnumerable<object> c = p.BuildItems();
					if(c != null)
						result.AddRange(c);
				}else{
					result.Add(o);
					var statusUpdate = o as IStatusUpdatable;
					if(statusUpdate != null){
						statusUpdate.UpdateStatus();
						statusUpdate.UpdateText();
					}
				}
			}
			if(addDummyEntryIfMenuEmpty && result.Count == 0)
				result.Add(new MenuItem { Header = "(empty menu)", IsEnabled = false });
			
			return result;
		}
		
		static object CreateMenuItemFromDescriptor(MenuCreateContext context, MenuItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Command";
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) {
				case "Separator":
					return new ConditionalSeparator(codon, descriptor.Parameter, false, descriptor.Conditions);
				case "CheckBox":
					return new MenuCheckBox(context.InputBindingOwner, codon, descriptor.Parameter, descriptor.Conditions);
				case "Item":
				case "Command":
					return new MenuCommand(context.InputBindingOwner, codon, descriptor.Parameter, createCommand, context.ActivationMethod, descriptor.Conditions);
				case "Menu":
					var item = new CoreMenuItem(codon, descriptor.Parameter, descriptor.Conditions) {
						ItemsSource = new object[1],
						SetEnabled = true
					};
					var subItems = CreateUnexpandedMenuItems(context, descriptor.SubItems);
					item.SubmenuOpened += (sender, args) => {
						item.ItemsSource = ExpandMenuBuilders(subItems, true);
						args.Handled = true;
					};
					if (context.ImmediatelyExpandMenuBuildersForShortcuts)
						ExpandMenuBuilders(subItems, false);
					return item;
				case "Builder":
					IMenuItemBuilder builder = codon.AddIn.CreateObject(codon.Properties["class"]) as IMenuItemBuilder;
					if(builder == null)
						throw new NotSupportedException("Menu item builder " + codon.Properties["class"] + " does not implement IMenuItemBuilder");
					
					return new MenuItemBuilderPlaceholder(builder, descriptor.Codon, descriptor.Parameter);
				default:
					throw new NotSupportedException("unsupported menu item type : " + type);
			}
		}
		
		/// <summary>
		/// Converts from the Windows-Forms style label format (accessor key marked with '&amp;')
		/// to a WPF label format (accessor key marked with '_').
		/// </summary>
		public static string ConvertLabel(string label)
		{
			return label.Replace("_", "__").Replace("&", "_");
		}
		
		/// <summary>
		/// Creates an KeyGesture for a shortcut.
		/// </summary>
		public static KeyGesture ParseShortcut(string text)
		{
			return (KeyGesture)new KeyGestureConverter().ConvertFromInvariantString(text.Replace('|', '+'));
		}
	}
}
