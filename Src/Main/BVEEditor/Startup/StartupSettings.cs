/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/15
 * Time: 18:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace BVEEditor.Startup
{
	/// <summary>
	/// This class contains properties you can use to control how SharpDevelop is launched.
	/// </summary>
	[Serializable]
	public sealed class StartupSettings
	{
		string application_name = "BVEEditor";
		bool allow_addIn_configuration_and_external_addIns = true;
		internal List<string> addInDirectories = new List<string>();
		internal List<string> addInFiles = new List<string>();
		
		/// <summary>
		/// Use the file <see cref="ConfigDirectory"/>\AddIns.xml to maintain
		/// a list of deactivated AddIns and list of AddIns to load from
		/// external locations.
		/// The default value is true.
		/// </summary>
		public bool AllowAddInConfigurationAndExternalAddIns {
			get { return allow_addIn_configuration_and_external_addIns; }
			set { allow_addIn_configuration_and_external_addIns = value; }
		}
		
		/// <summary>
		/// Allow user AddIns stored in the "application data" directory.
		/// The default is false.
		/// </summary>
		public bool AllowUserAddIns {get; set;}
		
		/// <summary>
		/// Gets/Sets the application name used by the MessageService and some
		/// BVEEditor windows. The default is "BVEEditor".
		/// </summary>
		public string ApplicationName {
			get { return application_name; }
			set {
				if(value == null)
					throw new ArgumentNullException("value");
				
				application_name = value;
			}
		}
		
		/// <summary>
		/// Gets/Sets the application root path to use.
		/// Use null (default) to use the base directory of the BVEEditor AppDomain.
		/// </summary>
		public string ApplicationRootPath {get; set;}
		
		/// <summary>
		/// Gets/Sets the directory used to store BVEEditor properties,
		/// settings and user AddIns.
		/// Use null (default) to use "ApplicationData\ApplicationName"
		/// </summary>
		public string ConfigDirectory {get; set;}
		
		/// <summary>
		/// Sets the data directory used to load resources.
		/// Use null (default) to use the default path "ApplicationRootPath\data".
		/// </summary>
		public string DataDirectory {get; set;}
		
		/// <summary>
		/// Sets the name used for the properties file (without path or extension).
		/// Use null (default) to use the default name.
		/// </summary>
		public string PropertiesName {get; set;}
		
		/// <summary>
		/// Sets the directory used to store the code completion cache.
		/// Use null (default) to disable the code completion cache.
		/// </summary>
		public string DomPersistencePath {get; set;}
		
		/// <summary>
		/// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
		/// </summary>
		public void AddAddInsFromDirectory(string addInDir)
		{
			if(addInDir == null)
				throw new ArgumentNullException("addInDir");
			
			addInDirectories.Add(addInDir);
		}
		
		/// <summary>
		/// Add the specified .addin file.
		/// </summary>
		public void AddAddInFile(string addInFile)
		{
			if(addInFile == null)
				throw new ArgumentNullException("addInFile");
			
			addInFiles.Add(addInFile);
		}
	}
}
