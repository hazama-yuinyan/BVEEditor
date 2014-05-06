/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/17
 * Time: 14:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using BVEEditor.Editor;
using Caliburn.Micro;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Workbench
{
    sealed class FileService : IFileService
    {
        IPropertyService property_service;
        IEventAggregator event_aggregator;
        IWorkbench workbench;
        IDisplayBindingService display_binding_service;

        public FileService(IPropertyService propertyService, IEventAggregator eventAggregator, IWorkbench workbench,
            IDisplayBindingService displayBindingService)
        {
            property_service = propertyService;
            event_aggregator = eventAggregator;
            this.workbench = workbench;
            display_binding_service = displayBindingService;
            //SD.ParserService.LoadSolutionProjectsThread.Finished += ParserServiceLoadSolutionProjectsThreadEnded;
        }

        /*void ParserServiceLoadSolutionProjectsThreadEnded(object sender, EventArgs e)
        {
            var displayBindingService = SD.DisplayBindingService;
            foreach(IViewContent content in SD.Workbench.ViewContentCollection.ToArray())
                displayBindingService.AttachSubWindows(content, true);
        }*/

        #region Options
        /// <summary>used for OptionBinding</summary>
        /*public static FileService Instance {
            get { return (FileService)SD.FileService; }
        }*/

        IRecentOpen recent_open;

        public IRecentOpen RecentOpen{
            get{
                return LazyInitializer.EnsureInitialized(
                    ref recent_open, () => new RecentOpen(property_service.NestedProperties("RecentOpen")));
            }
        }

        public bool DeleteToRecycleBin{
            get{
                return property_service.Get("BVEEditor.DeleteToRecycleBin", true);
            }
            
            set{
                property_service.Set("BVEEditor.DeleteToRecycleBin", value);
            }
        }

        public bool SaveUsingTemporaryFile{
            get{
                return property_service.Get("BVEEditor.SaveUsingTemporaryFile", true);
            }
            
            set{
                property_service.Set("BVEEditor.SaveUsingTemporaryFile", value);
            }
        }
        #endregion

        #region DefaultFileEncoding
        public int DefaultFileEncodingCodePage{
            get { return property_service.Get("BVEEditor.DefaultFileEncoding", 65001); }
            set { property_service.Set("BVEEditor.DefaultFileEncoding", value); }
        }

        public Encoding DefaultFileEncoding{
            get{
                return Encoding.GetEncoding(DefaultFileEncodingCodePage);
            }
        }

        readonly EncodingInfo[] allEncodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName).ToArray();

        public IReadOnlyList<EncodingInfo> AllEncodings{
            get { return allEncodings; }
        }

        public EncodingInfo DefaultFileEncodingInfo{
            get{
                int cp = DefaultFileEncodingCodePage;
                return allEncodings.Single(e => e.CodePage == cp);
            }

            set{
                DefaultFileEncodingCodePage = value.CodePage;
            }
        }
        #endregion

        #region GetFileContent
        public ITextSource GetFileContent(FileName fileName)
        {
            return GetFileContentForOpenFile(fileName) ?? GetFileContentFromDisk(fileName, CancellationToken.None);
        }

        public ITextSource GetFileContent(string fileName)
        {
            return GetFileContent(FileName.Create(fileName));
        }

        public ITextSource GetFileContentForOpenFile(FileName fileName)
        {
            return SD.MainThread.InvokeIfRequired(
                delegate{
                    OpenedFile file = this.GetOpenedFile(fileName);
                    if(file != null){
                        if(file.CurrentView != null){
                            /*IFileDocumentProvider provider = file.CurrentView.ViewDocument;
                            if(provider != null){
                                IDocument document = provider.GetDocumentForFile(file);
                                if(document != null)
                                    return document.CreateSnapshot();
                            }*/
                        }

                        using(Stream s = file.OpenRead()){
                            // load file
                            return new StringTextSource(FileReader.ReadFileContent(s, DefaultFileEncoding));
                        }
                    }
                    return null;
                });
        }

        public ITextSource GetFileContentFromDisk(FileName fileName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            string text = FileReader.ReadFileContent(fileName, DefaultFileEncoding);
            DateTime lastWriteTime = File.GetLastWriteTimeUtc(fileName);
            return new StringTextSource(text, new OnDiskTextSourceVersion(lastWriteTime));
        }
        #endregion

        #region BrowseForFolder
        public string BrowseForFolder(string description, string selectedPath)
        {
            using(FolderBrowserDialog dialog = new FolderBrowserDialog()){
                dialog.Description = StringParser.Parse(description);
                if(selectedPath != null && selectedPath.Length > 0 && Directory.Exists(selectedPath)){
                    dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                    dialog.SelectedPath = selectedPath;
                }
                if(dialog.ShowDialog() == DialogResult.OK)
                    return dialog.SelectedPath;
                else
                    return null;
            }
        }
        #endregion

        #region OpenedFile
        Dictionary<FileName, OpenedFile> opened_file_dict = new Dictionary<FileName, OpenedFile>();

        /// <inheritdoc/>
        public IReadOnlyList<OpenedFile> OpenedFiles{
            get{
                //SD.MainThread.VerifyAccess();
                return opened_file_dict.Values.ToArray();
            }
        }

        /// <inheritdoc/>
        public OpenedFile GetOpenedFile(string fileName)
        {
            return GetOpenedFile(FileName.Create(fileName));
        }

        /// <inheritdoc/>
        public OpenedFile GetOpenedFile(FileName fileName)
        {
            if(fileName == null)
                throw new ArgumentNullException("fileName");

            //SD.MainThread.VerifyAccess();

            OpenedFile file;
            opened_file_dict.TryGetValue(fileName, out file);
            return file;
        }

        /// <inheritdoc/>
        public OpenedFile GetOrCreateOpenedFile(string fileName)
        {
            return GetOrCreateOpenedFile(FileName.Create(fileName));
        }

        /// <inheritdoc/>
        public OpenedFile GetOrCreateOpenedFile(FileName fileName)
        {
            if(fileName == null)
                throw new ArgumentNullException("fileName");

            OpenedFile file;
            if(!opened_file_dict.TryGetValue(fileName, out file))
                opened_file_dict[fileName] = file = new FileServiceOpenedFile(this, fileName);

            return file;
        }

        /// <inheritdoc/>
        public OpenedFile CreateUntitledOpenedFile(string defaultName, byte[] content)
        {
            if(defaultName == null)
                throw new ArgumentNullException("defaultName");

            OpenedFile file = new FileServiceOpenedFile(this, content);
            file.FileName = new FileName(file.GetHashCode() + "/" + defaultName);
            opened_file_dict[file.FileName] = file;
            return file;
        }

        /// <summary>Called by OpenedFile.set_FileName to update the dictionary.</summary>
        internal void OpenedFileFileNameChange(OpenedFile file, FileName oldName, FileName newName)
        {
            if(oldName == null)
                return; // File just created with NewFile where name is being initialized.

            Log4netLogger.Instance.Debug("OpenedFileFileNameChange: " + oldName + " => " + newName);

            if(opened_file_dict[oldName] != file)
                throw new ArgumentException("file must be registered as oldName");

            if(opened_file_dict.ContainsKey(newName)){
                OpenedFile old_file = opened_file_dict[newName];
                if(old_file.CurrentView != null){
                    if(old_file.CurrentView.ViewDocument != null)
                        workbench.Close(old_file.CurrentView.ViewDocument);
                }else{
                    throw new ArgumentException("there already is a file with the newName");
                }
            }
            opened_file_dict.Remove(oldName);
            opened_file_dict[newName] = file;
        }

        /// <summary>Called by OpenedFile.UnregisterView to update the dictionary.</summary>
        internal void OpenedFileClosed(OpenedFile file)
        {
            OpenedFile existing;
            if(opened_file_dict.TryGetValue(file.FileName, out existing) && existing != file)
                throw new ArgumentException("file must be registered");

            opened_file_dict.Remove(file.FileName);
            Log4netLogger.Instance.Debug("OpenedFileClosed: " + file.FileName);
        }
        #endregion

        #region CheckFileName
        /// <inheritdoc/>
        public bool CheckFileName(string path)
        {
            if(FileUtility.IsValidPath(path))
                return true;

            MessageService.ShowMessage(StringParser.Parse("${res:BVEEditor.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", path)));
            return false;
        }

        /// <inheritdoc/>
        public bool CheckDirectoryEntryName(string name)
        {
            if(FileUtility.IsValidDirectoryEntryName(name))
                return true;

            MessageService.ShowMessage(StringParser.Parse("${res:BVEEditor.Commands.SaveFile.InvalidFileNameError}", new StringTagPair("FileName", name)));
            return false;
        }
        #endregion

        #region OpenFile (ViewContent)
        /// <inheritdoc/>
        public bool IsOpen(FileName fileName)
        {
            return GetOpenFile(fileName) != null;
        }

        /// <inheritdoc/>
        public ViewDocumentViewModel OpenFile(FileName fileName)
        {
            return OpenFile(fileName, true);
        }

        /// <inheritdoc/>
        public ViewDocumentViewModel OpenFile(FileName fileName, bool switchToOpenedView)
        {
            Log4netLogger.Instance.Info("Open file " + fileName);

            ViewDocumentViewModel view_document = GetOpenFile(fileName);
            if(view_document != null){
                if(switchToOpenedView)
                    view_document.Select();

                return view_document;
            }

            IDisplayBinding binding = display_binding_service.GetBindingPerFileName(fileName);

            if(binding == null)
                binding = new ErrorFallbackBinding("Could not find any display binding for " + Path.GetFileName(fileName));

            if(FileUtility.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(binding, switchToOpenedView).Invoke), fileName) == FileOperationResult.OK)
                RecentOpen.AddRecentFile(fileName);

            return GetOpenFile(fileName);
        }

        /// <inheritdoc/>
        public ViewDocumentViewModel OpenFileWith(FileName fileName, IDisplayBinding displayBinding, bool switchToOpenedView)
        {
            if(displayBinding == null)
                throw new ArgumentNullException("displayBinding");

            //if(FileUtility.ObservedLoad(new NamedFileOperationDelegate(new LoadFileWrapper(displayBinding, switchToOpenedView).Invoke), fileName) == FileOperationResult.OK)
             //   RecentOpen.AddRecentFile(fileName);

            return GetOpenFile(fileName);
        }

        sealed class LoadFileWrapper
        {
            readonly IDisplayBinding binding;
            readonly bool switchToOpenedView;

            public LoadFileWrapper(IDisplayBinding binding, bool switchToOpenedView)
            {
                this.binding = binding;
                this.switchToOpenedView = switchToOpenedView;
            }

            public void Invoke(FileName fileName)
            {
                OpenedFile file = SD.FileService.GetOrCreateOpenedFile(fileName);
                try{
                    ViewDocumentViewModel new_doc = binding.CreateViewModelForFile(file);
                    if(new_doc != null){
                        //SD.DisplayBindingService.AttachSubWindows(newContent, false);
                        //SD.Workbench.ShowView(newContent, switchToOpenedView);
                    }
                }
                finally{
                    file.CloseIfAllViewsClosed();
                }
            }
        }

        /// <inheritdoc/>
        public ViewDocumentViewModel NewFile(string defaultName, string content)
        {
            return NewFile(defaultName, DefaultFileEncoding.GetBytesWithPreamble(content));
        }

        /// <inheritdoc/>
        public ViewDocumentViewModel NewFile(string defaultName, byte[] content)
        {
            if(defaultName == null)
                throw new ArgumentNullException("defaultName");

            if(content == null)
                throw new ArgumentNullException("content");

            IDisplayBinding binding = display_binding_service.GetBindingPerFileName(FileName.Create(defaultName));

            if(binding == null){
                binding = new ErrorFallbackBinding("Can't create display binding for file " + defaultName);
            }
            OpenedFile file = CreateUntitledOpenedFile(defaultName, content);

            ViewDocumentViewModel new_doc = binding.CreateViewModelForFile(file);
            if(new_doc == null){
                Log4netLogger.Instance.Warn("Created view content was null - DefaultName:" + defaultName);
                file.CloseIfAllViewsClosed();
                return null;
            }

            //displayBindingService.AttachSubWindows(newContent, false);

            workbench.AddDocument(new_doc);
            return new_doc;
        }

        /// <inheritdoc/>
        public IReadOnlyList<FileName> OpenPrimaryFiles{
            get{
                List<FileName> fileNames = new List<FileName>();
                foreach(ViewDocumentViewModel document in workbench.Documents){
                    FileName content_name = document.File.FileName;
                    if(content_name != null && !fileNames.Contains(content_name))
                        fileNames.Add(content_name);
                }
                return fileNames;
            }
        }

        /// <inheritdoc/>
        public ViewDocumentViewModel GetOpenFile(FileName fileName)
        {
            if(fileName != null){
                foreach(ViewDocumentViewModel document in workbench.Documents){
                    string content_name = document.FileName;
                    if(content_name != null){
                        if(FileUtility.IsEqualFileName(fileName, content_name))
                            return document;
                    }
                }
            }
            return null;
        }

        sealed class ErrorFallbackBinding : IDisplayBinding
        {
            string error_message;

            public ErrorFallbackBinding(string errorMessage)
            {
                this.error_message = errorMessage;
            }

            public bool IsPreferredBindingForFile(FileName fileName)
            {
                return true;
            }

            public bool CanHandle(FileName fileName)
            {
                return true;
            }

            public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
            {
                return double.NegativeInfinity;
            }

            public ViewDocumentViewModel CreateViewModelForFile(OpenedFile file)
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public ViewDocumentViewModel JumpToFilePosition(FileName fileName, int line, int column)
        {
            Log4netLogger.Instance.InfoFormatted("FileService\n\tJumping to File Position:  [{0} : {1}x{2}]", fileName, line, column);

            if(fileName == null)
                return null;

            //NavigationService.SuspendLogging();
            //bool loggingResumed = false;

            try{
                ViewDocumentViewModel document = OpenFile(fileName);
                if(document is IPositionable){
                    // TODO: enable jumping to a particular view
                    //document.WorkbenchWindow.ActiveViewContent = document;
                    //NavigationService.ResumeLogging();
                    //loggingResumed = true;
                    ((IPositionable)document).JumpTo(Math.Max(1, line), Math.Max(1, column));
                }else{
                    //NavigationService.ResumeLogging();
                    //loggingResumed = true;
                    //NavigationService.Log(content);
                }

                return document;

            }
            finally{
                Log4netLogger.Instance.InfoFormatted("FileService\n\tJumped to File Position:  [{0} : {1}x{2}]", fileName, line, column);

                /*if(!loggingResumed)
                    NavigationService.ResumeLogging();*/
            }
        }

        public IEnumerable<ViewContentViewModel> ShowOpenWithDialog(IEnumerable<FileName> fileNames, bool switchToOpenedView = true)
        {
            /*var fileNamesList = fileNames.ToList();
            if(fileNamesList.Count == 0)
                return Enumerable.Empty<ViewContentViewModel>();

            var displayBindingService = SD.DisplayBindingService;
            List<DisplayBindingDescriptor> codons = displayBindingService.GetCodonsPerFileName(fileNamesList[0]).ToList();
            for(int i = 1; i < fileNamesList.Count; i++) {
                var codonsForThisFile = displayBindingService.GetCodonsPerFileName(fileNamesList[i]);
                codons.RemoveAll(c => !codonsForThisFile.Contains(c));
            }
            if(codons.Count == 0)
                return Enumerable.Empty<ViewContentViewModel>();

            int defaultCodonIndex = codons.IndexOf(displayBindingService.GetDefaultCodonPerFileName(fileNamesList[0]));
            if(defaultCodonIndex < 0)
                defaultCodonIndex = 0;
            /*using (OpenWithDialog dlg = new OpenWithDialog(codons, defaultCodonIndex, Path.GetExtension(fileNamesList[0]))) {
                if (dlg.ShowDialog(SD.WinForms.MainWin32Window) == DialogResult.OK) {
                    var result = new List<IViewContent>();
                    foreach (var fileName in fileNamesList) {
                        IViewContent vc = OpenFileWith(fileName, dlg.SelectedBinding.Binding, switchToOpenedView);
                        if (vc != null)
                            result.Add(vc);
                    }
                    return result;
                }
            }*/
            return Enumerable.Empty<ViewContentViewModel>();
        }
        #endregion

        #region Remove/Rename/Copy
        /// <summary>
        /// Removes a file, raising the appropriate events. This method may show message boxes.
        /// </summary>
        public void RemoveFile(string fileName, bool isDirectory)
        {
            FileCancelEvent eargs = new FileCancelEvent(fileName, isDirectory);
            event_aggregator.Publish(eargs);

            if(eargs.Cancel)
                return;

            if(!eargs.OperationAlreadyDone){
                if(isDirectory){
                    try{
                        if(Directory.Exists(fileName)){
                            /*if(DeleteToRecycleBin)
                                NativeMethods.DeleteToRecycleBin(fileName);
                            else*/
                                Directory.Delete(fileName, true);
                        }
                    }
                    catch(Exception e){
                        MessageService.ShowHandledException(e, "Can't remove directory " + fileName);
                    }
                }else{
                    try{
                        if(File.Exists(fileName)){
                            /*if(DeleteToRecycleBin)
                                NativeMethods.DeleteToRecycleBin(fileName);
                            else*/
                                File.Delete(fileName);
                        }
                    }
                    catch(Exception e){
                        MessageService.ShowHandledException(e, "Can't remove file " + fileName);
                    }
                }
            }

            event_aggregator.Publish(new FileEvent(fileName, isDirectory));
        }

        /// <summary>
        /// Renames or moves a file, raising the appropriate events. This method may show message boxes.
        /// </summary>
        public bool RenameFile(string oldName, string newName, bool isDirectory)
        {
            if(FileUtility.IsEqualFileName(oldName, newName))
                return false;

            //FileChangeWatcher.DisableAllChangeWatchers();
            try{
                FileRenamingEvent eargs = new FileRenamingEvent(oldName, newName, isDirectory);
                event_aggregator.Publish(eargs);
                
                if(eargs.Cancel)
                    return false;
                
                if(!eargs.OperationAlreadyDone){
                    try{
                        if(isDirectory && Directory.Exists(oldName)){
                            if(Directory.Exists(newName)){
                                MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                                return false;
                            }
                            Directory.Move(oldName, newName);
                        }else if(File.Exists(oldName)){
                            if(File.Exists(newName)){
                                MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                                return false;
                            }
                            File.Move(oldName, newName);
                        }
                    }
                    catch(Exception e){
                        if(isDirectory)
                            MessageService.ShowHandledException(e, "Can't rename directory " + oldName);
                        else
                            MessageService.ShowHandledException(e, "Can't rename file " + oldName);
                        
                        return false;
                    }
                }
                
                event_aggregator.Publish(new FileRenameEvent(oldName, newName, isDirectory));
                return true;
            }
            finally{
                //FileChangeWatcher.EnableAllChangeWatchers();
            }
        }

        /// <summary>
        /// Copies a file, raising the appropriate events. This method may show message boxes.
        /// </summary>
        public bool CopyFile(string oldName, string newName, bool isDirectory, bool overwrite)
        {
            if(FileUtility.IsEqualFileName(oldName, newName))
                return false;
            
            FileCopyingEvent eargs = new FileCopyingEvent(oldName, newName, isDirectory);
            event_aggregator.Publish(eargs);
            
            if(eargs.Cancel)
                return false;
            
            if(!eargs.OperationAlreadyDone){
                try{
                    if(isDirectory && Directory.Exists(oldName)){
                        if(!overwrite && Directory.Exists(newName)){
                            MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                            return false;
                        }
                        FileUtility.DeepCopy(oldName, newName, overwrite);
                    }else if(File.Exists(oldName)){
                        if(!overwrite && File.Exists(newName)){
                            MessageService.ShowMessage(StringParser.Parse("${res:Gui.ProjectBrowser.FileInUseError}"));
                            return false;
                        }
                        File.Copy(oldName, newName, overwrite);
                    }
                }
                catch(Exception e){
                    if(isDirectory)
                        MessageService.ShowHandledException(e, "Can't copy directory " + oldName);
                    else
                        MessageService.ShowHandledException(e, "Can't copy file " + oldName);
                    
                    return false;
                }
            }

            event_aggregator.Publish(new FileCopiedEvent(oldName, newName, isDirectory));
            return true;
        }
        #endregion

        #region FileCreated/Replaced
        /// <summary>
        /// Fires the event handlers for a file being created.
        /// </summary>
        /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
        /// <param name="isDirectory">Set to true if this is a directory</param>
        /// <returns>True if the operation can proceed, false if an event handler cancelled the operation.</returns>
        /*public bool FireFileReplacing(string fileName, bool isDirectory)
        {
            FileCancelEventArgs e = new FileCancelEventArgs(fileName, isDirectory);
            if(FileReplacing != null) {
                FileReplacing(this, e);
            }
            return !e.Cancel;
        }

        /// <summary>
        /// Fires the event handlers for a file being replaced.
        /// </summary>
        /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
        /// <param name="isDirectory">Set to true if this is a directory</param>
        public void FireFileReplaced(string fileName, bool isDirectory)
        {
            if(FileReplaced != null) {
                FileReplaced(this, new FileEventArgs(fileName, isDirectory));
            }
        }

        /// <summary>
        /// Fires the event handlers for a file being created.
        /// </summary>
        /// <param name="fileName">The name of the file being created. This should be a fully qualified path.</param>
        /// <param name="isDirectory">Set to true if this is a directory</param>
        public void FireFileCreated(string fileName, bool isDirectory)
        {
            if(FileCreated != null) {
                FileCreated(this, new FileEventArgs(fileName, isDirectory));
            }
        }

        public event EventHandler<FileEventArgs> FileCreated;
        public event EventHandler<FileCancelEventArgs> FileReplacing;
        public event EventHandler<FileEventArgs> FileReplaced;*/
        #endregion
    }
}
