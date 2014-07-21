using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// The default implementation of <see cref="BVEEditor.Editor.CodeCompltion.ICodeCompletionBinding"/>
    /// that does nothing.
    /// </summary>
    public class DefaultCodeCompletionBinding : ICodeCompletionBinding
    {
        static readonly ICodeCompletionBinding instance = new DefaultCodeCompletionBinding();

        public static ICodeCompletionBinding Instance{
            get{return instance;}
        }

        #region ICodeCompletionBinding メンバー

        public CodeCompletionKeyPressResult HandleKeyPress(ICompletionHandler completionHandler, char ch)
        {
            return CodeCompletionKeyPressResult.None;
        }

        public bool ShouldMarkEndOfExpression(ITextEditor editor, int startOffset)
        {
            return false;
        }

        public bool ShouldMarkEndOfExpression(ITextEditor editor, char ch, int startOffset)
        {
            return false;
        }

        public CodeCompletionKeyPressResult HandleCtrlSpace(ICompletionHandler completionHandler)
        {
            return CodeCompletionKeyPressResult.None;
        }

        #endregion
    }
}
