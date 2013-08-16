using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.Core;

namespace BVEEditor.Result
{
    /// <summary>
    /// An IResult implementation that shows a message box.
    /// </summary>
    public class MessageBoxResult : Result
    {
        readonly string text;
        readonly MessageBoxButton buttons;
        readonly string caption;

        public System.Windows.MessageBoxResult Result{get; private set;}

        public MessageBoxResult(string caption, string text) : this(caption, text, MessageBoxButton.OK)
        {}
        /// <summary>
        /// Constructs a new <see cref="BVEEditor.Result.MessageBoxResult"/> instance.
        /// Note that <code>caption</code> and <code>text</code> will be put into <see cref="ICSharpCode.Core.StringParser.Parse"/>
        /// method.
        /// </summary>
        public MessageBoxResult(string caption, string text, MessageBoxButton buttons)
        {
            this.caption = caption;
            this.buttons = buttons;
            this.text = text;
        }

        public override void Execute(Caliburn.Micro.ActionExecutionContext context)
        {
            Result = MessageBox.Show(StringParser.Parse(text), StringParser.Parse(caption), buttons);
            base.Execute(context);
        }
    }
}
