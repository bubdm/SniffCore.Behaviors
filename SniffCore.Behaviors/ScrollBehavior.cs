//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Brings the feature to modify the scroll position of an items control.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <ListBox ItemsSource="{Binding LogEntries}"
    ///          controls:ScrollBehavior.AutoScrollToLast="True" />
    /// 
    /// <ListBox ItemsSource="{Binding LogEntries}"
    ///          controls:ScrollBehavior.ScrollToItem="{Binding ImportantEntry}" />
    /// ]]>
    /// </code>
    /// </example>
    public class ScrollBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetScrollToItem(DependencyObject)" />
        ///     <see cref="SetScrollToItem(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ScrollToItemProperty =
            DependencyProperty.RegisterAttached("ScrollToItem", typeof(object), typeof(ScrollBehavior), new UIPropertyMetadata(OnScrollChanged));

        /// <summary>
        ///     Identifies the <see cref="GetAutoScrollToLast(DependencyObject)" />
        ///     <see cref="SetAutoScrollToLast(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoScrollToLastProperty =
            DependencyProperty.RegisterAttached("AutoScrollToLast", typeof(bool), typeof(ScrollBehavior), new UIPropertyMetadata(OnScrollChanged));

        /// <summary>
        ///     Identifies the <see cref="GetAutoScrollToSelected(DependencyObject)" />
        ///     <see cref="SetAutoScrollToSelected(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoScrollToSelectedProperty =
            DependencyProperty.RegisterAttached("AutoScrollToSelected", typeof(bool), typeof(ScrollBehavior), new UIPropertyMetadata(OnScrollChanged));

        private static readonly DependencyProperty ScrollBehaviorProperty =
            DependencyProperty.RegisterAttached("ScrollBehavior", typeof(ScrollBehavior), typeof(ScrollBehavior), new UIPropertyMetadata(null));

        private ScrollBehavior()
        {
        }

        /// <summary>
        ///     Gets the item to which it has to scroll in a list.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ScrollBehavior.ScrollToItem property value for the element.</returns>
        public static object GetScrollToItem(DependencyObject obj)
        {
            return obj.GetValue(ScrollToItemProperty);
        }

        /// <summary>
        ///     Attaches the item to which it has to scroll in a list.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ScrollBehavior.ScrollToItem value.</param>
        public static void SetScrollToItem(DependencyObject obj, object value)
        {
            obj.SetValue(ScrollToItemProperty, value);
        }

        /// <summary>
        ///     Gets a value that indicates if a list automatically have to scroll to the last item if the item collection changes.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ScrollBehavior.AutoScrollToLast property value for the element.</returns>
        public static bool GetAutoScrollToLast(DependencyObject obj)
        {
            return (bool) obj.GetValue(AutoScrollToLastProperty);
        }

        /// <summary>
        ///     Attaches a value that indicates if a list automatically have to scroll to the last item if the item collection
        ///     changes.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ScrollBehavior.AutoScrollToLast value.</param>
        public static void SetAutoScrollToLast(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToLastProperty, value);
        }

        /// <summary>
        ///     Gets a value that indicates if a list automatically have to scroll to the selected item if the selection has been
        ///     changed.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ScrollBehavior.AutoScrollToSelected property value for the element.</returns>
        public static bool GetAutoScrollToSelected(DependencyObject obj)
        {
            return (bool) obj.GetValue(AutoScrollToSelectedProperty);
        }

        /// <summary>
        ///     Attaches a value that indicates if a list automatically have to scroll to the selected item if the selection has
        ///     been changed.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ScrollBehavior.AutoScrollToSelected value.</param>
        public static void SetAutoScrollToSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToSelectedProperty, value);
        }

        private static ScrollBehavior GetScrollBehavior(DependencyObject obj)
        {
            return (ScrollBehavior) obj.GetValue(ScrollBehaviorProperty);
        }

        private static void SetScrollBehavior(DependencyObject obj, ScrollBehavior value)
        {
            obj.SetValue(ScrollBehaviorProperty, value);
        }

        private static void OnScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ListBox listbox) && !(sender is DataGrid dataGrid))
                throw new InvalidOperationException("The ScrollBehavior.ScrollToItem only can be attached to an ListBox, ListView or DataGrid.");

            var scrollBehavior = GetScrollBehavior(sender);
            if (scrollBehavior == null)
            {
                scrollBehavior = new ScrollBehavior();
                SetScrollBehavior(sender, scrollBehavior);
                ((FrameworkElement) sender).Loaded += scrollBehavior.Element_Loaded;
            }

            scrollBehavior.Element_Loaded(sender, null);
        }

        private void Element_Loaded(object sender, RoutedEventArgs _)
        {
            var depObj = (DependencyObject) sender;
            var autoSelectionScroll = GetAutoScrollToSelected(depObj);
            var autoLastScroll = GetAutoScrollToLast(depObj);
            var scrollToItem = GetScrollToItem(depObj);

            switch (sender)
            {
                case ListBox listbox:
                    Scroll(listbox, scrollToItem, autoLastScroll, autoSelectionScroll);
                    break;
                case DataGrid dataGrid:
                    Scroll(dataGrid, scrollToItem, autoLastScroll, autoSelectionScroll);
                    break;
            }
        }

        private void Scroll(ListBox listbox, object scrollToItem, bool autoLastScroll, bool autoSelectionScroll)
        {
            ScrollToItem(listbox, scrollToItem);
            if (autoLastScroll)
            {
                if (listbox.ItemsSource is INotifyCollectionChanged collection)
                    collection.CollectionChanged += (owner, arg) => { ScrollToLast(listbox); };
                ScrollToLast(listbox);
            }

            if (autoSelectionScroll)
                listbox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = (ListBox) sender;
            if (listbox.SelectedItem != null)
                listbox.ScrollIntoView(listbox.SelectedItem);
        }

        private void ScrollToItem(ListBox listBox, object item)
        {
            if (item != null)
                listBox.ScrollIntoView(item);
        }

        private void ScrollToLast(ListBox listBox)
        {
            if (listBox.Items.Count > 0)
                listBox.ScrollIntoView(listBox.Items[^1]);
        }

        private void Scroll(DataGrid dataGrid, object scrollToItem, bool autoLastScroll, bool autoSelectionScroll)
        {
            ScrollToItem(dataGrid, scrollToItem);
            if (autoLastScroll)
            {
                if (dataGrid.ItemsSource is INotifyCollectionChanged collection)
                    collection.CollectionChanged += (owner, arg) => { ScrollToLast(dataGrid); };
                ScrollToLast(dataGrid);
            }

            if (autoSelectionScroll)
                dataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = (DataGrid) sender;
            if (dataGrid.SelectedItem != null)
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
        }

        private void ScrollToItem(DataGrid dataGrid, object item)
        {
            if (item != null)
                dataGrid.ScrollIntoView(item);
        }

        private void ScrollToLast(DataGrid dataGrid)
        {
            if (dataGrid.Items.Count > 0)
                dataGrid.ScrollIntoView(dataGrid.Items[^1]);
        }
    }
}