//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Brings the feature to the <see cref="System.Windows.Window" /> to disable or hide elements in the title bar.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <Window controls:WindowTitleBarBehavior.DisableMinimizeButton="True"
    ///         controls:WindowTitleBarBehavior.DisableMaximizeButton="True"
    ///         controls:WindowTitleBarBehavior.DisableSystemMenu="True">
    /// </Window>
    /// ]]>
    /// </code>
    /// </example>
    public static class WindowTitleBarBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetRemoveTitleItems(DependencyObject)" />
        ///     <see cref="SetRemoveTitleItems(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty RemoveTitleItemsProperty =
            DependencyProperty.RegisterAttached("RemoveTitleItems", typeof(bool), typeof(WindowTitleBarBehavior), new UIPropertyMetadata(false, OnRemoveTitleItemsChanged));

        /// <summary>
        ///     Identifies the <see cref="GetDisableMinimizeButton(DependencyObject)" />
        ///     <see cref="SetDisableMinimizeButton(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DisableMinimizeButtonProperty =
            DependencyProperty.RegisterAttached("DisableMinimizeButton", typeof(bool), typeof(WindowTitleBarBehavior), new UIPropertyMetadata(false, OnDisableMinimizeButtonChanged));

        /// <summary>
        ///     Identifies the <see cref="GetDisableMaximizeButton(DependencyObject)" />
        ///     <see cref="SetDisableMaximizeButton(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DisableMaximizeButtonProperty =
            DependencyProperty.RegisterAttached("DisableMaximizeButton", typeof(bool), typeof(WindowTitleBarBehavior), new UIPropertyMetadata(false, OnDisableMaximizeButtonChanged));

        /// <summary>
        ///     Identifies the <see cref="GetDisableCloseButton(DependencyObject)" />
        ///     <see cref="SetDisableCloseButton(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DisableCloseButtonProperty =
            DependencyProperty.RegisterAttached("DisableCloseButton", typeof(bool), typeof(WindowTitleBarBehavior), new PropertyMetadata(false, OnDisableCloseButtonChanged));

        /// <summary>
        ///     Identifies the <see cref="GetDisableSystemMenu(DependencyObject)" />
        ///     <see cref="SetDisableSystemMenu(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DisableSystemMenuProperty =
            DependencyProperty.RegisterAttached("DisableSystemMenu", typeof(bool), typeof(WindowTitleBarBehavior), new PropertyMetadata(false, OnDisableSystemMenuChanged));

        /// <summary>
        ///     Gets a value the indicates if the window has to show title bar items or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowTitleBarBehavior.RemoveTitleItems property value for the element.</returns>
        public static bool GetRemoveTitleItems(DependencyObject obj)
        {
            return (bool) obj.GetValue(RemoveTitleItemsProperty);
        }

        /// <summary>
        ///     Attaches a value the indicates if the window has to show title bar items or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowTitleBarBehavior.RemoveTitleItems value.</param>
        public static void SetRemoveTitleItems(DependencyObject obj, bool value)
        {
            obj.SetValue(RemoveTitleItemsProperty, value);
        }

        /// <summary>
        ///     Gets a value the indicates if the window has an enabled minimize button or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowTitleBarBehavior.DisableMinimizeButton property value for the element.</returns>
        public static bool GetDisableMinimizeButton(DependencyObject obj)
        {
            return (bool) obj.GetValue(DisableMinimizeButtonProperty);
        }

        /// <summary>
        ///     Attaches a value the indicates if the window has an enabled minimize button or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowTitleBarBehavior.DisableMinimizeButton value.</param>
        public static void SetDisableMinimizeButton(DependencyObject obj, bool value)
        {
            obj.SetValue(DisableMinimizeButtonProperty, value);
        }

        /// <summary>
        ///     Gets a value the indicates if the window has an enabled maximize button or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowTitleBarBehavior.DisableMaximizeButton property value for the element.</returns>
        public static bool GetDisableMaximizeButton(DependencyObject obj)
        {
            return (bool) obj.GetValue(DisableMaximizeButtonProperty);
        }

        /// <summary>
        ///     Attaches a value the indicates if the window has an enabled maximize button or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowTitleBarBehavior.DisableMaximizeButton value.</param>
        public static void SetDisableMaximizeButton(DependencyObject obj, bool value)
        {
            obj.SetValue(DisableMaximizeButtonProperty, value);
        }

        /// <summary>
        ///     Gets a value that indicates if the window has an enabled close button or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <returns>The WindowTitleBarBehavior.DisableCloseButton property value for the element.</returns>
        public static bool GetDisableCloseButton(DependencyObject obj)
        {
            return (bool) obj.GetValue(DisableCloseButtonProperty);
        }

        /// <summary>
        ///     Attaches a value the indicates if the window has an enabled close button or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowTitleBarBehavior.DisableCloseButton value.</param>
        public static void SetDisableCloseButton(DependencyObject obj, bool value)
        {
            obj.SetValue(DisableCloseButtonProperty, value);
        }

        /// <summary>
        ///     Gets a value that indicates if the window has a system menu or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <returns>The WindowTitleBarBehavior.DisableSystemMenu property value for the element.</returns>
        public static bool GetDisableSystemMenu(DependencyObject obj)
        {
            return (bool) obj.GetValue(DisableSystemMenuProperty);
        }

        /// <summary>
        ///     Attaches a value the indicates if the window has a system menu or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowTitleBarBehavior.DisableSystemMenu value.</param>
        public static void SetDisableSystemMenu(DependencyObject obj, bool value)
        {
            obj.SetValue(DisableSystemMenuProperty, value);
        }

        private static void OnRemoveTitleItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.SourceInitialized += RemoveTitleItems_SourceInitialized;
        }

        private static void OnDisableMinimizeButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.SourceInitialized += DisableMinimizeButton_SourceInitialized;
        }

        private static void OnDisableMaximizeButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.SourceInitialized += DisableMaximizeButton_SourceInitialized;
        }

        private static void OnDisableCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.SourceInitialized += DisableCloseButton_SourceInitialized;
        }

        private static void OnDisableSystemMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.SourceInitialized += DisableSystemMenu_SourceInitialized;
        }

        private static void RemoveTitleItems_SourceInitialized(object sender, EventArgs e)
        {
            var window = (Window) sender;
            WindowTitleBar.RemoveTitleItems(window);
        }

        private static void DisableMinimizeButton_SourceInitialized(object sender, EventArgs e)
        {
            var window = (Window) sender;
            WindowTitleBar.DisableMinimizeButton(window);
        }

        private static void DisableMaximizeButton_SourceInitialized(object sender, EventArgs e)
        {
            var window = (Window) sender;
            WindowTitleBar.DisableMaximizeButton(window);
        }

        private static void DisableCloseButton_SourceInitialized(object sender, EventArgs e)
        {
            var window = (Window) sender;
            WindowTitleBar.DisableCloseButton(window);
        }

        private static void DisableSystemMenu_SourceInitialized(object sender, EventArgs e)
        {
            var window = (Window) sender;
            WindowTitleBar.DisableSystemMenu(window);
        }
    }
}