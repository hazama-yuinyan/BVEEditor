using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Workbench;
using Caliburn.Micro;

namespace BVEEditor.Strategies
{
    /// <summary>
    /// Interface for showing file-related dialogs.
    /// </summary>
    public interface IFileDialogStrategies
    {
        IEnumerable<IResult> SaveAs(ViewDocumentViewModel document, bool quickSave, Action<string> fileSelected);
        IEnumerable<IResult> Open(Action<string> fileSelected);
    }
}
