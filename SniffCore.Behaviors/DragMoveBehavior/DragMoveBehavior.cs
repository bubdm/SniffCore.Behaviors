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
    ///     Enables that a window can be moved by the mouse when drop down on a content.
    /// </summary>
    /// <example>
    ///     <code lang="xaml">
    /// <![CDATA[
    /// <Window controls:DragMoveBehavior.Enabled="True">
    /// </Window>
    /// 
    /// <Window>
    ///     <DockPanel>
    ///         <Border Height="22" Background="Gray" controls:DragMoveBehavior.Enabled="True">
    ///             <TextBlock Text="{Binding Title}" />
    ///         </Border>
    /// 
    ///         <Grid>
    ///         </Grid>
    ///     </DockPanel>
    /// </Window>
    /// ]]>
    /// </code>
    /// </example>
    public static class DragMoveBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetEnable(DependencyObject)" /> <see cref="SetEnable(DependencyObject, DataTemplate)" />
        ///     attached property.
        /// </summary>
        public static readonly DependencyProperty EnableProperty =
            DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(DragMoveBehavior), new PropertyMetadata(OnEnableChanged));

        /// <summary>
        ///     Gets the indicator whether the window shall be moved on pressed on the UI element or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>True if the mouse button down is observed on the UIElement or not.</returns>
        public static bool GetEnable(DependencyObject obj)
        {
            return (bool) obj.GetValue(EnableProperty);
        }

        /// <summary>
        ///     Sets the indicator whether the window shall be moved on pressed on the UI element or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is set to.</param>
        /// <param name="value">The indicator whether the window shall be moved on pressed on the UI element or not.</param>
        public static void SetEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableProperty, value);
        }

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement element))
                throw new InvalidOperationException("The DragMoveBehavior.Enable can be attached to a element only.");

            if ((bool) e.OldValue)
                element.MouseDown -= OnWindowMouseDown;
            if ((bool) e.NewValue)
                element.MouseDown += OnWindowMouseDown;
        }

        private static void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var window = Window.GetWindow((UIElement) sender);
                window?.DragMove();
            }
        }
    }
}