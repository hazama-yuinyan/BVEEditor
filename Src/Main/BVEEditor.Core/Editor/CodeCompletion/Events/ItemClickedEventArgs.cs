using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BVEEditor.Editor.CodeCompletion.Events
{
    public class ItemClickedEventArgs
    {
        public MouseEventArgs Args{get; private set;}
        public ICompletionItem Item{get; private set;}

        public ItemClickedEventArgs(MouseEventArgs args, ICompletionItem item)
        {
            Args = args;
            Item = item;
        }
    }
}
