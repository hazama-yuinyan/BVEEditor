using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace BVEEditor.Editor.CodeCompletion
{
    interface IDescriptionElementTransformer
    {
        Inline Transform(List<KeyValuePair<string, string>> parameters, string value);
    }
}
