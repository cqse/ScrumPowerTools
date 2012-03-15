using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScrumPowerTools.Framework.Presentation
{
    public class ListViewItemClickBehavior
    {
        public static DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached("DoubleClick",
            typeof(ICommand),
            typeof(ListViewItemClickBehavior),
            new FrameworkPropertyMetadata(null, DoubleClickChanged));

        public static void SetDoubleClick(DependencyObject target, ICommand value)
        {
            target.SetValue(DoubleClickCommandProperty, value);
        }

        public static ICommand GetDoubleClick(DependencyObject target)
        {
            return (ICommand)target.GetValue(DoubleClickCommandProperty);
        }

        private static void DoubleClickChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ListViewItem element = target as ListViewItem;

            if (element != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    element.MouseDoubleClick += MouseDoubleClicked;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    element.MouseDoubleClick -= MouseDoubleClicked;
                }
            }
        }

        private static void MouseDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            var command = (ICommand)element.GetValue(DoubleClickCommandProperty);
            object parameter = ((ListViewItem)element).Content;

            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}