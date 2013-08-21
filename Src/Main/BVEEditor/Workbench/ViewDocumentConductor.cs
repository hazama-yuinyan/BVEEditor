using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;

namespace BVEEditor.Workbench
{
    public class ViewDocumentConductor : Conductor<ViewDocumentViewModel>.Collection.OneActive
    {
        public void AddDocument(ViewDocumentViewModel viewDoc)
        {
            ActivateItem(viewDoc);
        }

        public void DeactivateDocument(ViewDocumentViewModel viewDoc)
        {
            DeactivateItem(viewDoc, false);
        }

        public void CloseDocument(ViewDocumentViewModel viewDoc)
        {
            DeactivateItem(viewDoc, true);
        }
    }
}
