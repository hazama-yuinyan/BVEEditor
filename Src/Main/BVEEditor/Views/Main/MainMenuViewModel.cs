﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Events;
using BVEEditor.Options;
using BVEEditor.Result;
using BVEEditor.Strategies;
using BVEEditor.Views.Help;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Views.Main
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

        public MainMenuViewModel(IResultFactory resultFactory, IFileDialogStrategies fileStrategies, IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);

            result_factory = resultFactory;
            file_strategies = fileStrategies;
            event_aggregator = eventAggregator;
        }

        public void NewDocument()
        {
            Workbench.CreateViewDocumentViewModel(null);
        }

        public IEnumerable<IResult> OpenDocument()
        {
            return file_strategies.Open(Workbench.CreateViewDocumentViewModel);
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

        public IEnumerable<IResult> ShowAboutDialog()
        {
            yield return result_factory.ShowDialogResult<AboutDialogViewModel>();
        }

        public IEnumerable<IResult> ShowOptionsDialog()
        {
            yield return result_factory.ShowDialogResult<OptionsViewModel>();
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
