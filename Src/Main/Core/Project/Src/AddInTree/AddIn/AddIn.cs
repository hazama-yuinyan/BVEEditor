﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Caliburn.Micro;

namespace ICSharpCode.Core
{
	public sealed class AddIn
	{
		IAddInTree addInTree;
		Properties    properties = new Properties();
		List<Runtime> runtimes   = new List<Runtime>();
		List<string> bitmapResources = new List<string>();
		List<string> stringResources = new List<string>();
		
		internal string addInFileName = null;
		AddInManifest manifest = new AddInManifest();
		Dictionary<string, ExtensionPath> paths = new Dictionary<string, ExtensionPath>();
		AddInAction action = AddInAction.Disable;
		bool enabled;
		
		static bool hasShownErrorMessage = false;
		
		internal AddIn(IAddInTree addInTree)
		{
			if(addInTree == null)
				throw new ArgumentNullException("addInTree");
			
            this.addInTree = addInTree;
		}
		
		/// <summary>
		/// Gets the parent AddIn-Tree that contains this AddIn.
		/// </summary>
		public IAddInTree AddInTree {
			get { return addInTree; }
		}
		
		public object CreateObject(string className)
		{
			Type t = FindType(className);
			if(t != null)
				return IoC.GetInstance(t, null);
			else
				return null;
		}
		
		public Type FindType(string className)
		{
			foreach(Runtime runtime in runtimes){
				if(!runtime.IsHostApplicationAssembly){
					// Load dependencies only when a plugin library is first loaded.
					// This allows looking in host assemblies for service IDs.
					LoadDependencies();
				}
				Type t = runtime.FindType(className);
				if(t != null)
					return t;
			}
			if(hasShownErrorMessage){
				DebugLogger.Instance.Error("Cannot find class: " + className);
			}else{
				hasShownErrorMessage = true;
				IoC.Get<IMessageService>()
                    .ShowError("Cannot find class: " + className + "\nFuture missing objects will not cause an error message.");
			}
			return null;
		}
		
		public Stream GetManifestResourceStream(string resourceName)
		{
			LoadDependencies();
			foreach(Runtime runtime in runtimes){
				Assembly assembly = runtime.LoadedAssembly;
				if(assembly != null){
					Stream s = assembly.GetManifestResourceStream(resourceName);
					if(s != null)
						return s;
				}
			}
			return null;
		}
		
		public void LoadRuntimeAssemblies()
		{
			LoadDependencies();
			foreach(Runtime runtime in runtimes){
				if(runtime.IsActive)
					runtime.Load();
			}
		}
		
		volatile bool dependencies_loaded;
		
		void LoadDependencies()
		{
			// Thread-safe dependency loading:
			// Because the methods being called should be thread-safe, there's
			// no problem when we load dependencies multiple times concurrently.
			// However, we need to make sure we don't return before the dependencies are ready,
			// so "bool dependenciesLoaded" must be volatile and set only at the very end of this method.
			if(!dependencies_loaded){
				DebugLogger.Instance.Info("Loading addin " + this.Name);
				
				AssemblyLocator.Init();
				foreach(AddInReference r in manifest.Dependencies){
					if(r.RequirePreload){
						bool found = false;
						foreach(AddIn add_in in AddInTree.AddIns){
							if(add_in.Manifest.Identities.ContainsKey(r.Name)){
								found = true;
								add_in.LoadRuntimeAssemblies();
							}
						}

						if(!found)
							throw new AddInLoadException("Cannot load run-time dependency for " + r.ToString());
					}
				}
				dependencies_loaded = true;
			}
		}
		
		public override string ToString()
		{
			return "[AddIn: " + Name + "]";
		}
		
		string custom_error_message;
		
		/// <summary>
		/// Gets the message of a custom load error. Used only when AddInAction is set to CustomError.
		/// Setting this property to a non-null value causes Enabled to be set to false and
		/// Action to be set to AddInAction.CustomError.
		/// </summary>
		public string CustomErrorMessage {
			get { return custom_error_message; }
			internal set {
				if(value != null){
					Enabled = false;
					Action = AddInAction.CustomError;
				}
				custom_error_message = value;
			}
		}
		
		/// <summary>
		/// Action to execute when the application is restarted.
		/// </summary>
		public AddInAction Action {
			get { return action; }
			set { action = value; }
		}
		
		public IReadOnlyList<Runtime> Runtimes {
			get { return runtimes; }
		}
		
		public Version Version {
			get { return manifest.PrimaryVersion; }
		}
		
		public string FileName {
			get { return addInFileName; }
			set { addInFileName = value; }
		}
		
		public string Name {
			get { return properties["name"]; }
		}
		
		public AddInManifest Manifest {
			get { return manifest; }
		}
		
		public Dictionary<string, ExtensionPath> Paths {
			get { return paths; }
		}
		
		public Properties Properties {
			get { return properties; }
		}
		
		public List<string> BitmapResources {
			get { return bitmapResources; }
			set { bitmapResources = value; }
		}
		
		public List<string> StringResources {
			get { return stringResources; }
			set { stringResources = value; }
		}
		
		public bool Enabled {
			get { return enabled; }
			set {
				enabled = value;
				this.Action = value ? AddInAction.Enable : AddInAction.Disable;
			}
		}
		
		static void SetupAddIn(XmlReader reader, AddIn addIn, string hintPath)
		{
			while(reader.Read()){
				if(reader.NodeType == XmlNodeType.Element && reader.IsStartElement()){
					switch(reader.LocalName){
                    case "StringResources":
                    case "BitmapResources":
                        if(reader.AttributeCount != 1)
							throw new AddInLoadException("BitmapResources requires ONE attribute.");
							
						string filename = StringParser.Parse(reader.GetAttribute("file"));
							
						if(reader.LocalName == "BitmapResources")
							addIn.BitmapResources.Add(filename);
						else
							addIn.StringResources.Add(filename);
					
                        break;
					case "Runtime":
						if(!reader.IsEmptyElement)
							addIn.runtimes.AddRange(Runtime.ReadSection(reader, addIn, hintPath));
						
					    break;
					case "Include":
						if(reader.AttributeCount != 1)
							throw new AddInLoadException("Include requires ONE attribute.");
						if(!reader.IsEmptyElement)
							throw new AddInLoadException("Include nodes must be empty!");
						if(hintPath == null)
							throw new AddInLoadException("Cannot use include nodes when hintPath was not specified (e.g. when AddInManager reads a .addin file)!");
							
                        string fileName = Path.Combine(hintPath, reader.GetAttribute(0));
						XmlReaderSettings xrs = new XmlReaderSettings();
						xrs.NameTable = reader.NameTable; // share the name table
						xrs.ConformanceLevel = ConformanceLevel.Fragment;
						using(XmlReader includeReader = XmlTextReader.Create(fileName, xrs)){
							SetupAddIn(includeReader, addIn, Path.GetDirectoryName(fileName));
						}
						break;
					case "Path":
						if(reader.AttributeCount != 1)
							throw new AddInLoadException("Import node requires ONE attribute.");
						
                        string pathName = reader.GetAttribute(0);
						ExtensionPath extensionPath = addIn.GetExtensionPath(pathName);
						if(!reader.IsEmptyElement)
							ExtensionPath.SetUp(extensionPath, reader, "Path");
					    
                        break;
				    case "Manifest":
						addIn.Manifest.ReadManifestSection(reader, hintPath);
						break;
					default:
						throw new AddInLoadException("Unknown root path node:" + reader.LocalName);
					}
				}
			}
		}
		
		public ExtensionPath GetExtensionPath(string pathName)
		{
			if(!paths.ContainsKey(pathName))
				return paths[pathName] = new ExtensionPath(pathName, this);
			
			return paths[pathName];
		}
		
		public static AddIn Load(IAddInTree addInTree, TextReader textReader, string hintPath = null, XmlNameTable nameTable = null)
		{
			if(nameTable == null)
				nameTable = new NameTable();
			
            try{
				AddIn addIn = new AddIn(addInTree);
				using(XmlTextReader reader = new XmlTextReader(textReader, nameTable)){
					while(reader.Read()){
						if(reader.IsStartElement()){
							switch(reader.LocalName){
								case "AddIn":
									addIn.properties = Properties.ReadFromAttributes(reader);
									SetupAddIn(reader, addIn, hintPath);
									break;
								default:
									throw new AddInLoadException("Unknown add-in file.");
							}
						}
					}
				}
				return addIn;
			}catch(XmlException ex){
				throw new AddInLoadException(ex.Message, ex);
			}
		}
		
		public static AddIn Load(IAddInTree addInTree, string fileName, XmlNameTable nameTable = null)
		{
			try{
				using(TextReader textReader = File.OpenText(fileName)){
					AddIn addIn = Load(addInTree, textReader, Path.GetDirectoryName(fileName), nameTable);
					addIn.addInFileName = fileName;
					return addIn;
				}
			}catch(AddInLoadException){
				throw;
			}catch(Exception e){
				throw new AddInLoadException("Can't load " + fileName, e);
			}
		}
		
		/// <summary>
		/// Gets whether the AddIn is a preinstalled component of the host application.
		/// </summary>
		public bool IsPreinstalled {
			get {
				if(FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, this.FileName)){
					string hidden = this.Properties["addInManagerHidden"];
					return string.Equals(hidden, "true", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(hidden, "preinstalled", StringComparison.OrdinalIgnoreCase);
				}else{
					return false;
				}
			}
		}
	}
}
