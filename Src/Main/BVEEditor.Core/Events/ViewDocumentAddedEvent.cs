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
        public ViewDocumentAddedEvent(ViewDocumentViewModel document)
        {
            Document = document;
        }
    }
}
