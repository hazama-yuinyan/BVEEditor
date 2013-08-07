/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/15
 * Time: 18:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using BVEEditor.AvalonDock;
using BVEEditor.Commands;
using BVEEditor.Logging;
using BVEEditor.Result;
using BVEEditor.Strategies;
using BVEEditor.Views;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;
using Ninject;

namespace BVEEditor.Startup
{
	/// <summary>
	/// The bootstrapper for BVEEditor.
	/// </summary>
	public class Bootstrapper : Bootstrapper<IWorkbench>    //Because we register WorkbenchViewModel class as IWorkbench on our IoC container,
	{                                                       //we should tell the bootstrapper that IWorkbench is the root view model class.
        IKernel kernel;
        System.Action addin_initializer;
		
		#region Bootstrapper overrides
		protected override void Configure()
		{
            init_status = InitStatus.CoreInitializing;
			kernel = ServiceBootstrapper.Create();
			
            kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            kernel.Bind<IResultFactory>().To<ResultFactory>();
            kernel.Bind<IFileDialogStrategies>().To<FileDialogStrategies>();
            kernel.Bind<IWorkbench>().To<WorkbenchViewModel>().InSingletonScope();

            SetupCustomMessageBindings();
			
			StartupSettings startup = new StartupSettings();
				
			Assembly exe = typeof(App).Assembly;
			startup.ApplicationRootPath = Path.Combine(Path.GetDirectoryName(exe.Location), "..");
			startup.AllowUserAddIns = true;
				
			string configDirectory = ConfigurationManager.AppSettings["settingsPath"];
			if(String.IsNullOrEmpty(configDirectory)){
				startup.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				                                       "BVEEditor1");// + RevisionClass.Major);
			}else{
				startup.ConfigDirectory = Path.Combine(Path.GetDirectoryName(exe.Location), configDirectory);
			}
				
			startup.DomPersistencePath = ConfigurationManager.AppSettings["domPersistencePath"];
			if(string.IsNullOrEmpty(startup.DomPersistencePath)){
				startup.DomPersistencePath = Path.Combine(Path.GetTempPath(), "BVEEditor" + "1.0");//RevisionClass.Major + "." + RevisionClass.Minor);
				#if DEBUG
				startup.DomPersistencePath = Path.Combine(startup.DomPersistencePath, "Debug");
				#endif
			}else if(startup.DomPersistencePath == "none"){
				startup.DomPersistencePath = null;
			}
				
			startup.AddAddInsFromDirectory(Path.Combine(startup.ApplicationRootPath, "AddIns"));
			InitBVEEditorCore(startup);

            init_status = InitStatus.CoreInitialized;
		}
		
		protected override object GetInstance(Type service, string key)
		{
            return kernel.Get(service, key);
		}
		
		protected override IEnumerable<object> GetAllInstances(Type service)
		{
			return kernel.GetAll(service);
		}
		
		protected override void BuildUp(object instance)
		{
            kernel.Inject(instance);
		}
		
		protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
		{
            Log4netLogger.Instance.Info("Run OnStartup...");
            StringParser.RegisterStringTagProvider(new BVEEditorStringTagProvider(IoC.Get<IWorkbench>()));
            Log4netLogger.Instance.Info("Initializing AddinTree...");
            addin_initializer();
            addin_initializer = null;

            init_status = InitStatus.Busy;
			base.OnStartup(sender, e);
		}
		#endregion

        void SetupCustomMessageBindings()
        {
            DocumentContext.Init();
        }
		
		#region Static helpers
		internal static Assembly BVEEditorAssembly {
			get {
				return typeof(Bootstrapper).Assembly;
			}
		}
		#endregion
		
		#region InitStatus enum
		enum InitStatus
		{
			None,
            CoreInitializing,
			CoreInitialized,
			WorkbenchInitialized,
			Busy,
			WorkbenchUnloaded,
			AppDomainUnloaded
		}
		#endregion
		
		InitStatus init_status;
		
		#region Initialize Core
		public void InitBVEEditorCore(StartupSettings properties)
		{
			// Initialize the most important services:
			/*var container = new BVEEditorServiceContainer(ServiceSingleton.FallbackServiceProvider);
			container.AddService(typeof(IMessageService), new BVEEditorMessageService());
			container.AddService(typeof(ILoggingService), new log4netLoggingService());
			ServiceSingleton.ServiceProvider = container;*/
			
			Log4netLogger.Instance.Info("InitBVEEditor...");
			CoreStartup startup = new CoreStartup(properties.ApplicationName);
			/*if (properties.UseSharpDevelopErrorHandler) {
				this.useSharpDevelopErrorHandler = true;
				ExceptionBox.RegisterExceptionBoxForUnhandledExceptions();
			}*/
			startup.ConfigDirectory = properties.ConfigDirectory;
			startup.DataDirectory = properties.DataDirectory;
			if(properties.PropertiesName != null)
				startup.PropertiesName = properties.PropertiesName;
			
			if(properties.ApplicationRootPath != null)
				FileUtility.ApplicationRootPath = properties.ApplicationRootPath;
			
			var core_services = startup.StartCoreServices();    //register services provided by ICSharpCode.Core
            foreach(var service in core_services.Item1)
                kernel.Bind(service.Key).To(service.Value).InSingletonScope();

            foreach(var inst in core_services.Item2)
                kernel.Bind(inst.Key).ToConstant(inst.Value);

            //var resource_service = core_services.Item2.Select(inst => inst.;

			//Assembly exe = Assembly.Load(properties.ResourceAssemblyName);
			//resource_service.RegisterNeutralStrings(new ResourceManager("BVEEditor.Resources.StringResources", exe));
			//resource_service.RegisterNeutralImages(new ResourceManager("BVEEditor.Resources.BitmapResources", exe));
			
			/*CommandWrapper.LinkCommandCreator = (link => new LinkCommand(link));
			//CommandWrapper.WellKnownCommandCreator = Core.Presentation.MenuService.GetKnownCommand;
			CommandWrapper.RegisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested += eh);
			CommandWrapper.UnregisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested -= eh);*/
			
			Log4netLogger.Instance.Info("Looking for AddIns...");
			foreach(string file in properties.addInFiles)
				startup.AddAddInFile(file);

			foreach(string dir in properties.addInDirectories)
				startup.AddAddInsFromDirectory(dir);

            var prop_service = GetInstance(typeof(IPropertyService), null) as IPropertyService;
			
			if(properties.AllowAddInConfigurationAndExternalAddIns)
				startup.ConfigureExternalAddIns(Path.Combine(prop_service.ConfigDirectory, "AddIns.xml"));
			
			if(properties.AllowUserAddIns){
				startup.ConfigureUserAddIns(Path.Combine(prop_service.ConfigDirectory, "AddInInstallTemp"),
				                            Path.Combine(prop_service.ConfigDirectory, "AddIns"));
			}
            addin_initializer = startup.RunInitialization;
			
			//((AssemblyParserService)SD.AssemblyParserService).DomPersistencePath = properties.DomPersistencePath;
			
			Log4netLogger.Instance.Info("InitBVEEditor finished");
		}
		#endregion
	}
}