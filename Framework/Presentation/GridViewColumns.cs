using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ScrumPowerTools.Framework.Presentation
{
    public static class GridViewColumns
    {
        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static object GetColumnsSource(DependencyObject obj)
        {
            return obj.GetValue(ColumnsSourceProperty);
        }

        public static void SetColumnsSource(DependencyObject obj, object value)
        {
            obj.SetValue(ColumnsSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnsSource.
        // This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty ColumnsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ColumnsSource",
                typeof(object),
                typeof(GridViewColumns),
                new UIPropertyMetadata(null, ColumnsSourceChanged));


        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static string GetHeaderTextMember(DependencyObject obj)
        {
            return (string)obj.GetValue(HeaderTextMemberProperty);
        }

        public static void SetHeaderTextMember(DependencyObject obj, string value)
        {
            obj.SetValue(HeaderTextMemberProperty, value);
        }

        // Using a DependencyProperty as the backing store for HeaderTextMember.
        //This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty HeaderTextMemberProperty =
            DependencyProperty.RegisterAttached(
                "HeaderTextMember",
                typeof(string),
                typeof(GridViewColumns),
                new UIPropertyMetadata(null));


        [AttachedPropertyBrowsableForType(typeof(GridView))]
        public static string GetDisplayMemberMember(DependencyObject obj)
        {
            return (string)obj.GetValue(DisplayMemberMemberProperty);
        }

        public static void SetDisplayMemberMember(DependencyObject obj, string value)
        {
            obj.SetValue(DisplayMemberMemberProperty, value);
        }

        // Using a DependencyProperty as the backing store for DisplayMember.
        //This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty DisplayMemberMemberProperty =
            DependencyProperty.RegisterAttached(
                "DisplayMemberMember",
                typeof(string),
                typeof(GridViewColumns),
                new UIPropertyMetadata(null));

        private static void ColumnsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var gridView = obj as GridView;
            if (gridView != null)
            {
                gridView.Columns.Clear();

                if (e.OldValue != null)
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(e.OldValue);
                    if (view != null)
                    {
                        RemoveHandlers(gridView, view);
                    }
                }

                if (e.NewValue != null)
                {
                    ICollectionView view = CollectionViewSource.GetDefaultView(e.NewValue);
                    if (view != null)
                    {
                        AddHandlers(gridView, view);
                        CreateColumns(gridView, view);
                    }
                }
            }
        }

        private static readonly IDictionary<ICollectionView, List<GridView>> GridViewsByColumnsSource =
            new Dictionary<ICollectionView, List<GridView>>();

        private static List<GridView> GetGridViewsForColumnSource(ICollectionView columnSource)
        {
            List<GridView> gridViews;

            if (!GridViewsByColumnsSource.TryGetValue(columnSource, out gridViews))
            {
                gridViews = new List<GridView>();
                GridViewsByColumnsSource.Add(columnSource, gridViews);
            }

            return gridViews;
        }

        private static void AddHandlers(GridView gridView, ICollectionView view)
        {
            GetGridViewsForColumnSource(view).Add(gridView);
            view.CollectionChanged += ColumnsSourceCollectionChanged;
        }

        private static void CreateColumns(GridView gridView, ICollectionView view)
        {
            foreach (var item in view)
            {
                GridViewColumn column = CreateColumn(gridView, item);
                gridView.Columns.Add(column);
            }
        }

        private static void RemoveHandlers(GridView gridView, ICollectionView view)
        {
            view.CollectionChanged -= ColumnsSourceCollectionChanged;
            GetGridViewsForColumnSource(view).Remove(gridView);
        }

        private static void ColumnsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var view = sender as ICollectionView;
            var gridViews = GetGridViewsForColumnSource(view);

            if (gridViews == null || gridViews.Count == 0)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var gridView in gridViews)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            GridViewColumn column = CreateColumn(gridView, e.NewItems[i]);
                            gridView.Columns.Insert(e.NewStartingIndex + i, column);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    foreach (var gridView in gridViews)
                    {
                        var columns = new List<GridViewColumn>();

                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            GridViewColumn column = gridView.Columns[e.OldStartingIndex + i];
                            columns.Add(column);
                        }

                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            GridViewColumn column = columns[i];
                            gridView.Columns.Insert(e.NewStartingIndex + i, column);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var gridView in gridViews)
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            gridView.Columns.RemoveAt(e.OldStartingIndex);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var gridView in gridViews)
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            GridViewColumn column = CreateColumn(gridView, e.NewItems[i]);
                            gridView.Columns[e.NewStartingIndex + i] = column;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (var gridView in gridViews)
                    {
                        gridView.Columns.Clear();
                        CreateColumns(gridView, sender as ICollectionView);
                    }
                    break;
            }
        }

        private static GridViewColumn CreateColumn(GridView gridView, object columnSource)
        {
            var column = new GridViewColumn();
            string headerTextMember = GetHeaderTextMember(gridView);
            string displayMemberMember = GetDisplayMemberMember(gridView);

            if (!string.IsNullOrEmpty(headerTextMember))
            {
                column.Header = GetPropertyValue(columnSource, headerTextMember);
            }

            if (!string.IsNullOrEmpty(displayMemberMember))
            {
                var propertyName = GetPropertyValue(columnSource, displayMemberMember) as string;
                column.DisplayMemberBinding = new Binding(propertyName);
                if (propertyName.Contains("Date"))
                {
                    string shortDatePattern = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
                    string shortTimePattern = CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern;

                    column.DisplayMemberBinding.StringFormat = shortDatePattern + " " + shortTimePattern;
                }
            }

            return column;
        }

        private static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj != null)
            {
                PropertyInfo prop = obj.GetType().GetProperty(propertyName);

                if (prop != null)
                    return prop.GetValue(obj, null);
            }

            return null;
        }
    }
}