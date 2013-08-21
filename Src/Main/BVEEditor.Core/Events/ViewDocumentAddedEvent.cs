using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Workbench;

namespace BVEEditor.Events
{
    /// <summary>
    /// Event fired when a new <see cref="BVEEditor.Workbench.ViewDocumentViewModel"/> is added to the
    /// <see cref="BVEEditor.Workbench.ViewDocumentViewModel.ViewDocuments"/> collection.
    /// </summary>
    public class ViewDocumentAddedEvent
    {
        public ViewDocumentViewModel Document{get; private set;}
        public bool ShowOnAdded{get; private set;}
        /// <summary>
        /// Creates a new <see cref="BVEEditor.Events.ViewDocumentAddedEvent"/> instance.
        /// </summary>
        /// <param name="document">The ViewDocument being added</param>
        /// <param name="showOnAdded">If true, the ViewDocument will be shown when added</param>
        public ViewDocumentAddedEvent(ViewDocumentViewModel document, bool showOnAdded = true)
        {
            Document = document;
            ShowOnAdded = showOnAdded;
        }
    }
}
