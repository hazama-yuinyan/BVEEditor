using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor
{
    /// <summary>
    /// Denotes the derived class that it is a text editor that is capable of detecting the text encoding type
    /// of its content.
    /// </summary>
    public interface ITextEncodingProvider
    {
        Encoding TextEncoding{get;}
    }
}
