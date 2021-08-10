//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;
using System.Windows.Input;

// ReSharper disable CheckNamespace

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Brings the feature to be able to double click any UI element.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <TextBlock Text="Doubleclick Me"
    ///            controls:DoubleClickBehavior.Command="{Binding ItemDoubleClicked}"
    ///            controls:DoubleClickBehavior.CommandParameter="Parameter" />
    /// ]]>
    /// </code>
    /// </example>
    public static class DoubleClickBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetCommand(DependencyObject)" /> <see cref="SetCommand(DependencyObject, ICommand)" />
        ///     attached property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(DoubleClickBehavior), new UIPropertyMetadata(OnCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetCommandParameter(DependencyObject)" />
        ///     <see cref="SetCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(DoubleClickBehavior), new UIPropertyMetadata(null));

        /// <summary>
        ///     Gets the command to be called when the element gets double clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DoubleClickBehavior.Command property value for the element.</returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(CommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be called when the element gets double clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DoubleClickBehavior.Command value.</param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        /// <summary>
        ///     Gets the command parameter to be passed when the called when DoubleClickBehavior.Command gets called.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DoubleClickBehavior.CommandParameter property value for the element.</returns>
        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the command parameter to be passed when the called when DoubleClickBehavior.Command gets called.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DoubleClickBehavior.CommandParameter value.</param>
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UIElement control))
                throw new InvalidOperationException("The DoubleClickBehavior can only attached to an UIElement");

            control.PreviewMouseLeftButtonDown += MouseButtonDown;
        }

        private static void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
                return;

            var command = GetCommand((DependencyObject) sender);
            var parameter = GetCommandParameter((DependencyObject) sender);
            if (parameter == null && sender is FrameworkElement element)
                parameter = element.DataContext;
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}