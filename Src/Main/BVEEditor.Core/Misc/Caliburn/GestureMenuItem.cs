using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BVEEditor.Misc.Caliburn
{
    /// <summary>
    /// A <see cref="System.Windows.Controls.MenuItem"/> that automatically fills its InputGestureText and associates its Command property
    /// with an <see cref="Caliburn.Micro.ActionMessage"/> that calls the same name method as the MenuItem.
    /// </summary>
    public class GestureMenuItem : MenuItem
    {
        public static readonly DependencyProperty ModifiersProperty =
            DependencyProperty.Register("Modifiers", typeof(ModifierKeys), typeof(GestureMenuItem), new PropertyMetadata(default(ModifierKeys)));

        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(Key), typeof(GestureMenuItem), new PropertyMetadata(default(Key)));

        public ModifierKeys Modifiers{
            get{return (ModifierKeys)GetValue(ModifiersProperty);}
            set{SetValue(ModifiersProperty, value);}
        }

        public Key Key{
            get{return (Key)GetValue(KeyProperty);}
            set{SetValue(KeyProperty, value);}
        }

        static readonly IEnumerable<ModifierKeys> ModifierKeyValues = Enum.GetValues(typeof(ModifierKeys)).Cast<ModifierKeys>().Except(new []{ModifierKeys.None});
        static readonly IDictionary<ModifierKeys, string> Translations = new Dictionary<ModifierKeys, string>{
            {ModifierKeys.Control, "Ctrl"},
            {ModifierKeys.None, ""}
        };

        static string BuildInputGestureText(ModifierKeys modifiers, Key key)
        {
            var result = new StringBuilder();

            foreach(var val in ModifierKeyValues){
                if((modifiers & val) == val)
                    result.Append((Translations.ContainsKey(val) ? Translations[val] : val.ToString()) + " + ");
            }

            result.Append(key);
            
            return result.ToString();
        }

        public override void EndInit()
        {
            Interaction.GetTriggers(this).Add(ConstructTrigger());

            if(string.IsNullOrEmpty(InputGestureText))
                InputGestureText = BuildInputGestureText(Modifiers, Key);

            base.EndInit();
        }

        TriggerBase<FrameworkElement> ConstructTrigger()
        {
            var trigger = new InputBindingTrigger();

            trigger.GlobalInputBindings.Add(new KeyBinding{Modifiers = Modifiers, Key = Key});

            var command = new ActionMessageCommand{MethodName = Name};
            Command = command;
            trigger.Actions.Add(command);

            return trigger;
        }
    }
}
