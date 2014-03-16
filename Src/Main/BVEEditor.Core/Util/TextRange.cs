using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Util
{
    public class TextRange : ISegment
    {
        #region ISegment メンバー

        public int Offset{get; private set;}

        public int Length{get; private set;}

        public int EndOffset{
            get{
                return Offset + Length;
            }
        }

        #endregion

        public TextRange(int offset, int length)
        {
            Offset = offset;
            Length = length;
        }
    }
}
