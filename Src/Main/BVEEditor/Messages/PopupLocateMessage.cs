using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Messages
{
    /// <summary>
    /// A message that tells the recievers to locate the popup.
    /// </summary>
    public class PopupLocateMessage : MessageBase
    {
        public object Target{get; private set;}
        public PopupLocateMessage(object sender, object target) : base(sender)
        {
            Target = target;
        }
    }
}
