using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Messages
{
    /// <summary>
    /// A message that tells receivers to hide the popup.
    /// </summary>
    public class PopupHideMessage : MessageBase
    {
        public bool FocusingParent{get; private set;}
        public PopupHideMessage(object sender, bool willFocus) : base(sender)
        {
            FocusingParent = willFocus;
        }
    }
}
