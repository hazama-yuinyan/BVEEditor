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

    public class ErrorDocumentDisplayBinding : IDisplayBinding
    {
        Func<ErrorDocumentViewModel> error_doc_factory;

        public ErrorDocumentDisplayBinding(Func<ErrorDocumentViewModel> errorDocFactory)
        {
            error_doc_factory = errorDocFactory;
        }

        #region IDisplayBinding メンバー

        public bool IsPreferredBindingForFile(ICSharpCode.Core.FileName fileName)
        {
            return true;
        }

        public bool CanHandle(ICSharpCode.Core.FileName fileName)
        {
            return true;
        }

        public double AutoDetectFileContent(ICSharpCode.Core.FileName fileName, System.IO.Stream fileContent, string detectedMimeType)
        {
            return 0.0;
        }

        public ViewDocumentViewModel CreateViewModelForFile(ICSharpCode.Core.FileName path)
        {
            return error_doc_factory();
        }

        #endregion
    }
}
