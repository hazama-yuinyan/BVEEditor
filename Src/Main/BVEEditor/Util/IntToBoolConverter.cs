using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BVEEditor.Util
{
    /// <summary>
    /// Converts int value to bool.
    /// Returns false if the value is 0, otherwise true.
    /// </summary>
    public class IntToBoolConverter : IValueConverter
    {
        #region IValueConverter メンバー

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is int){
                if((int)value == 0)
                    return false;
                else
                    return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
