﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BVEEditor.Workbench
{
    public class AutobinderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template{
            get; set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Template;
        }
    }
}
