using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Messages
{
    public class InvalidatePositionMessage : MessageBase
    {
        public InvalidatePositionMessage(object sender) : base(sender)
        {
        }
    }
}
