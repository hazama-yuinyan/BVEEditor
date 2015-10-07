using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BVEEditor.Editor.CodeCompletion
{
    class ParamDescriptionElementTransformer : IDescriptionElementTransformer
    {
        #region IDescriptionElementTransformer メンバー

        public Inline[] Transform(List<KeyValuePair<string, string>> parameters, string value)
        {
            return new []{
                new Span(new Bold(new Run(parameters[0].Value))),
                new Span(new Run(value))
            };
        }

        #endregion
    }
}
