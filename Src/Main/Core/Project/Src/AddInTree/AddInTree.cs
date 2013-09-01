// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using Caliburn.Micro;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Static class containing the AddInTree. Contains methods for accessing tree nodes and building items.
	/// </summary>
	public static class AddInTree
	{
		public static List<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound = true)
		{
			var addInTree = IoC.Get<IAddInTree>();
			return addInTree.BuildItems<T>(path, parameter, throwOnNotFound).ToList();
		}
		
		public static AddInTreeNode GetTreeNode(string path, bool throwOnNotFound = true)
		{
			var addInTree = IoC.Get<IAddInTree>();
			return addInTree.GetTreeNode(path, throwOnNotFound);
		}
	}
	
	/// <summary>
	/// Class containing the AddInTree. Contains methods for accessing tree nodes and building items.
	/// </summary>
	public class AddInTreeImpl : IAddInTree
	{
		List<AddIn>   add_ins   = new List<AddIn>();
		AddInTreeNode root_node = new AddInTreeNode();
		
		ConcurrentDictionary<string, IDoozer> doozers = new ConcurrentDictionary<string, IDoozer>();
		ConcurrentDictionary<string, IConditionEvaluator> condition_evaluators = new ConcurrentDictionary<string, IConditionEvaluator>();
		
		public AddInTreeImpl(ApplicationStateInfoService applicationStateService)
		{
			doozers.TryAdd("Class", new ClassDoozer());
			doozers.TryAdd("FileFilter", new FileFilterDoozer());
			doozers.TryAdd("String", new StringDoozer());
			doozers.TryAdd("Icon", new IconDoozer());
			doozers.TryAdd("MenuItem", new MenuItemDoozer());
			doozers.TryAdd("ToolbarItem", new ToolbarItemDoozer());
			doozers.TryAdd("Include", new IncludeDoozer());
			doozers.TryAdd("Service", new ServiceDoozer());
			
			condition_evaluators.TryAdd("Compare", new CompareConditionEvaluator());
			condition_evaluators.TryAdd("Ownerstate", new OwnerStateConditionEvaluator());
			
			if(applicationStateService != null)
				applicationStateService.RegisterStateGetter("Installed 3rd party AddIns", GetInstalledThirdPartyAddInsListAsString);
		}
		
		string GetInstalledThirdPartyAddInsListAsString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach(AddIn add_in in AddIns){
				// Skip preinstalled AddIns (show only third party AddIns)
				if(add_in.IsPreinstalled)
					continue;
				
				if(sb.Length > 0) sb.Append(", ");
				sb.Append("[");
				sb.Append(add_in.Name);
				if(add_in.Version != null){
					sb.Append(' ');
					sb.Append(add_in.Version.ToString());
				}
				if(!add_in.Enabled){
					sb.Append(", Enabled=");
					sb.Append(add_in.Enabled);
				}
				if(add_in.Action != AddInAction.Enable){
					sb.Append(", Action=");
					sb.Append(add_in.Action.ToString());
				}
				sb.Append("]");
			}
			return sb.ToString();
		}
		
		/// <summary>
		/// Gets the list of loaded AddIns.
		/// </summary>
		public IReadOnlyList<AddIn> AddIns {
			get {
				return add_ins;
			}
		}
		
		/// <summary>
		/// Gets a dictionary of registered doozers.
		/// </summary>
		public ConcurrentDictionary<string, IDoozer> Doozers {
			get {
				return doozers;
			}
		}
		
		/// <summary>
		/// Gets a dictionary of registered condition evaluators.
		/// </summary>
		public ConcurrentDictionary<string, IConditionEvaluator> ConditionEvaluators {
			get {
				return condition_evaluators;
			}
		}
		
		/// <summary>
		/// Gets the <see cref="AddInTreeNode"/> representing the specified path.
		/// </summary>
		/// <param name="path">The path of the AddIn tree node</param>
		/// <param name="throwOnNotFound">
		/// If set to <c>true</c>, this method throws a
		/// <see cref="TreePathNotFoundException"/> when the path does not exist.
		/// If set to <c>false</c>, <c>null</c> is returned for non-existing paths.
		/// </param>
		public AddInTreeNode GetTreeNode(string path, bool throwOnNotFound = true)
		{
			if(path == null || path.Length == 0)
				return root_node;
			
			string[] split_path = path.Split('/');
			AddInTreeNode cur_path = root_node;
			for(int i = 0; i < split_path.Length; i++){
				if(!cur_path.ChildNodes.TryGetValue(split_path[i], out cur_path)){
					if(throwOnNotFound)
						throw new TreePathNotFoundException(path);
					else
						return null;
				}
			}
			return cur_path;
		}
		
		/// <summary>
		/// Builds a single item in the addin tree.
		/// </summary>
		/// <param name="path">A path to the item in the addin tree.</param>
		/// <param name="parameter">A parameter that gets passed into the doozer and condition evaluators.</param>
		/// <exception cref="TreePathNotFoundException">The path does not
		/// exist or does not point to an item.</exception>
		public object BuildItem(string path, object parameter)
		{
			return BuildItem(path, parameter, null);
		}
		
		public object BuildItem(string path, object parameter, IEnumerable<ICondition> additionalConditions)
		{
			int pos = path.LastIndexOf('/');
			string parent = path.Substring(0, pos);
			string child = path.Substring(pos + 1);
			AddInTreeNode node = GetTreeNode(parent);
			return node.BuildChildItem(child, parameter, additionalConditions);
		}
		
		/// <summary>
		/// Builds the items in the path. Ensures that all items have the type T.
		/// </summary>
		/// <param name="path">A path in the addin tree.</param>
		/// <param name="parameter">The owner used to create the objects.</param>
		/// <param name="throwOnNotFound">If true, throws a <see cref="TreePathNotFoundException"/>
		/// if the path is not found. If false, an empty ArrayList is returned when the
		/// path is not found.</param>
		public IReadOnlyList<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound = true)
		{
			AddInTreeNode node = GetTreeNode(path, throwOnNotFound);
			if(node == null)
				return new List<T>();
			else
				return node.BuildChildItems<T>(parameter);
		}
		
		AddInTreeNode CreatePath(AddInTreeNode localRoot, string path)
		{
			if(path == null || path.Length == 0)
				return localRoot;
			
			string[] split_path = path.Split('/');
			AddInTreeNode cur_path = localRoot;
			int i = 0;
			while(i < split_path.Length){
				if(!cur_path.ChildNodes.ContainsKey(split_path[i]))
					cur_path.ChildNodes[split_path[i]] = new AddInTreeNode();
				
				cur_path = cur_path.ChildNodes[split_path[i]];
				++i;
			}
			
			return cur_path;
		}
		
		void AddExtensionPath(ExtensionPath path)
		{
			AddInTreeNode tree_path = CreatePath(root_node, path.Name);
			foreach(IEnumerable<Codon> inner_codons in path.GroupedCodons)
				tree_path.AddCodons(inner_codons);
		}
		
		/// <summary>
		/// The specified AddIn is added to the <see cref="AddIns"/> collection.
		/// If the AddIn is enabled, its doozers, condition evaluators and extension
		/// paths are added to the AddInTree.
		/// </summary>
		public void InsertAddIn(AddIn addIn)
		{
			if(addIn.Enabled){
				foreach(ExtensionPath path in addIn.Paths.Values)
					AddExtensionPath(path);
				
				foreach(Runtime runtime in addIn.Runtimes){
					if(runtime.IsActive){
						foreach(var pair in runtime.DefinedDoozers){
							if(!doozers.TryAdd(pair.Key, pair.Value))
								throw new AddInLoadException("Duplicate doozer: " + pair.Key);
						}
						foreach(var pair in runtime.DefinedConditionEvaluators){
							if(!condition_evaluators.TryAdd(pair.Key, pair.Value))
								throw new AddInLoadException("Duplicate condition evaluator: " + pair.Key);
						}
					}
				}
				
				string add_in_root = Path.GetDirectoryName(addIn.FileName);
				/*foreach(string bitmapResource in addIn.BitmapResources){
					string path = Path.Combine(add_in_root, bitmapResource);
					ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
					IoC.Get<IResourceService>().RegisterNeutralImages(resourceManager);
				}
				
				foreach(string stringResource in addIn.StringResources){
					string path = Path.Combine(add_in_root, stringResource);
					ResourceManager resourceManager = ResourceManager.CreateFileBasedResourceManager(Path.GetFileNameWithoutExtension(path), Path.GetDirectoryName(path), null);
					IoC.Get<IResourceService>().RegisterNeutralStrings(resourceManager);
				}*/
			}
			add_ins.Add(addIn);
		}
		
		/// <summary>
		/// The specified AddIn is removed to the <see cref="AddIns"/> collection.
		/// This is only possible for disabled AddIns, enabled AddIns require
		/// the application to restart before it gets removed.
		/// </summary>
		/// <exception cref="ArgumentException">Occurs when trying to remove an enabled AddIn.</exception>
		public void RemoveAddIn(AddIn addIn)
		{
			if(addIn.Enabled)
				throw new ArgumentException("Cannot remove enabled AddIns at runtime.");

			add_ins.Remove(addIn);
		}
		
		// used by Load(): disables an addin and removes it from the dictionaries.
		void DisableAddin(AddIn addIn, Dictionary<string, Version> dict, Dictionary<string, AddIn> addInDict)
		{
			addIn.Enabled = false;
			addIn.Action = AddInAction.DependencyError;
			foreach (string name in addIn.Manifest.Identities.Keys) {
				dict.Remove(name);
				addInDict.Remove(name);
			}
		}
		
		/// <summary>
		/// Loads a list of .addin files, ensuring that dependencies are satisfied.
		/// This method is normally called by <see cref="CoreStartup.RunInitialization"/>.
		/// </summary>
		/// <param name="addInFiles">
		/// The list of .addin file names to load.
		/// </param>
		/// <param name="disabledAddIns">
		/// The list of disabled AddIn identity names.
		/// </param>
		public void Load(List<string> addInFiles, List<string> disabledAddIns)
		{
			List<AddIn> list = new List<AddIn>();
			Dictionary<string, Version> dict = new Dictionary<string, Version>();
			Dictionary<string, AddIn> addInDict = new Dictionary<string, AddIn>();
			var name_table = new System.Xml.NameTable();
			foreach(string file_name in addInFiles){
				AddIn add_in;
				try{
					add_in = AddIn.Load(this, file_name, name_table);
				}catch(AddInLoadException ex){
					LogManager.GetLog(typeof(AddInTree)).Error(ex);
					if(ex.InnerException != null){
                        IoC.Get<IMessageService>().ShowError("Error loading AddIn " + file_name + ":\n"
						                         + ex.InnerException.Message);
					}else{
                        IoC.Get<IMessageService>().ShowError("Error loading AddIn " + file_name + ":\n"
						                         + ex.Message);
					}
					add_in = new AddIn(this);
					add_in.addInFileName = file_name;
					add_in.CustomErrorMessage = ex.Message;
				}
				if(add_in.Action == AddInAction.CustomError){
					list.Add(add_in);
					continue;
				}
				add_in.Enabled = true;
				if(disabledAddIns != null && disabledAddIns.Count > 0){
					foreach(string name in add_in.Manifest.Identities.Keys){
						if(disabledAddIns.Contains(name)){
							add_in.Enabled = false;
							break;
						}
					}
				}

				if(add_in.Enabled){
					foreach(KeyValuePair<string, Version> pair in add_in.Manifest.Identities){
						if(dict.ContainsKey(pair.Key)){
							IoC.Get<IMessageService>().ShowError("Name '" + pair.Key + "' is used by " +
							                         "'" + addInDict[pair.Key].FileName + "' and '" + file_name + "'");
							add_in.Enabled = false;
							add_in.Action = AddInAction.InstalledTwice;
							break;
						}else{
							dict.Add(pair.Key, pair.Value);
							addInDict.Add(pair.Key, add_in);
						}
					}
				}
				list.Add(add_in);
			}
		checkDependencies:
			for(int i = 0; i < list.Count; i++){
				AddIn addIn = list[i];
				if(!addIn.Enabled) continue;
				
				Version versionFound;
				
				foreach(AddInReference reference in addIn.Manifest.Conflicts){
					if(reference.Check(dict, out versionFound)){
						IoC.Get<IMessageService>().ShowError(addIn.Name + " conflicts with " + reference.ToString()
						                         + " and has been disabled.");
						DisableAddin(addIn, dict, addInDict);
						goto checkDependencies; // after removing one addin, others could break
					}
				}

				foreach(AddInReference reference in addIn.Manifest.Dependencies){
					if(!reference.Check(dict, out versionFound)){
						if(versionFound != null){
							IoC.Get<IMessageService>().ShowError(addIn.Name + " has not been loaded because it requires "
							                         + reference.ToString() + ", but version "
							                         + versionFound.ToString() + " is installed.");
						}else{
							IoC.Get<IMessageService>().ShowError(addIn.Name + " has not been loaded because it requires "
							                         + reference.ToString() + ".");
						}
						DisableAddin(addIn, dict, addInDict);
						goto checkDependencies; // after removing one addin, others could break
					}
				}
			}

			foreach(AddIn addIn in list){
				try{
					InsertAddIn(addIn);
				}catch(AddInLoadException ex){
					LogManager.GetLog(typeof(AddInTree)).Error(ex);
                    IoC.Get<IMessageService>().ShowError("Error loading AddIn " + addIn.FileName + ":\n"
					                         + ex.Message);
				}
			}
		}
	}
}
