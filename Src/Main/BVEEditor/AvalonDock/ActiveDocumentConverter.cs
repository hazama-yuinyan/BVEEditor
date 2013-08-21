using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BVEEditor.Workbench;

namespace BVEEditor.AvalonDock
{
    class ActiveDocumentConverter : IValueConverter
    {
        #region IValueConverter メンバー

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is ViewDocumentViewModel)
                return value;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is ViewDocumentViewModel)
                return value;

            return Binding.DoNothing;
        }

        #endregion
    }
}
