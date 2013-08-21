using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Events;
using BVEEditor.Result;
using BVEEditor.Strategies;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Views
{
    /// <summary>
    /// The view model for the main menu.
    /// </summary>
    public class MainMenuViewModel : PropertyChangedBase, IHandle<ActiveViewDocumentChangedEvent>
    {
        const string MainMenuPath = "/BVEEditor/Workbench/MainMenu";

        readonly IResultFactory result_factory;
        readonly IFileDialogStrategies file_strategies;
        readonly IEventAggregator event_aggregator;
        readonly IDisplayBindingService display_binding;

        ViewDocumentViewModel active_doc;
        ViewDocumentViewModel ActiveDocument{
            get{return active_doc;}
            set{
                active_doc = value;
                NotifyOfPropertyChange(() => CanSaveDocument);
                NotifyOfPropertyChange(() => IsPathSet);
                NotifyOfPropertyChange(() => CanQuickSaveDocument);
            }
        }

        public MainMenuViewModel(IResultFactory resultFactory, IFileDialogStrategies fileStrategies, IEventAggregator eventAggregator,
            IDisplayBindingService displayBindingService)
        {
            eventAggregator.Subscribe(this);

            result_factory = resultFactory;
            file_strategies = fileStrategies;
            event_aggregator = eventAggregator;
            display_binding = displayBindingService;
        }

        public void NewDocument()
        {
            CreateViewDocumentViewModel(null);
        }

        public IEnumerable<IResult> OpenDocument()
        {
            return file_strategies.Open(CreateViewDocumentViewModel);
        }

        internal void CreateViewDocumentViewModel(string filePath)
        {
            FileName filename = FileName.Create(filePath);
            var new_doc = display_binding.GetBindingPerFileName(filename)
                .CreateViewModelForFile(filename);
            
            if(!string.IsNullOrEmpty(filePath))
                new_doc.Load(filePath);

            event_aggregator.Publish(new ViewDocumentAddedEvent(new_doc));
        }

        public IEnumerable<IResult> SaveDocument()
        {
            return SaveDocument(active_doc);
        }

        public IEnumerable<IResult> SaveDocument(ViewDocumentViewModel viewDoc)
        {
            return file_strategies.SaveAs(viewDoc, false, path => SaveInternel(viewDoc, path));
        }

        void SaveInternel(ViewDocumentViewModel viewDoc)
        {
            SaveInternel(viewDoc, viewDoc.FileName);
        }

        void SaveInternel(ViewDocumentViewModel viewDoc, string path)
        {
            viewDoc.FilePath = FileName.Create(path);
            viewDoc.Save(path);
        }

        public IEnumerable<IResult> QuickSaveDocument()
        {
            if(IsPathSet){
                SaveInternel(active_doc);
                return null;
            }
            
            return SaveDocument();
        }

        public bool CanQuickSaveDocument{
            get{return CanSaveDocument && active_doc.IsDirty;}
        }

        public bool IsPathSet{
            get{return !active_doc.IsUntitled;}
        }

        public bool CanSaveDocument{
            get{return active_doc != null;}
        }

        public IEnumerable<IResult> Close()
        {
            yield return result_factory.Close();
        }

        public IEnumerable<IResult> ShowAboutDialog()
        {
            yield return result_factory.ShowDialogResult<AboutDialogViewModel>();
        }

        #region IHandle<ActiveViewDocumentChangedEvent> メンバー

        public void Handle(ActiveViewDocumentChangedEvent message)
        {
            if(ActiveDocument != null)
                ActiveDocument.PropertyChanged -= ViewDocumentPropertyChanged;

            ActiveDocument = message.ViewDocument;
            ActiveDocument.PropertyChanged += ViewDocumentPropertyChanged;
        }

        #endregion

        void ViewDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "is_dirty"){
                NotifyOfPropertyChange(() => CanQuickSaveDocument);
            }
        }
    }
}
