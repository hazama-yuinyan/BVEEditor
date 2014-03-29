using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.Util;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    public class CustomKeyAction : KeyAction
    {
        public CustomKeyAction(Action<CompletionPopupViewModel> action, IEnumerable<Key> modifiers, Key key)
            : base(modifiers, key)
        {
            this.Action = action;
        }

        public CustomKeyAction()
        {}

        [TypeConverter(typeof(ActionConverter))]
        public Action<CompletionPopupViewModel> Action{get; set;}

        protected override void DoAct(CompletionPopupViewModel viewModel, KeyEventArgs args)
        {
            Action(viewModel);
        }

        public bool ShouldSwallowKeyPress{get; set;}

        protected override bool ShouldSwallow(CompletionPopupViewModel viewModel, KeyEventArgs args)
        {
            return ShouldSwallowKeyPress;
        }
    }

    public class ActionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(sourceType == typeof(string))
                return true;

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var val = value as string;

            return Delegate.CreateDelegate(typeof(Action<CompletionPopupViewModel>), typeof(CompletionPopupViewModel), val);
        }
    }
}
