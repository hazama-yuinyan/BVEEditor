using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using ICSharpCode.AvalonEdit.Editing;

namespace BVEEditor.Editor
{
    public class EditorAdaptorConverter : MarkupExtension, IValueConverter
    {
        public Dictionary<Type, Func<object, EditorAdaptorBase>> factories;

        public EditorAdaptorConverter()
        {
            factories = new Dictionary<Type, Func<object, EditorAdaptorBase>>(){
                { typeof(TextArea), val => new AvalonEditorAdaptor((TextArea)val) }
            };
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
                return null;

            Type val_type = value.GetType();

            if(!factories.ContainsKey(val_type))
                throw new NotSupportedException("Conversion from " + val_type + " to EditorAdapterBase is currently not supported.");

            return factories[val_type](value);
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
