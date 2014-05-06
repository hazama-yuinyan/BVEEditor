/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/07/03
 * Time: 16:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using BVEEditor.AvalonDock;
using BVEEditor.Events;
using BVEEditor.Logging;
using BVEEditor.Result;
using BVEEditor.Services;
using BVEEditor.Settings;
using BVEEditor.Startup;
using BVEEditor.Strategies;
using BVEEditor.Views.Main;
using Caliburn.Micro;
//using Core.Presentation;
using ICSharpCode.Core;
//using ICSharpCode.SharpDevelop.Widgets;
using Microsoft.Win32;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// The view model for workbench.
	/// The following table lists some terms relating to the content shown in the main window and its meanings.
	/// <list type="bullet | number | table">
	/// 	<item>
	/// 		<term>ViewDocument</term>
	/// 		<description>The content type shown in the document area</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Pad</term>
	/// 		<description>The content type shown not in the document area</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>ViewContent</term>
	/// 		<description>One of the content in a single ViewDocument</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>WorkbenchWindow</term>
	/// 		<description>A window that wraps a ViewDocument or a Pad</description>
	/// 	</item>
	/// </list>
	/// </summary>
	public class WorkbenchViewModel : ShellPresentationViewModel, IWorkbench, IHandle<FileEvent>, /*IHandle<FileRenameEvent>,*/
        IHandle<ViewDocumentAddedEvent>, IHandle<ActiveViewDocumentChangedEvent>, IHandle<JumpLocationEvent>
	{
		const string PadContentPath = "/BVEEditor/Workbench/Pad";
		const string LayoutConfig = "LayoutConfig.xml";
        const string WorkbenchMemento = "WorkbenchMemento";
		
		Conductor<PadViewModel>.Collection.AllActive pad_conductor = new Conductor<PadViewModel>.Collection.AllActive();
        ViewDocumentConductor viewdoc_conductor = new ViewDocumentConductor();
        readonly IEventAggregator event_aggregator;
        //IFileSystem file_system;
        readonly IPropertyService property_service;
        readonly IFileDialogStrategies file_strategies;
        readonly IMessageService msg_service;
        readonly IDisplayBindingService display_binding;
        readonly ISettingsManager settings_manager;
        static readonly ILog Logger = LogManager.GetLog(typeof(WorkbenchViewModel));

        #region Binding sources
        public FlowDirection FlowDirection{
			get;
			private set;
		}

        public Rect RestoreBounds{
            get; set;
        }
		
        WindowState last_non_minimized_window_state;
		public WindowState LastNonMinimizedState{
			get{return last_non_minimized_window_state;}
			set{
                if(last_non_minimized_window_state != value){
                    last_non_minimized_window_state = value;
                    NotifyOfPropertyChange(() => LastNonMinimizedState);
                }
            }
		}

        public IList<PadViewModel> Pads{
            get{
                return pad_conductor.Items;
            }
        }

        public IList<ViewDocumentViewModel> ViewDocuments{
            get{
                return viewdoc_conductor.Items;
            }
        }
        #endregion

        #region View related events
        internal void OnViewOpened(ViewContentEvent e)
		{
			event_aggregator.Publish(e);
		}
		
		internal void OnViewClosed(ViewContentEvent e)
		{
			event_aggregator.Publish(e);
		}
		#endregion
		
		public Window MainWindow{
			get{return Application.Current.MainWindow;}
		}

        public MainMenuViewModel Menu{
            get; private set;
        }

        public ToolBarViewModel ToolBar{
            get; private set;
        }

        public StatusBarViewModel StatusBar{
            get; private set;
        }

        IRecentOpen recent_open;
        public IRecentOpen RecentOpen{
            get{return recent_open;}
        }
		
		public WorkbenchViewModel(/*IFileSystem fileService, */IEventAggregator eventAggregator, IPropertyService propertyService,
            IResultFactory resultFactory, IFileDialogStrategies fileStrategies, MainMenuViewModel mainMenuViewModel,
            IMessageService messageService, ViewDocumentConductor viewDocConductor, StatusBarViewModel statusBar,
            ToolBarViewModel toolBar, IDisplayBindingService displayBindingService,
            ISettingsManager settingsManager) : base(resultFactory)
		{
            event_aggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            //file_service = fileService;
            property_service = propertyService;
            file_strategies = fileStrategies;
            msg_service = messageService;
            Menu = mainMenuViewModel;
            StatusBar = statusBar;
            ToolBar = toolBar;
            display_binding = displayBindingService;
            settings_manager = settingsManager;

            mainMenuViewModel.Workbench = this;
            toolBar.Workbench = this;
            
            viewdoc_conductor.ConductWith(this);

            DisplayName = "BVEEditor";

            SetMemento(property_service.NestedProperties(WorkbenchMemento));    //restore workbench memento
			
			var descriptors = AddInTree.BuildItems<PadDescriptor>(PadContentPath, this, false);
			foreach(PadDescriptor content in descriptors){
				//if(content != null)
					//AddPad(content);
			}
			
			//Project.ProjectService.CurrentProjectChanged += SetProjectTitle;
			
			//SD.FileService.FileRemoved += ((RecentOpen)SD.FileService.RecentOpen).FileRemoved;
			//SD.FileService.FileRenamed += ((RecentOpen)SD.FileService.RecentOpen).FileRenamed;
			
            viewdoc_conductor.PropertyChanged += ViewDocConductorPropertyChanged;
		}

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            InitDocking();
            event_aggregator.Publish(new StatusBarMessageChangedEvent("${res:BVEEditor:StringResources:StatusBar.ReadyMessage}"));
        }

        protected override void OnInitialize()
        {
            Logger.Info("Initializing the workbench...");
            base.OnInitialize();
            // This causes the WPF Localization Extension library to synchronize its setting with System.Threading.Thread.CultureInfo
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.SetCurrentThreadCulture = true;

            settings_manager.LoadSettings();

            foreach(ICommand command in AddInTree.BuildItems<ICommand>("/BVEEditor/Workbench/AutostartAfterWorkbenchInitialized", null, false)){
                try{
                    command.Execute(null);
                }
                catch(Exception ex){
                    // allow startup to continue if some commands fail
                    msg_service.ShowException(ex);
                }
            }
        }

        protected override IEnumerable<IResult> CanClose()
        {
            var handle_dirty_results = ViewDocuments.SelectMany(HandleDocumentClosing);
            foreach(var result in handle_dirty_results)
                yield return result;
        }

        protected override void OnActivate()
        {
            Logger.Info("Activating the workbench...");
            base.OnActivate();

            Menu.Activate();
        }

        protected override void OnDeactivate(bool close)
        {
            Logger.Info("Deactivating the workbench with close={0}.", close);
            base.OnDeactivate(close);
            Menu.Deactivate(close);
            
            if(close)
                event_aggregator.Publish(new ApplicationExitingEvent());

            settings_manager.SaveSettings();
            DeinitDocking();

            var workbench_memento = CreateMemento();    // store workbench memento
            property_service.SetNestedProperties(WorkbenchMemento, workbench_memento);
            property_service.Save();
        }

        #region Layout mainipulation
        void InitDocking()
        {
            var path = System.IO.Path.Combine(property_service.ConfigDirectory, LayoutConfig);
            if(!System.IO.File.Exists(path))
                return;

            try {
                var serializer = new XmlLayoutSerializer(DockingManager);
                serializer.Deserialize(path);
            }
            catch {
                Log4netLogger.Instance.Error("Could not load layout configuration from {0}. Maybe the file is missing? Try to delete the file.", path);
                //SD.FileSystem.Delete(FileName.Create(path));
            }
        }

        void DeinitDocking()
        {
            var path = System.IO.Path.Combine(property_service.ConfigDirectory, LayoutConfig);

            try {
                var serializer = new XmlLayoutSerializer(DockingManager);
                serializer.Serialize(path);
            }
            catch {
                Log4netLogger.Instance.Error("Could not save the layout configuration. Any changes to the layout will be lost.");
            }
        }
        #endregion

        DockingManager DockingManager{
            get{return (this.GetView() as IDockingManagerProvider).DockingManager;}
        }
		
        /*void SetProjectTitle(object sender, Project.ProjectEventArgs e)
        {
            if (e.Project != null) {
                Title = e.Project.Name + " - " + ResourceService.GetString("MainWindow.DialogName");
            } else {
                Title = ResourceService.GetString("MainWindow.DialogName");
            }
        }*/

        #region Event handlers
        public void OnDragEnter(DragEventArgs e)
        {
            try{
                if(!e.Handled){
                    e.Effects = GetEffect(e.Data);
                    e.Handled = true;
                }
            }
            catch(Exception ex){
                msg_service.ShowException(ex);
            }
        }

        public void OnDragOver(DragEventArgs e)
        {
            try{
                if(!e.Handled){
                    e.Effects = GetEffect(e.Data);
                    e.Handled = true;
                }
            }
            catch(Exception ex){
                msg_service.ShowException(ex);
            }
        }

        DragDropEffects GetEffect(IDataObject data)
        {
            try {
                if(data != null && data.GetDataPresent(DataFormats.FileDrop)){
                    string[] files = (string[])data.GetData(DataFormats.FileDrop);
                    
                    if(files != null){
                        foreach(string file in files){
                            if(File.Exists(file))
                                return DragDropEffects.Link;
                        }
                    }
                }
            }
            catch(COMException){
                // Ignore errors getting the data (e.g. happens when dragging attachments out of Thunderbird)
            }
            return DragDropEffects.None;
        }

        protected void OnDrop(DragEventArgs e)
        {
            try{
                if(!e.Handled && e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)){
                    e.Handled = true;
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if(files == null)
                        return;

                    foreach(string file in files){
                        if(File.Exists(file)){
                            var file_name = FileName.Create(file);
                            /*if (SD.ProjectService.IsSolutionOrProjectFile(fileName)) {
                                SD.ProjectService.OpenSolutionOrProject(fileName);
                            } else {*/
                            CreateViewDocumentViewModel(file_name);
                            //}
                        }
                    }
                }
            }
            catch(Exception ex){
                msg_service.ShowException(ex);
            }
        }

        void OnRequestNavigate(RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        void ViewDocConductorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // We publish the ActiveDocumentChangedEvent here
            // because ViewDocumentConductor does the real job.
            if(e.PropertyName == "ActiveItem"){
                NotifyOfPropertyChange(() => ActiveDocument);

                // Ensure that no pending calls are in the dispatcher queue
                // This makes sure that we are blocked until bindings are re-established
                // (Bindings are, for example, required to scroll a selection into view for search/replace)
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (System.Action)delegate{
                    event_aggregator.Publish(new ActiveViewDocumentChangedEvent(viewdoc_conductor.ActiveItem));

                    /*if(value != null && this.mShutDownInProgress == false) {
                            if(value.IsFilePathReal == true)
                                this.Config.LastActiveFile = value.FilePath;
                    }*/
                });
            }
        }
        #endregion

        #region Properties representing contents
        public ICollection<ViewContentViewModel> ViewContentCollection {
			get {
				//SD.MainThread.VerifyAccess();
				return null;//return view_documents.SelectMany(d => d.ViewContents).ToList().AsReadOnly();
			}
		}
		
		public ICollection<ViewContentViewModel> PrimaryViewContents {
			get {
				//SD.MainThread.VerifyAccess();
				/*return (from document in view_documents
				        where document.ViewContents.Count > 0
				        select document.ViewContents[0]
				       ).ToList().AsReadOnly();*/
                return null;
			}
		}
		#endregion

		public ViewDocumentViewModel ActiveDocument{
			get{
				return viewdoc_conductor.ActiveItem;
			}
			private set{
                // Don't ever try to publish an ActiveDocumentChangedEvent here!
                // Since we delegate the job of setting active document to ViewDocumentConductor
                // it'll cause event re-publishing, ignorance of events or, in worst case, recursive calls
                // and lead to stack overflow.
				if(viewdoc_conductor.ActiveItem != value)
                    viewdoc_conductor.ActiveItem = value;
			}
		}
		
		#region Pad related methods
		#endregion
		
		#region ViewDocument related commands
        public IEnumerable<IResult> DocumentClosing(ViewDocumentViewModel document, DocumentClosingEventArgs args)
        {
            return HandleDocumentClosing(document);
        }

        public void DocumentClosed(ViewDocumentViewModel document)
        {
            StoreMemento(document);
            viewdoc_conductor.CloseDocument(document);
        }

        IEnumerable<IResult> HandleDocumentClosing(ViewDocumentViewModel document)
        {
            return HandleDocumentClosing(document, null);
        }

        IEnumerable<IResult> HandleDocumentClosing(ViewDocumentViewModel document, System.Action cancelCallback)
        {
            if(document.IsDirty){
                var res = Result.ShowMessageBox(StringParser.Parse("${res:BVEEditor:StringResources:Common.Dialogs.Captions.SaveBeforeClose}"),
                    StringParser.Format("${res:BVEEditor:StringResources:Common.Dialogs.Messages.SaveBeforeClose}", document.FileName), MessageBoxButton.YesNoCancel);
				yield return res;

				if(res.Result == System.Windows.MessageBoxResult.Cancel){
					yield return Result.Cancel(cancelCallback);
                }else if(res.Result == System.Windows.MessageBoxResult.Yes){
                    foreach(var result in file_strategies.Save(document, true, null))
					    yield return result;
                }
			}
        }

        public void CreateViewDocumentViewModel(string filePath)
        {
            FileName non_null_filename = FileName.Create(filePath ?? "Untitled.txt");
            FileName possibly_null_filename = FileName.Create(filePath);
            Logger.Info("Creating a ViewDocument for '{0}'.", non_null_filename);
            var new_doc = display_binding.GetBindingPerFileName(non_null_filename)
                                         .CreateViewModelForFile(possibly_null_filename);

            if(!string.IsNullOrEmpty(filePath)) // Loads content from the specified file if the path is not null
                new_doc.Load(filePath);

            event_aggregator.Publish(new ViewDocumentAddedEvent(new_doc));
        }

        public IEnumerable<IResult> SaveDocument(ViewDocumentViewModel document, bool doQuickSave)
        {
            return file_strategies.Save(document, doQuickSave, path => SaveInternal(document, path));
        }

        void SaveInternal(ViewDocumentViewModel document, string path)
        {
            document.FilePath = ICSharpCode.Core.FileName.Create(path);
            document.Save(path);
        }
		#endregion
		
		#region ViewContent Memento Handling
		FileName view_content_mementos_file_name;
		
		FileName ViewContentMementosFileName{
			get{
				if(view_content_mementos_file_name == null)
					view_content_mementos_file_name = FileName.Create(Path.Combine(property_service.ConfigDirectory, "LastViewStates.xml"));
					
				return view_content_mementos_file_name;
			}
		}
		
		ICSharpCode.Core.Properties LoadOrCreateViewContentMementos()
		{
			try{
				return ICSharpCode.Core.Properties.Load(this.ViewContentMementosFileName) ?? new ICSharpCode.Core.Properties();
			}catch(Exception ex){
				Logger.Warn("Error while loading the view content memento file. Discarding any saved view states.", ex);
				return new ICSharpCode.Core.Properties();
			}
		}
		
		static string GetMementoKeyName(ViewDocumentViewModel viewDocument)
		{
			return String.Concat(viewDocument.GetType().FullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture), ":",
			                     FileUtility.NormalizePath(viewDocument.FileName).ToUpperInvariant());
		}
		
		public bool WillLoadDocumentProperties{
			get{return property_service.Get("BVEEditor.LoadDocumentProperties", true);}
			set{property_service.Set("BVEEditor.LoadDocumentProperties", value);}
		}
		
		/// <summary>
		/// Stores the memento for the view content.
		/// Such mementos are automatically loaded in CreateViewDocumentViewModel().
		/// </summary>
		public void StoreMemento(ViewDocumentViewModel viewDocument)
		{
			IMementoCapable memento_capable = viewDocument as IMementoCapable;
			if(memento_capable != null && WillLoadDocumentProperties){
				if(viewDocument.FilePath == null)
					return;
				
				string key = GetMementoKeyName(viewDocument);
				Log4netLogger.Instance.Debug("Saving memento of '{0}' to key '{1}'.", viewDocument.ContentId, key);
				
				ICSharpCode.Core.Properties memento = memento_capable.CreateMemento();
				ICSharpCode.Core.Properties p = this.LoadOrCreateViewContentMementos();
				p.SetNestedProperties(key, memento);
				FileUtility.ObservedSave(new NamedFileOperationDelegate(p.Save), this.ViewContentMementosFileName, FileErrorPolicy.Inform);
			}
		}
		
		void LoadViewDocumentMemento(ViewDocumentViewModel viewDocument)
		{
			IMementoCapable memento_capable = viewDocument as IMementoCapable;
			if(memento_capable != null && WillLoadDocumentProperties){
				if(viewDocument.FileName == null)
					return;
				
				try{
					string key = GetMementoKeyName(viewDocument);
					Log4netLogger.Instance.Debug("Trying to restore memento of '{0}' from key '{1}'.", viewDocument.ToString(), key);
					
					memento_capable.SetMemento(LoadOrCreateViewContentMementos().NestedProperties(key));
				}catch(Exception e){
					msg_service.ShowException(e, "Can't get/set memento");
				}
			}
		}
		#endregion
		
		/*protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
			if(this.WindowState != System.Windows.WindowState.Minimized)
				last_non_minimized_window_state = this.WindowState;
		}*/
		
		public ICSharpCode.Core.Properties CreateMemento()
		{
			ICSharpCode.Core.Properties prop = new ICSharpCode.Core.Properties();
			prop.Set("WindowState", LastNonMinimizedState);
			var bounds = this.RestoreBounds;
			
			if(!bounds.IsEmpty)
				prop.Set("Bounds", bounds);
			
			return prop;
		}
		
		public void SetMemento(ICSharpCode.Core.Properties memento)
		{
			Rect bounds = memento.Get("Bounds", new Rect(10, 10, 750, 550));
			// bounds are validated after PresentationSource is initialized (see OnSourceInitialized)
			LastNonMinimizedState = memento.Get("WindowState", System.Windows.WindowState.Maximized);
			RestoreBounds = bounds;
		}
		
		/*public string CurrentLayoutConfiguration {
			get {
				return LayoutConfiguration.CurrentLayoutName;
			}
			set {
				LayoutConfiguration.CurrentLayoutName = value;
			}
		}*/
		
		/*public void LoadConfiguration()
		{
			if (!dockingManager.IsLoaded)
				return;
			Busy = true;
			try {
				TryLoadConfiguration();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
				// ignore errors loading configuration
			} finally {
				Busy = false;
			}
			foreach (AvalonPadContent p in pads.Values) {
				p.LoadPadContentIfRequired();
			}
		}
		
		void TryLoadConfiguration()
		{
			bool isPlainLayout = LayoutConfiguration.CurrentLayoutName == "Plain";
			if(File.Exists(LayoutConfiguration.CurrentLayoutFileName)){
				try {
					LoadLayout(LayoutConfiguration.CurrentLayoutFileName, isPlainLayout);
					return;
				} catch (FileFormatException) {
					// error when version of AvalonDock has changed: ignore and load template instead
				}
			}
			if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
				LoadLayout(LayoutConfiguration.CurrentLayoutTemplateFileName, isPlainLayout);
			}
		}
		
		void LoadLayout(string fileName, bool hideAllLostPads)
		{
			Log4netLogger.Instance.Info("Loading layout file: " + fileName + ", hideAllLostPads=" + hideAllLostPads);
//			DockableContent[] oldContents = dockingManager.DockableContents;
			dock_manager(fileName);
//			DockableContent[] newContents = dockingManager.DockableContents;
			// Restoring a AvalonDock layout will remove pads that are not
			// stored in the layout file.
			// We'll re-add those lost pads.
//			foreach (DockableContent lostContent in oldContents.Except(newContents)) {
//				AvalonPadContent padContent = lostContent as AvalonPadContent;
//				Log4netLogger.Instance.Debug("Re-add lost pad: " + padContent);
//				if (padContent != null && !hideAllLostPads) {
//					padContent.ShowInDefaultPosition();
//				} else {
//					dockingManager.Hide(lostContent);
//				}
//			}
		}
		
		public void StoreConfiguration()
		{
			try {
				LayoutConfiguration current = LayoutConfiguration.CurrentLayout;
				if (current != null && !current.IsReadOnly) {
					string configPath = LayoutConfiguration.ConfigLayoutPath;
					Directory.CreateDirectory(configPath);
					string fileName = Path.Combine(configPath, current.FileName);
					Log4netLogger.Instance.Info("Saving layout file: " + fileName);
					// Save docking layout into memory stream first, then write the contents to file.
					// This prevents corruption when there is an exception saving the layout.
					var memoryStream = new MemoryStream();
					dockingManager.SaveLayout(memoryStream);
					memoryStream.Position = 0;
					try {
						using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
							memoryStream.CopyTo(stream);
					} catch (IOException ex) {
						// ignore IO errors (maybe switching layout in two SharpDevelop instances at once?)
						Log4netLogger.Instance.Warn(ex);
					}
				}
			} catch (Exception e) {
				MessageService.ShowException(e);
			}
		}
		
		public void SwitchLayout(string layoutName)
		{
			StoreConfiguration();
			LayoutConfiguration.CurrentLayoutName = layoutName;
		}*/

        #region IHandle<FileEvent> メンバー

        public void Handle(FileEvent message)
        {
            /*foreach(OpenedFile file in file_system.OpenedFiles){
                if(FileUtility.IsBaseDirectory(message.FileName, file.FileName)){
                    foreach(ViewContentViewModel content in file.RegisteredViewContents.ToArray()){
                        // content.WorkbenchWindow can be null if multiple view contents
                        // were in the same WorkbenchWindow and both should be closed
                        // (e.g. Windows Forms Designer, Subversion History View)
                        if(content.ViewDocument != null)
                            HandleDocumentClosing(content.ViewDocument);
                    }
                }
            }*/
            //Editor.PermanentAnchorService.FileDeleted(e);
        }

        #endregion

        /*#region IHandle<FileRenameEvent> メンバー

        public void Handle(FileRenameEvent message)
        {
            if(message.IsDirectory){
                foreach(OpenedFile file in file_system.OpenedFiles){
                    if(file.FileName != null && FileUtility.IsBaseDirectory(message.SourceFile, file.FileName))
                        file.FileName = FileName.Create(FileUtility.RenameBaseDirectory(file.FileName, message.SourceFile, message.TargetFile));
                }
            }else{
                OpenedFile file = file_system.GetOpenedFile(message.SourceFile);
                if(file != null)
                    file.FileName = FileName.Create(message.TargetFile);
            }
            //Editor.PermanentAnchorService.FileRenamed(e);
        }

        #endregion*/

        #region IHandle<ViewDocumentAddedEvent> メンバー

        public void Handle(ViewDocumentAddedEvent message)
        {
            //See if the new ViewDocument already exists in view_documents collection.
            var doc = ViewDocuments.FirstOrDefault(view_doc => view_doc.ContentId == message.Document.ContentId);
            
            if(doc == null){
                doc = message.Document;
                viewdoc_conductor.AddDocument(doc);
            }

            //if(message.ShowOnAdded)
            //    ActiveDocument = doc;
        }

        #endregion

        #region IHandle<ActiveViewDocumentChangedEvent> メンバー

        public void Handle(ActiveViewDocumentChangedEvent message)
        {
            LoadViewDocumentMemento(message.ViewDocument);
        }

        #endregion

        #region IHandle<JumpLocationEvent> メンバー

        public void Handle(JumpLocationEvent message)
        {
            var open_doc = ViewDocuments.Where(doc => doc.FilePath == message.FileName).Single();
            /*if(open_doc == null){
                open_doc = Menu.CreateViewDocumentViewModel
            }*/
            
        }

        #endregion
    }
}
