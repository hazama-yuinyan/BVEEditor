using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Workbench;

namespace BVEEditor.Events
{
    public class ActiveViewDocumentChangedEvent
    {
        public ViewDocumentViewModel ViewDocument{get; private set;}

        public ActiveViewDocumentChangedEvent(ViewDocumentViewModel viewDoc)
        {
            ViewDocument = viewDoc;
        }
    }
}
