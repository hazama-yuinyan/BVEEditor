// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using Caliburn.Micro;

namespace ICSharpCode.Core
{
    /// <summary>
    /// Represents an in-app representation of Runtime element.
    /// </summary>
	public class Runtime
	{
		string   hint_path;
		string   assembly;
		Assembly loaded_assembly = null;
		
		List<LazyLoadDoozer> defined_doozers = new List<LazyLoadDoozer>();
		List<LazyConditionEvaluator> defined_condition_evaluators = new List<LazyConditionEvaluator>();
		ICondition[] conditions;
		IAddInTree add_in_tree;
		bool is_active = true;
		bool is_assembly_loaded;
		readonly object LockObj = new object(); // used to protect mutable parts of runtime
		
		public bool IsActive {
			get {
				lock(LockObj){
					if(conditions != null){
						is_active = Condition.GetFailedAction(conditions, this) == ConditionFailedAction.Nothing;
						conditions = null;
					}
					return is_active;
				}
			}
		}
		
		public Runtime(IAddInTree addInTree, string assembly, string hintPath)
		{
			if(addInTree == null)
				throw new ArgumentNullException("addInTree");
			if(assembly == null)
				throw new ArgumentNullException("assembly");
			
            this.add_in_tree = addInTree;
			this.assembly = assembly;
			this.hint_path = hintPath;
		}
		
		public string Assembly {
			get { return assembly; }
		}
		
		/// <summary>
		/// Gets whether the assembly belongs to the host application (':' prefix).
		/// </summary>
		public bool IsHostApplicationAssembly {
			get { return !string.IsNullOrEmpty(assembly) && assembly[0] == ':'; }
		}
		
		/// <summary>
		/// Force loading the runtime assembly now.
		/// </summary>
		public void Load()
		{
			lock(LockObj){
				if(!is_assembly_loaded){
					if(!this.IsActive)
						throw new InvalidOperationException("Cannot load inactive AddIn runtime");
					
					is_assembly_loaded = true;
					
					try{
						if(assembly[0] == ':'){
							loaded_assembly = LoadAssembly(assembly.Substring(1));
						}else if(assembly[0] == '$'){
							int pos = assembly.IndexOf('/');
							if(pos < 0)
								throw new CoreException("Expected '/' in path beginning with '$'!");
							
                            string referenced_addin = assembly.Substring(1, pos - 1);
							foreach(var add_in in add_in_tree.AddIns){
								if(add_in.Enabled && add_in.Manifest.Identities.ContainsKey(referenced_addin)){
									string assemblyFile = Path.Combine(Path.GetDirectoryName(add_in.FileName),
									                                   assembly.Substring(pos + 1));
									loaded_assembly = LoadAssemblyFrom(assemblyFile);
									break;
								}
							}
							if(loaded_assembly == null)
								throw new FileNotFoundException("Could not find referenced AddIn " + referenced_addin);
						}else{
							loaded_assembly = LoadAssemblyFrom(Path.Combine(hint_path, assembly));
						}

						#if DEBUG
						// preload assembly to provoke FileLoadException if dependencies are missing
						loaded_assembly.GetExportedTypes();
						#endif
					}catch(FileNotFoundException ex){
						ShowError("The addin '" + assembly + "' could not be loaded:\n" + ex.ToString());
					}catch(FileLoadException ex){
						ShowError("The addin '" + assembly + "' could not be loaded:\n" + ex.ToString());
					}

                    //Register the assembly to Caliburn.Micro in order for it to
                    //retrieve views from the assembly.
                    if(!AssemblySource.Instance.Contains(loaded_assembly))
                        AssemblySource.Instance.Add(loaded_assembly);
				}
			}
		}
		
		public Assembly LoadedAssembly {
			get {
				if(this.IsActive){
					Load(); // load the assembly, in case it is not already done
					return loaded_assembly;
				}else{
					return null;
				}
			}
		}
		
		public IEnumerable<KeyValuePair<string, IDoozer>> DefinedDoozers {
			get {
				return defined_doozers.Select(d => new KeyValuePair<string, IDoozer>(d.Name, d));
			}
		}
		
		public IEnumerable<KeyValuePair<string, IConditionEvaluator>> DefinedConditionEvaluators {
			get {
				return defined_condition_evaluators.Select(c => new KeyValuePair<string, IConditionEvaluator>(c.Name, c));
			}
		}
		
		public Type FindType(string className)
		{
			Assembly asm = LoadedAssembly;
			if(asm == null)
				return null;
			
            return asm.GetType(className);
		}
		
		internal static List<Runtime> ReadSection(XmlReader reader, AddIn addIn, string hintPath)
		{
			List<Runtime> runtimes = new List<Runtime>();
			Stack<ICondition> conditionStack = new Stack<ICondition>();
			while(reader.Read()){
				switch(reader.NodeType){
                case XmlNodeType.EndElement:
                    if(reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition"){
                        conditionStack.Pop();
                    }else if(reader.LocalName == "Runtime"){
                        return runtimes;
                    }
					break;
                
                case XmlNodeType.Element:
                    switch(reader.LocalName){
                    case "Condition":
                        conditionStack.Push(Condition.Read(reader));
                        break;
						
                    case "ComplexCondition":
                        conditionStack.Push(Condition.ReadComplexCondition(reader));
                        break;
						
                    case "Import":
                        runtimes.Add(Runtime.Read(addIn, reader, hintPath, conditionStack));
                        break;
						
                    case "DisableAddIn":
                        if(Condition.GetFailedAction(conditionStack, addIn) == ConditionFailedAction.Nothing){
                            // The DisableAddIn node not was not disabled by a condition
                            addIn.CustomErrorMessage = reader.GetAttribute("message");
                        }
                        break;
                    
                    default:
                        throw new AddInLoadException("Unknown node in runtime section :" + reader.LocalName);
                    }
                    break;
				}
			}
			return runtimes;
		}
		
		internal static Runtime Read(AddIn addIn, XmlReader reader, string hintPath, Stack<ICondition> conditionStack)
		{
			if(reader.AttributeCount != 1)
				throw new AddInLoadException("Import node requires ONE attribute.");
			
			Runtime	runtime = new Runtime(addIn.AddInTree, reader.GetAttribute(0), hintPath);
			if(conditionStack.Count > 0)
				runtime.conditions = conditionStack.ToArray();
			
			if(!reader.IsEmptyElement){
				while(reader.Read()){
					switch(reader.NodeType){
                    case XmlNodeType.EndElement:
                        if(reader.LocalName == "Import")
                            return runtime;

                        break;
						
                    case XmlNodeType.Element:
                        string nodeName = reader.LocalName;
                        Properties properties = Properties.ReadFromAttributes(reader);
                        
                        switch(nodeName){
                        case "Doozer":
                            if(!reader.IsEmptyElement)
                                throw new AddInLoadException("Doozer nodes must be empty!");
						
                            runtime.defined_doozers.Add(new LazyLoadDoozer(addIn, properties));
                            break;
		
                        case "ConditionEvaluator":
                            if(!reader.IsEmptyElement)
                                throw new AddInLoadException("ConditionEvaluator nodes must be empty!");
									
							runtime.defined_condition_evaluators.Add(new LazyConditionEvaluator(addIn, properties));
                            break;
							
                        default:
                            throw new AddInLoadException("Unknown node in Import section:" + nodeName);
                        }
                        break;
					}
				}
			}
			return runtime;
		}
		
		protected virtual Assembly LoadAssembly(string assemblyString)
		{
			return System.Reflection.Assembly.Load(assemblyString);
		}
		
		protected virtual Assembly LoadAssemblyFrom(string assemblyFile)
		{
			return System.Reflection.Assembly.LoadFrom(assemblyFile);
		}
		
		protected virtual void ShowError(string message)
		{
			IoC.Get<IMessageService>().ShowError(message);
		}
	}
}
