using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Editor
{
    /// <summary>
    /// Interface for <see cref="ICSharpCode.AvalonEdit.IHighlighter"/> factories.
    /// </summary>
    public interface IHighlighterFactory
    {
        IHighlighter CreateHighlighter(IDocument document);
    }
}
