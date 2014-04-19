using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BVEEditor.CodeCompletion.Controls
{
    public class EventArgs<T, K> : EventArgs
    {
        public T Arg1{get; private set;}
        public K Arg2{get; private set;}

        public EventArgs(T arg1, K arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }

    /// <summary>
    /// EvenListBox.xaml の相互作用ロジック
    /// </summary>
    public partial class EvenListBox : ListBox
    {
        public EvenListBox()
        {
            Loaded += (sender, args) => {
                this.item_height = CalculateItemHeight();
                UpdateHeight();
            };
            PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;

            if(!(source is TextBlock) || source == this || this == source.TemplatedParent)
                return;

            OnItemClicked(source.DataContext, e);
        }

        void OnItemClicked(object dataContext, MouseButtonEventArgs e)
        {
            if(ItemClicked != null)
                ItemClicked(this, new EventArgs<object, MouseButtonEventArgs>(dataContext, e));
        }

        public event EventHandler<EventArgs<object, MouseButtonEventArgs>> ItemClicked;

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            UpdateHeight();
            base.OnItemsChanged(e);
        }

        void UpdateHeight()
        {
            this.Height = CalculateHeight();
        }

        double item_height;

        public static readonly DependencyProperty DisplayedRowsProperty =
            DependencyProperty.Register("DisplayedRows", typeof(int), typeof(EvenListBox), new PropertyMetadata(default(int)));

        public int DisplayedRows{
            get{return (int)GetValue(DisplayedRowsProperty);}
            set{SetValue(DisplayedRowsProperty, value);}
        }

        double CalculateItemHeight()
        {
            if(Items.Count <= 0)
                return double.NaN;

            var container = ItemContainerGenerator.ContainerFromIndex(0) as Control;

            if(container == null)
                return double.NaN;

            container.Measure(new Size(200, 200));

            double item_height;

            item_height = Math.Abs(container.ActualHeight) < 0.01 ? container.DesiredSize.Height : container.ActualHeight;
            item_height = item_height + container.Margin.Bottom + container.Margin.Top + container.Padding.Bottom + container.Padding.Top;

            return item_height;
        }

        double CalculateHeight()
        {
            if(Items.Count <= DisplayedRows)
                return double.NaN;

            return item_height * DisplayedRows + 5;
        }
    }
}
