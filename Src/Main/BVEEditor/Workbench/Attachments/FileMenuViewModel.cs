using BVEEditor.Events;
using BVEEditor.Result;
using BVEEditor.Strategies;
using Caliburn.Micro;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// The view model for File menu.
    /// </summary>
    public class FileMenuViewModel : MenuViewModelBase, IHandle<ActiveViewDocumentChangedEvent>
    {
        readonly IEventAggregator event_aggregator;
        readonly IResultFactory result_factory;
        readonly IFileDialogStrategies file_strategies;

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

        public RecentOpenViewModel RecentOpen{
            get; private set;
        }

        public FileMenuViewModel(IEventAggregator eventAggregator, IResultFactory resultFactory, RecentOpenViewModel recentOpen,
            IFileDialogStrategies fileStrategies) : base(LogManager.GetLog(typeof(FileMenuViewModel)), "File")
        {
            eventAggregator.Subscribe(this);

            event_aggregator = eventAggregator;
            result_factory = resultFactory;
            RecentOpen = recentOpen;
            file_strategies = fileStrategies;
        }

        public void NewDocument()
        {
            Workbench.CreateViewDocumentViewModel(null);
        }

        public IEnumerable<IResult> OpenDocument()
        {
            return file_strategies.Open(Workbench.CreateViewDocumentViewModel);
        }

        public void OpenDocument(RoutedEventArgs e)
        {
            var original_source = e.OriginalSource as FrameworkElement;
            if(original_source == null)
                return;

            var file_name = original_source.DataContext as FileName;
            if(file_name != null)
                Workbench.CreateViewDocumentViewModel(file_name);
        }

        public IEnumerable<IResult> SaveDocument()
        {
            return Workbench.SaveDocument(active_doc, false);
        }

        public IEnumerable<IResult> QuickSaveDocument()
        {
            return Workbench.SaveDocument(active_doc, IsPathSet);
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
            
            if(ActiveDocument != null)
                ActiveDocument.PropertyChanged += ViewDocumentPropertyChanged;
        }

        #endregion

        void ViewDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsDirty"){
                NotifyOfPropertyChange(() => CanQuickSaveDocument);
            }
        }
    }
}
