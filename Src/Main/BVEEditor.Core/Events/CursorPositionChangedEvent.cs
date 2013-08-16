using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Events
{
    public class CaretPositionChangedEvent
    {
        public int Line{get; private set;}
        public int Column{get; private set;}
        public int CharNumber{get; private set;}

        public CaretPositionChangedEvent(int lineNum, int colNum, int charNum)
        {
            Line = lineNum;
            Column = colNum;
            CharNumber = charNum;
        }
    }
}
