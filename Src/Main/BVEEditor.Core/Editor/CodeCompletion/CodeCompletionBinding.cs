/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 06/30/2013
 * Time: 22:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// Interface that gives backend bindings the possibility to control what characters and
    /// keywords invoke code completion.
    /// </summary>
    public interface ICodeCompletionBinding
    {
        /// <summary>
        /// This method is called when typing a character in the editor, immediately before
        /// the character is inserted into the document.
        /// </summary>
        /// <param name="editor">The editor</param>
        /// <param name="ch">The character being inserted.</param>
        /// <returns>Returns whether the completion binding has shown code completion.</returns>
        CodeCompletionKeyPressResult HandleKeyPress(ICompletionHandler completionHandler, char ch);

        /// <summary>
        /// Determines whether the cursor is at the end of an expression.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <param name="startOffset">The offset at which the expression being edited starts.</param>
        /// <returns>true, if it recognizes that the cursor is currently out of the scope of an expression.</returns>
        bool ShouldMarkEndOfExpression(ITextEditor editor, int startOffset);

        /// <summary>
        /// Determines whether the user finishes typing in an expression.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <param name="ch">The character being inserted.</param>
        /// <param name="startOffset">The offset at which the expression being edited starts.</param>
        /// <returns>true, if it recognizes that the user finished typing in an expression; otherwise, false.</returns>
        bool ShouldMarkEndOfExpression(ITextEditor editor, char ch, int startOffset);

        /// <summary>
        /// This method is called when a character is typed in.
        /// </summary>
        /// <param name="editor">The editor</param>
        /// <returns>Returns whether the completion binding has shown code completion.</returns>
        CodeCompletionKeyPressResult HandleCtrlSpace(ICompletionHandler completionHandler);
    }

    /// <summary>
    /// The result of <see cref="ICodeCompletionBinding.HandleKeyPress"/>.
    /// </summary>
    public enum CodeCompletionKeyPressResult
    {
        /// <summary>
        /// The binding did not run code completion. The pressed key will be handled normally.
        /// </summary>
        None,
        /// <summary>
        /// The binding handled code completion, the pressed key will be handled normally.
        /// The pressed key will not be included in the completion expression, but will be
        /// in front of it (this is usually used when the key is '.').
        /// </summary>
        Completed,
        /// <summary>
        /// The binding handled code completion, the pressed key will be handled normally.
        /// The pressed key will be included in the completion expression.
        /// This is used when starting to type any character starts code completion.
        /// </summary>
        CompletedIncludeKeyInCompletion,
        /// <summary>
        /// The binding handled code completion, and the key will not be handled by the text editor.
        /// </summary>
        EatKey
    }

    /// <summary>
    /// Creates code completion bindings that manage code completion for one language.
    /// </summary>
    /// <attribute name="class" use="required">
    /// Name of the ICodeCompletionBinding class (normally deriving from DefaultCodeCompletionBinding).
    /// </attribute>
    /// <attribute name="extensions" use="optional">
    /// List of semicolon-separated entries of the file extensions handled by the binding.
    /// If no extensions attribute is specified, the binding is activated in all files.
    /// </attribute>
    /// <usage>Only in /BVEEditor/ViewContent/TextEditor/CodeCompletion</usage>
    /// <returns>
    /// The ICodeCompletionBinding class specified with the 'class' attribute, or a
    /// wrapper that lazy-loads the actual class when it is used in a file with the specified
    /// extension.
    /// </returns>
    public class CodeCompletionBindingDoozer : IDoozer
    {
        public bool HandleConditions{
            get{
                return false;
            }
        }

        public object BuildItem(BuildItemArgs args)
        {
            string ext = args.Codon["extensions"];
            if(ext != null && ext.Length > 0)
                return new CodeCompletionBindingDescriptor(args.Codon, ext.Split(';'));
            else
                return args.AddIn.CreateObject(args.Codon["class"]);
        }
    }

    /// <summary>
    /// Descriptor of <see cref="BVEEditor.Editor.CodeCompletion.ICodeCompletionBinding"/>.
    /// </summary>
    public sealed class CodeCompletionBindingDescriptor
    {
        Codon codon;
        string[] extensions;
        ICodeCompletionBinding binding;

        public ICodeCompletionBinding Binding{
            get{
                if(binding == null)
                    binding = (ICodeCompletionBinding)codon.AddIn.CreateObject(codon.Properties["class"]);

                return binding;
            }
        }

        public CodeCompletionBindingDescriptor(Codon codon, string[] extensions)
        {
            this.codon = codon;
            this.extensions = extensions;
        }

        /// <summary>
        /// Determines whether the <see cref="BVEEditor.Editor.CodeCompletion.ICodeCompletionBinding"/> can handle
        /// the specific type of files.
        /// </summary>
        /// <param name="editor"></param>
        /// <returns>true, if it can handle the content of the editor; otherwise, false.</returns>
        public bool CanHandle(ITextEditor editor)
        {
            string ext = Path.GetExtension(editor.FileName);
            foreach(var extension in extensions){
                if(ext.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
