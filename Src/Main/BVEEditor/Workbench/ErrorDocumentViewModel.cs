using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Result;
using BVEEditor.Services;
using Caliburn.Micro;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// An ErrorView presents the user with the error message indicating that some operation failed.
    /// </summary>
    public class ErrorDocumentViewModel : ViewDocumentViewModel
    {
        public ErrorDocumentViewModel(IFileSystem fileSystem, IEventAggregator eventAggregator, IResultFactory resultFactory)
            : base(fileSystem, eventAggregator, resultFactory)
        {}
    }
}
