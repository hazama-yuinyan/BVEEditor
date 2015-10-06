using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Messages
{
    /// <summary>
    /// A message that tells the receivers to invalidate the positions and recalculate them.
    /// </summary>
    public class InvalidatePositionMessage : MessageBase
    {
        public InvalidatePositionMessage(object sender) : base(sender)
        {
        }
    }
}
