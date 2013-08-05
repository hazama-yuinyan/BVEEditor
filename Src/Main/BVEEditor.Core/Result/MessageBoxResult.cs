using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BVEEditor.Result
{
    public class MessageBoxResult : Result
    {
        readonly string text;
        readonly MessageBoxButton buttons;
        readonly string caption;

        public System.Windows.MessageBoxResult Result{get; private set;}

        public MessageBoxResult(string caption, string text) : this(caption, text, MessageBoxButton.OK)
        {}

        public MessageBoxResult(string caption, string text, MessageBoxButton buttons)
        {
            this.caption = caption;
            this.buttons = buttons;
            this.text = text;
        }

        public override void Execute(Caliburn.Micro.ActionExecutionContext context)
        {
            Result = MessageBox.Show(text, caption, buttons);
            base.Execute(context);
        }
    }
}
