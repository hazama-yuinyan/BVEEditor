using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Misc;

namespace BVEEditor.Messages
{
    /// <summary>
    /// A message that tells receivers to show a popup.
    /// </summary>
    public class PopupShowMessage : MessageBase
    {
        public bool IsForce{get; private set;}
        public PopupShowMessage(object sender, bool force) : base(sender)
        {
            IsForce = force;
        }
    }
}
