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

namespace BVEEditor.Views.Main
{
    public class ToolBarViewModel : PropertyChangedBase
    {
        const string ToolBarPath = "/BVEEditor/Workbench/ToolBar";

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

        // This property cann't use constructor injection
        // because doing so creates a cyclic dependency.
        public IWorkbench Workbench{private get; set;}

        public ToolBarViewModel(IResultFactory resultFactory, IFileDialogStrategies fileStrategies, IEventAggregator eventAggregator,
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
            return Workbench.SaveDocument(active_doc, false);
        }

        public IEnumerable<IResult> QuickSaveDocument()
        {
            if(IsPathSet){
                Workbench.SaveDocument(active_doc, true);
                return null;
            }
            
            return Workbench.SaveDocument(active_doc, false);
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
