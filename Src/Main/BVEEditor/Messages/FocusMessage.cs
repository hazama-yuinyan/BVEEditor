using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Messages
{
    /// <summary>
    /// A message that tells the receivers to focus on elements corresponding to the senders.
    /// </summary>
    public class FocusMessage : MessageBase
    {
        public FocusMessage(object sender) : base(sender)
        {
        }
    }
}
