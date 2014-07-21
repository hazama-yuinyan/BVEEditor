using System;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Util
{
    /// <summary>
    /// Represents a range of text simply by an offset and a length.
    /// </summary>
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

    /// <summary>
    /// A text range segment that can lazily evaluate its content.
    /// </summary>
    public class LazyEvaluationTextRange : ISegment
    {
        Lazy<int> offset_promise, length_promise;

        #region ISegment メンバー

        public int Offset{
            get{return offset_promise.Value;}
        }

        public int Length{
            get{return length_promise.Value;}
        }

        public int EndOffset{
            get{return Offset + Length;}
        }

        #endregion

        public LazyEvaluationTextRange(Lazy<int> offsetPromise, Lazy<int> lengthPromise)
        {
            offset_promise = offsetPromise;
            length_promise = lengthPromise;
        }

        public LazyEvaluationTextRange(int offset, Lazy<int> lengthPromise)
        {
            offset_promise = new Lazy<int>(() => offset);
            length_promise = lengthPromise;
        }

        public LazyEvaluationTextRange(Lazy<int> offsetPromise, int length)
        {
            offset_promise = offsetPromise;
            length_promise = new Lazy<int>(() => length);
        }
    }
}
