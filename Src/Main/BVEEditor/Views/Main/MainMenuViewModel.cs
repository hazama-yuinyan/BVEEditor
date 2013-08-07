using System;
using System.Collections.Generic;
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
    public class MainMenuViewModel : PropertyChangedBase, IHandle<ActiveViewContentChangedEvent>
    {
        readonly IResultFactory result_factory;
        readonly IFileDialogStrategies file_strategies;
        readonly IEventAggregator event_aggregator;
        readonly Func<ErrorDocumentViewModel> error_doc_factory;

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
            Func<ErrorDocumentViewModel> errorDocumentFactory)
        {
            eventAggregator.Subscribe(this);

            result_factory = resultFactory;
            file_strategies = fileStrategies;
            event_aggregator = eventAggregator;
            error_doc_factory = errorDocumentFactory;
        }

        public void NewDocument()
        {
            CreateViewDocumentViewModel(null);
        }

        public IEnumerable<IResult> OpenDocument()
        {
            return file_strategies.Open(CreateViewDocumentViewModel);
        }

        void CreateViewDocumentViewModel(string filePath)
        {
            var new_doc = error_doc_factory()
                .Configure(FileName.Create("Untitled"));
            
            //if(!string.IsNullOrEmpty(filePath))
            //    new

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
            viewDoc.File.FileName = FileName.Create(path);

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
            get{return CanSaveDocument;}
        }

        public bool IsPathSet{
            get{return !string.IsNullOrEmpty(active_doc.FileName);}
        }

        public bool CanSaveDocument{
            get{return active_doc != null;}
        }

        public IEnumerable<IResult> Close()
        {
            yield return result_factory.Close();
        }

        #region IHandle<ActiveViewContentChangedEvent> メンバー

        public void Handle(ActiveViewContentChangedEvent message)
        {
            active_doc = message.Content.ViewDocument;
        }

        #endregion
    }
}
