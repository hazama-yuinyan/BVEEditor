using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BVEEditor.CodeCompletion;

namespace BVEEditor.Editor.CodeCompletion.Actions
{
    public abstract class KeyAction : IEventObserver<IPopupEvent, ICancellablePopupEvent, CompletionPopupViewModel>
    {
        protected KeyAction(IEnumerable<Key> modifiers, Key key)
        {
            this.Modifiers = modifiers;
            this.Key = key;
            this.IsTargetSource = x => true;
        }

        protected KeyAction()
        {
            this.IsTargetSource = x => true;
            this.Modifiers = new Caliburn.Micro.BindableCollection<Key>();
        }

        bool IsTriggered(IPopupEvent @event)
        {
            if(@event.Type != EventType.KeyPress)
                return false;

            var key_args = @event.EventArgs as KeyEventArgs;

            return IsTargetSource(@event.Source) && Key == key_args.Key && Modifiers.All(key_args.KeyboardDevice.IsKeyDown);
        }

        protected abstract void DoAct(CompletionPopupViewModel viewModel, KeyEventArgs args);

        protected virtual bool ShouldSwallow(CompletionPopupViewModel viewModel, KeyEventArgs args)
        {
            return false;
        }

        protected virtual bool IsTriggeredAddon(IPopupEvent @event, CompletionPopupViewModel viewModel)
        {
            return true;
        }

        public Key Key{get; set;}

        public Predicate<EventSource> IsTargetSource{get; set;}

        [TypeConverter(typeof(KeyActionListConverter))]
        public IEnumerable<Key> Modifiers { get; set; }

        public void Preview(IEnumerable<IPopupEvent> events, ICancellablePopupEvent current, CompletionPopupViewModel viewModel)
        {
            if(!IsTriggered(current) || !IsTriggeredAddon(current, viewModel))
                return;

            var keyArgs = current.EventArgs as KeyEventArgs;

            DoAct(viewModel, keyArgs);

            if(ShouldSwallow(viewModel, keyArgs))
                current.Cancel();
        }

        public void Handle(IEnumerable<IPopupEvent> events, CompletionPopupViewModel viewModel)
        {
        }
    }

    public class KeyActionListConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if(sourceType == typeof(string))
                return true;

            return false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
                                           object value)
        {
            var val = value as string;

            if(val == null)
                throw new InvalidOperationException("Can only convert from strings.");

            var values = val.Split(' ');

            return values.Select(modifier => (Key)Enum.Parse(typeof(Key), modifier));
        }
    }
}
