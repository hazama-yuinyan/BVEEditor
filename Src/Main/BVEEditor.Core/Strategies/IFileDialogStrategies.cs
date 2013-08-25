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
        /// <summary>
        /// Saves the document to the specified file.
        /// </summary>
        /// <param name="document">The ViewDocument to be saved</param>
        /// <param name="doQuickSave">Flag to indicate it should show a save-as dialog</param>
        /// <param name="fileSelected">Delegate</param>
        IEnumerable<IResult> Save(ViewDocumentViewModel document, bool doQuickSave, System.Action<string> fileSelected);

        /// <summary>
        /// Opens a document from a file.
        /// </summary>
        /// <param name="fileSelected"></param>
        IEnumerable<IResult> Open(System.Action<string> fileSelected);
    }
}
