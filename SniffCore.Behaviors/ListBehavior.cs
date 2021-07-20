//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Gives you some commands when clicking in an ItemsControl or its items.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <ListBox controls:ListBehavior.ItemDoubleClickedCommand="{Binding ItemDoubleClickedCommand}"
    ///          
    ///          controls:ListBehavior.ItemClickedCommand="{Binding ItemClickedCommand}"
    ///          
    ///          controls:ListBehavior.EmptyAreaDoubleClickCommand="{Binding EmptyAreaDoubleClickCommand}"
    ///          controls:ListBehavior.EmptyAreaDoubleClickCommandParameter="Parameter"
    ///          
    ///          controls:ListBehavior.EmptyAreaClickCommand="{Binding EmptyAreaClickCommand}"
    ///          controls:ListBehavior.EmptyAreaClickCommandParameter="Parameter"
    ///          
    ///          controls:ListBehavior.AutoDeselect="True" />
    /// ]]>
    /// </code>
    /// </example>
    public class ListBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetItemDoubleClickedCommand(DependencyObject)" />
        ///     <see cref="SetItemDoubleClickedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemDoubleClickedCommandProperty =
            DependencyProperty.RegisterAttached("ItemDoubleClickedCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnItemDoubleClickedCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetItemDoubleClickedCommandParameter(DependencyObject)" />
        ///     <see cref="SetItemDoubleClickedCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemDoubleClickedCommandParameterProperty =
            DependencyProperty.RegisterAttached("ItemDoubleClickedCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetItemClickedCommand(DependencyObject)" />
        ///     <see cref="SetItemClickedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemClickedCommandProperty =
            DependencyProperty.RegisterAttached("ItemClickedCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnItemClickedCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetItemClickedCommandParameter(DependencyObject)" />
        ///     <see cref="SetItemClickedCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemClickedCommandParameterProperty =
            DependencyProperty.RegisterAttached("ItemClickedCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetEmptyAreaDoubleClickCommand(DependencyObject)" />
        ///     <see cref="SetEmptyAreaDoubleClickCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("EmptyAreaDoubleClickCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnEmptyAreaDoubleClickCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetEmptyAreaDoubleClickCommandParameter(DependencyObject)" />
        ///     <see cref="SetEmptyAreaDoubleClickCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaDoubleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("EmptyAreaDoubleClickCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetEmptyAreaClickCommand(DependencyObject)" />
        ///     <see cref="SetEmptyAreaClickCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaClickCommandProperty =
            DependencyProperty.RegisterAttached("EmptyAreaClickCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnEmptyAreaClickCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetEmptyAreaClickCommandParameter(DependencyObject)" />
        ///     <see cref="SetEmptyAreaClickCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("EmptyAreaClickCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetAutoDeselect(DependencyObject)" />
        ///     <see cref="SetAutoDeselect(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoDeselectProperty =
            DependencyProperty.RegisterAttached("AutoDeselect", typeof(bool), typeof(ListBehavior), new PropertyMetadata(false, OnAutoDeselectChanged));

        private ListBehavior()
        {
        }

        /// <summary>
        ///     Gets the command which will be called when an items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ItemDoubleClickedCommand property value for the element.</returns>
        /// <remarks>If the ItemDoubleClickedCommandParameter is not set, the double clicked item will be passed with the command.</remarks>
        public static ICommand GetItemDoubleClickedCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ItemDoubleClickedCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be called when an items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.ItemDoubleClickedCommand value.</param>
        public static void SetItemDoubleClickedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ItemDoubleClickedCommandProperty, value);
        }

        /// <summary>
        ///     Gets the command parameter which will be passed with the ItemDoubleClickedCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.ItemDoubleClickedCommandParameter property value for the element.</returns>
        public static object GetItemDoubleClickedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(ItemDoubleClickedCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the command parameter to be passed with the ItemDoubleClickedCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.ItemDoubleClickedCommandParameter value.</param>
        public static void SetItemDoubleClickedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ItemDoubleClickedCommandParameterProperty, value);
        }

        private static void OnItemDoubleClickedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl list))
                return;

            if (e.NewValue != null)
                list.MouseDoubleClick -= HandleItemMouseDoubleClick;

            if (e.NewValue != null)
                list.MouseDoubleClick += HandleItemMouseDoubleClick;
        }

        /// <summary>
        ///     Gets the command which will be called when an items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.ItemClickedCommand property value for the element.</returns>
        /// <remarks>If the ItemClickedCommandParameter is not set, the clicked item will be passed with the command.</remarks>
        public static ICommand GetItemClickedCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ItemClickedCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be called when an items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.ItemClickedCommand value.</param>
        public static void SetItemClickedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ItemClickedCommandProperty, value);
        }

        /// <summary>
        ///     Gets the command parameter which will be passed with the ItemClickedCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.ItemClickedCommandParameter property value for the element.</returns>
        public static object GetItemClickedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(ItemClickedCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the command parameter to be passed with the ItemClickedCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.ItemClickedCommandParameter value.</param>
        public static void SetItemClickedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ItemClickedCommandParameterProperty, value);
        }

        private static void OnItemClickedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl list))
                return;

            if (e.NewValue != null)
                list.MouseLeftButtonUp -= HandleItemMouseClick;

            if (e.NewValue != null)
                list.MouseLeftButtonUp += HandleItemMouseClick;
        }

        /// <summary>
        ///     Gets the command which will be called when the area beside the items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.EmptyAreaDoubleClickCommand property value for the element.</returns>
        public static ICommand GetEmptyAreaDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(EmptyAreaDoubleClickCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be called when the area beside the items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.EmptyAreaDoubleClickCommand value.</param>
        public static void SetEmptyAreaDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EmptyAreaDoubleClickCommandProperty, value);
        }

        /// <summary>
        ///     Gets the command parameter which will be passed with the EmptyAreaDoubleClickCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.EmptyAreaDoubleClickCommandParameter property value for the element.</returns>
        public static object GetEmptyAreaDoubleClickCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(EmptyAreaDoubleClickCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the command parameter to be passed with the EmptyAreaDoubleClickCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.EmptyAreaDoubleClickCommandParameter value.</param>
        public static void SetEmptyAreaDoubleClickCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(EmptyAreaDoubleClickCommandParameterProperty, value);
        }

        private static void OnEmptyAreaDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl list))
                return;

            if (e.NewValue != null)
                list.MouseDoubleClick -= HandleEmptyAreaMouseDoubleClick;

            if (e.NewValue != null)
                list.MouseDoubleClick += HandleEmptyAreaMouseDoubleClick;
        }

        /// <summary>
        ///     Gets the command which will be called when the area beside the items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.EmptyAreaDoubleClickCommand property value for the element.</returns>
        public static ICommand GetEmptyAreaClickCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(EmptyAreaClickCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be called when the area beside the items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.EmptyAreaClickCommand value.</param>
        public static void SetEmptyAreaClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EmptyAreaClickCommandProperty, value);
        }

        /// <summary>
        ///     Gets the command parameter which will be passed with the EmptyAreaClickCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.EmptyAreaClickCommandParameter property value for the element.</returns>
        public static object GetEmptyAreaClickCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(EmptyAreaClickCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the command parameter to be passed with the EmptyAreaClickCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.EmptyAreaClickCommandParameter value.</param>
        public static void SetEmptyAreaClickCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(EmptyAreaClickCommandParameterProperty, value);
        }

        private static void OnEmptyAreaClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl list))
                return;

            if (e.NewValue != null)
                list.MouseLeftButtonUp -= HandleEmptyAreaMouseClick;

            if (e.NewValue != null)
                list.MouseLeftButtonUp += HandleEmptyAreaMouseClick;
        }

        /// <summary>
        ///     Gets the value which indicates if the items should be deselected automatically when the area beside the items got
        ///     single clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The ListBehavior.AutoDeselect property value for the element.</returns>
        public static bool GetAutoDeselect(DependencyObject obj)
        {
            return (bool) obj.GetValue(AutoDeselectProperty);
        }

        /// <summary>
        ///     Attaches a value which indicates if the items should be deselected automatically when the area beside the items got
        ///     single clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed ListBehavior.AutoDeselect value.</param>
        public static void SetAutoDeselect(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoDeselectProperty, value);
        }

        private static void OnAutoDeselectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as ItemsControl;
            if (list == null)
                return;

            if ((bool) e.OldValue)
                list.MouseLeftButtonUp -= HandleAutoDeselectClick;

            if ((bool) e.NewValue)
                list.MouseLeftButtonUp += HandleAutoDeselectClick;
        }

        private static void HandleItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer == null)
                return;

            var command = GetItemDoubleClickedCommand(data.List);
            var parameter = GetItemDoubleClickedCommandParameter(data.List) ?? data.ItemContainer.DataContext;

            Invoke(command, parameter);
        }

        private static void HandleItemMouseClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer == null)
                return;

            var command = GetItemClickedCommand(data.List);
            var parameter = GetItemClickedCommandParameter(data.List) ?? data.ItemContainer.DataContext;

            Invoke(command, parameter);
        }

        private static void HandleEmptyAreaMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer != null)
                return;

            var command = GetEmptyAreaDoubleClickCommand(data.List);
            var parameter = GetEmptyAreaDoubleClickCommandParameter(data.List);

            Invoke(command, parameter);
        }

        private static void HandleEmptyAreaMouseClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer != null)
                return;

            var command = GetEmptyAreaClickCommand(data.List);
            var parameter = GetEmptyAreaClickCommandParameter(data.List);

            Invoke(command, parameter);
        }

        private static void HandleAutoDeselectClick(object sender, MouseButtonEventArgs e)
        {
            var list = sender as Selector;
            if (list == null)
                return;

            if (list.ContainerFromElement((DependencyObject) e.OriginalSource) is FrameworkElement itemContainer)
                return;

            list.SelectedIndex = -1;
        }

        private static Data GetData(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ItemsControl list))
                return null;

            return new Data(list, list.ContainerFromElement((DependencyObject) e.OriginalSource) as FrameworkElement);
        }

        private static void Invoke(ICommand command, object parameter)
        {
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }

        private class Data
        {
            public Data(DependencyObject list, FrameworkElement itemContainer)
            {
                List = list;
                ItemContainer = itemContainer;
            }

            public DependencyObject List { get; }
            public FrameworkElement ItemContainer { get; }
        }
    }
}