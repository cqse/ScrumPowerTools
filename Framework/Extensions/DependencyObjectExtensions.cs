namespace ScrumPowerTools.Framework.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;

    public static class DependencyObjectExtensions
    {
        private static readonly PropertyInfo InheritanceContextProp = typeof(DependencyObject).GetProperty("InheritanceContext", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Retrieves the parent hierarchy of a dependency object according to http://stackoverflow.com/questions/20985005/get-parent-for-gridviewcolumn
        /// </summary>
        public static IEnumerable<DependencyObject> GetParents(this DependencyObject child)
        {
            while (child != null)
            {
                var parent = LogicalTreeHelper.GetParent(child);
                if (parent == null)
                {
                    if (child is FrameworkElement)
                    {
                        parent = VisualTreeHelper.GetParent(child);
                    }
                    if (parent == null && child is ContentElement)
                    {
                        parent = ContentOperations.GetParent((ContentElement)child);
                    }
                    if (parent == null)
                    {
                        parent = InheritanceContextProp.GetValue(child, null) as DependencyObject;
                    }
                }
                child = parent;
                yield return parent;
            }
        }
    }
}
