using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Editor.CodeCompletion;
using BVEEditor.Misc;

namespace BVEEditor.Messages
{
    /// <summary>
    /// A message that tells receivers to scroll and show the specified item.
    /// </summary>
    public class ScrollIntoViewMessage : MessageBase
    {
        public ICompletionItem TargetItem{get; private set;}
        public ScrollIntoViewMessage(object sender, ICompletionItem target) : base(sender)
        {
            TargetItem = target;
        }
    }
}
