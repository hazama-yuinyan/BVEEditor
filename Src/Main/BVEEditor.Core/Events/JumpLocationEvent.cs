using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Events
{
    /// <summary>
    /// An event that requests text editor to jump to a certain position.
    /// </summary>
    public class JumpLocationEvent
    {
        public FileName FileName{get; private set;}
        public int LineNumber{get; private set;}
        public int ColumnNumber{get; private set;}

        public JumpLocationEvent(FileName fileName, int lineNum, int columnNum)
        {
            FileName = fileName;
            LineNumber = lineNum;
            ColumnNumber = columnNum;
        }
    }
}
