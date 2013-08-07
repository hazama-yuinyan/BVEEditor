// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Input;
using Caliburn.Micro;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class that helps starting up ICSharpCode.Core.
	/// </summary>
	/// <remarks>
	/// Initializing ICSharpCode.Core requires initializing several static classes
	/// and the <see cref="AddInTree"/>. <see cref="CoreStartup"/> does this work
	/// for you, provided you use it like this:
	/// 1. Create a new CoreStartup instance
	/// 2. (Optional) Set the values of the properties.
	/// 3. Call <see cref="StartCoreServices()"/>.
	/// 4. Add "preinstalled" AddIns using <see cref="AddAddInsFromDirectory"/>
	///    and <see cref="AddAddInFile"/>.
	/// 5. (Optional) Call <see cref="ConfigureExternalAddIns"/> to support
	///    disabling AddIns and installing external AddIns
	/// 6. (Optional) Call <see cref="ConfigureUserAddIns"/> to support installing
	///    user AddIns.
	/// 7. Call <see cref="RunInitialization"/>.
	/// </remarks>
	public sealed class CoreStartup
	{
		List<string> addInFiles = new List<string>();
		List<string> disabledAddIns = new List<string>();
		bool externalAddInsConfigured;
		string properties_name;
		string config_directory;
		string data_directory;
		string application_name;
		AddInTreeImpl addInTree;
		
		/// <summary>
		/// Sets the name used for the properties (only name, without path or extension).
		/// Must be set before StartCoreServices() is called.
		/// </summary>
		public string PropertiesName {
			get {
				return properties_name;
			}
			set {
				if(value == null || value.Length == 0)
					throw new ArgumentNullException("value");
				
                properties_name = value;
			}
		}
		
		/// <summary>
		/// Sets the directory name used for the property service.
		/// Must be set before StartCoreServices() is called.
		/// Use null to use the default path "%ApplicationData%\%ApplicationName%",
		/// where %ApplicationData% is the system setting for
		/// "c:\documents and settings\username\application data"
		/// and %ApplicationName% is the application name you used in the
		/// CoreStartup constructor call.
		/// </summary>
		public string ConfigDirectory {
			get {
				return config_directory;
			}
			set {
				config_directory = value;
			}
		}
		
		/// <summary>
		/// Sets the data directory used to load resources.
		/// Must be set before StartCoreServices() is called.
		/// Use null to use the default path "ApplicationRootPath\data".
		/// </summary>
		public string DataDirectory {
			get {
				return data_directory;
			}
			set {
				data_directory = value;
			}
		}
		
		/// <summary>
		/// Creates a new CoreStartup instance.
		/// </summary>
		/// <param name="applicationName">
		/// The name of your application.
		/// This is used as default title for message boxes,
		/// default name for the configuration directory etc.
		/// </param>
		public CoreStartup(string applicationName)
		{
			if(applicationName == null)
				throw new ArgumentNullException("applicationName");
			
            this.application_name = applicationName;
			properties_name = applicationName + "Properties";
		}
		
		/// <summary>
		/// Find AddIns by searching all .addin files recursively in <paramref name="addInDir"/>.
		/// The AddIns that were found are added to the list of AddIn files to load.
		/// </summary>
		public void AddAddInsFromDirectory(string addInDir)
		{
			if(addInDir == null)
				throw new ArgumentNullException("addInDir");
			
            addInFiles.AddRange(Directory.GetFiles(addInDir, "*.addin", SearchOption.AllDirectories));
		}
		
		/// <summary>
		/// Add the specified .addin file to the list of AddIn files to load.
		/// </summary>
		public void AddAddInFile(string addInFile)
		{
			if(addInFile == null)
				throw new ArgumentNullException("addInFile");
			
            addInFiles.Add(addInFile);
		}
		
		/// <summary>
		/// Use the specified configuration file to store information about
		/// disabled AddIns and external AddIns.
		/// You have to call this method to support the <see cref="AddInManager"/>.
		/// </summary>
		/// <param name="addInConfigurationFile">
		/// The name of the file used to store the list of disabled AddIns
		/// and the list of installed external AddIns.
		/// A good value for this parameter would be
		/// <c>Path.Combine(<see cref="PropertyService.ConfigDirectory"/>, "AddIns.xml")</c>.
		/// </param>
		public void ConfigureExternalAddIns(string addInConfigurationFile)
		{
			externalAddInsConfigured = true;
			AddInManager.ConfigurationFileName = addInConfigurationFile;
			AddInManager.LoadAddInConfiguration(addInFiles, disabledAddIns);
		}
		
		/// <summary>
		/// Configures user AddIn support.
		/// </summary>
		/// <param name="addInInstallTemp">
		/// The AddIn installation temporary directory.
		/// ConfigureUserAddIns will install the AddIns from this directory and
		/// store the parameter value in <see cref="AddInManager.AddInInstallTemp"/>.
		/// </param>
		/// <param name="userAddInPath">
		/// The path where user AddIns are installed to.
		/// AddIns from this directory will be loaded.
		/// </param>
		public void ConfigureUserAddIns(string addInInstallTemp, string userAddInPath)
		{
			if(!externalAddInsConfigured)
				throw new InvalidOperationException("ConfigureExternalAddIns must be called before ConfigureUserAddIns");
			
            AddInManager.AddInInstallTemp = addInInstallTemp;
			AddInManager.UserAddInPath = userAddInPath;
			if(Directory.Exists(addInInstallTemp))
				AddInManager.InstallAddIns(disabledAddIns);
			
			if(Directory.Exists(userAddInPath))
				AddAddInsFromDirectory(userAddInPath);
		}
		
		/// <summary>
		/// Initializes the AddIn system.
		/// This loads the AddIns that were added to the list,
		/// then it executes the <see cref="ICommand">commands</see>
		/// in <c>/BVEEditor/Autostart</c>.
		/// </summary>
		public void RunInitialization()
		{
			addInTree.Load(addInFiles, disabledAddIns);
			
			// perform service registration
			//var container = IoC.Get<IServiceContainer>();
			//if(container != null)
			//	addInTree.BuildItems<object>("/BVEEditor/Services", container, false);
			
			// run workspace autostart commands
			DebugLogger.Instance.Info("Running autostart commands...");
			foreach(ICommand command in addInTree.BuildItems<ICommand>("/BVEEditor/Autostart", null, false)){
				try {
					command.Execute(null);
				}catch(Exception ex){
					// allow startup to continue if some commands fail
					IoC.Get<IMessageService>().ShowException(ex);
				}
			}
		}
		
		/// <summary>
		/// Starts the core services.
		/// This initializes the PropertyService and ResourceService.
		/// </summary>
        /// <returns>A list containing service types and the corresponding instances.</returns>
		public Tuple<IEnumerable<KeyValuePair<Type, Type>>, IEnumerable<KeyValuePair<Type, object>>> StartCoreServices()
		{
			if(config_directory == null)
				config_directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				                               application_name);
			
			var property_service = new PropertyServiceImpl(
				DirectoryName.Create(config_directory),
				DirectoryName.Create(data_directory ?? Path.Combine(FileUtility.ApplicationRootPath, "data")),
				properties_name);
			var application_state_info_service = new ApplicationStateInfoService();
			addInTree = new AddInTreeImpl(application_state_info_service);

			var services = new List<KeyValuePair<Type, Type>>();
            var instances = new List<KeyValuePair<Type, object>>();
            instances.Add(new KeyValuePair<Type,object>(typeof(IPropertyService), property_service));
            //instances.Add(new KeyValuePair<Type,object>(typeof(IResourceService), new ResourceServiceImpl(
            //    Path.Combine(property_service.DataDirectory, "resources"), property_service)));
            instances.Add(new KeyValuePair<Type,object>(typeof(IAddInTree), addInTree));
            
            services.Add(new KeyValuePair<Type,Type>(typeof(ApplicationStateInfoService), typeof(ApplicationStateInfoService)));
			StringParser.RegisterStringTagProvider(new AppNameProvider{appName = application_name});

            return Tuple.Create((IEnumerable<KeyValuePair<Type, Type>>)services, (IEnumerable<KeyValuePair<Type, object>>)instances);
		}
		
        /// <summary>
        /// As the name suggests, it is a StringTagProvider that provides the application name.
        /// </summary>
		sealed class AppNameProvider : IStringTagProvider
		{
			internal string appName;
			
			public string ProvideString(string tag, StringTagPair[] customTags)
			{
				if (string.Equals(tag, "AppName", StringComparison.OrdinalIgnoreCase))
					return appName;
				else
					return null;
			}
		}
	}
}
