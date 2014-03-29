using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using ICSharpCode.AvalonEdit.Editing;

namespace BVEEditor.Editor.CodeCompletion
{
    public class EditorAdaptorConverter : MarkupExtension, IValueConverter
    {
        public EditorAdaptorConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
                return null;

            var res = value as ITextEditor;
            if(res == null)
                throw new InvalidOperationException("Could not cast the value to ITextEditor!");

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
