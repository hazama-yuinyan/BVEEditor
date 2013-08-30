using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BVEEditor.Options
{
    [ValueConversion(typeof(object), typeof(OptionPanelViewModel))]
    public class OptionPanelViewModelConverter : IValueConverter
    {
        #region IValueConverter メンバー

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var category_viewmodel = value as OptionCategoryViewModel;
            if(category_viewmodel != null)
                return category_viewmodel.Children[0];

            var option_viewmodel = value as OptionPanelViewModel;
            if(option_viewmodel != null)
                return option_viewmodel;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
