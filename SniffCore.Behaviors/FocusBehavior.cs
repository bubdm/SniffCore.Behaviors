//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;
using System.Windows.Input;
using SniffCore.Behaviors.Internal;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Brings the feature to set the focus to a specific element or on window launch.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <Window controls:FocusBehavior.ApplicationGotFocusCommand="{Binding SwitchedToApplicationCommand}"
    ///         controls:FocusBehavior.ApplicationLostFocusCommand="{Binding SwitchedOutFromApplicationCommand}">
    /// </Window>
    /// 
    /// <Button controls:FocusBehavior.GotFocusCommand="{Binding ButtonGotFocusCommand}"
    ///         controls:FocusBehavior.GotFocusCommandParameter="Example" />
    /// 
    /// <Button controls:FocusBehavior.LostFocusCommand="{Binding ButtonGotFocusCommand}"
    ///         controls:FocusBehavior.LostFocusCommandParameter="Example" />
    ///  
    /// <Button controls:FocusBehavior.HasFocus="{Binding IsButtonFocused}" />
    /// ]]>
    /// </code>
    /// </example>
    public class FocusBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetStartFocusedControl(DependencyObject)" />
        ///     <see cref="SetStartFocusedControl(DependencyObject, UIElement)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty StartFocusedControlProperty =
            DependencyProperty.RegisterAttached("StartFocusedControl", typeof(UIElement), typeof(FocusBehavior), new UIPropertyMetadata(OnStartFocusedControlChanged));

        /// <summary>
        ///     Identifies the <see cref="GetHasFocus(DependencyObject)" /> <see cref="SetHasFocus(DependencyObject, bool)" />
        ///     attached property.
        /// </summary>
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.RegisterAttached("HasFocus", typeof(bool), typeof(FocusBehavior), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHasFocusChanged));

        /// <summary>
        ///     Identifies the <see cref="GetLostFocusCommand(DependencyObject)" />
        ///     <see cref="SetLostFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LostFocusCommandProperty =
            DependencyProperty.RegisterAttached("LostFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(LostFocusCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetLostFocusCommandParameter(DependencyObject)" />
        ///     <see cref="SetLostFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LostFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("LostFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetGotFocusCommand(DependencyObject)" />
        ///     <see cref="SetGotFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty GotFocusCommandProperty =
            DependencyProperty.RegisterAttached("GotFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(OnGotFocusCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetGotFocusCommandParameter(DependencyObject)" />
        ///     <see cref="SetGotFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty GotFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("GotFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetApplicationLostFocusCommand(DependencyObject)" />
        ///     <see cref="SetApplicationLostFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationLostFocusCommandProperty =
            DependencyProperty.RegisterAttached("ApplicationLostFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetApplicationLostFocusCommandParameter(DependencyObject)" />
        ///     <see cref="SetApplicationLostFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationLostFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("ApplicationLostFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(OnApplicationFocusCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetApplicationGotFocusCommand(DependencyObject)" />
        ///     <see cref="SetApplicationGotFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationGotFocusCommandProperty =
            DependencyProperty.RegisterAttached("ApplicationGotFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(OnApplicationFocusCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetApplicationGotFocusCommandParameter(DependencyObject)" />
        ///     <see cref="SetApplicationGotFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationGotFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("ApplicationGotFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(null));

        private static readonly DependencyProperty GlobalEventWatcherProperty =
            DependencyProperty.RegisterAttached("GlobalEventWatcher", typeof(GlobalEventWatcher), typeof(FocusBehavior));

        /// <summary>
        ///     Gets the control which has to get the focus when its loaded.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.StartFocusedControl property value for the element.</returns>
        public static UIElement GetStartFocusedControl(DependencyObject obj)
        {
            return (UIElement) obj.GetValue(StartFocusedControlProperty);
        }

        /// <summary>
        ///     Attaches the control which has to get the focus when its loaded.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.StartFocusedControl value.</param>
        public static void SetStartFocusedControl(DependencyObject obj, UIElement value)
        {
            obj.SetValue(StartFocusedControlProperty, value);
        }

        private static void OnStartFocusedControlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement element))
                throw new InvalidOperationException("The FocusBehavior.StartFocusedControl only can be attached to an FrameworkElement");

            element.Loaded += Element_Loaded;
        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            var target = GetStartFocusedControl((DependencyObject) sender);
            ControlFocus.GiveFocus(target);
        }

        /// <summary>
        ///     Gets a value that indicates the state if the element has the focus or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.HasFocus property value for the element.</returns>
        public static bool GetHasFocus(DependencyObject obj)
        {
            return (bool) obj.GetValue(HasFocusProperty);
        }

        /// <summary>
        ///     Attaches a value that indicates the state if the element has the focus or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.HasFocus value.</param>
        public static void SetHasFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(HasFocusProperty, value);
        }

        private static void OnHasFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement element))
                throw new InvalidOperationException("The FocusBehavior.HasFocus only can be attached to an FrameworkElement");

            if ((bool) e.NewValue)
            {
                ControlFocus.GiveFocus(element);
                element.LostFocus += Element_LostFocus;
            }
        }

        private static void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement) sender;
            element.LostFocus -= Element_LostFocus;
            SetHasFocus(element, false);
        }

        /// <summary>
        ///     Gets the command to be executed when the element lost its focus.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.LostFocusCommand property value for the element.</returns>
        public static ICommand GetLostFocusCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(LostFocusCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be executed when the control lost its focus.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.LostFocusCommand value.</param>
        public static void SetLostFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LostFocusCommandProperty, value);
        }

        /// <summary>
        ///     Gets the parameter to be passed with the FocusBehavior.LostFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.LostFocusCommandParameter property value for the element.</returns>
        public static object GetLostFocusCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(LostFocusCommandParameterProperty);
        }

        /// <summary>
        ///     Sets the parameter to be passed with the FocusBehavior.LostFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.LostFocusCommandParameter value.</param>
        public static void SetLostFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(LostFocusCommandParameterProperty, value);
        }

        private static void LostFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement control))
                throw new InvalidOperationException("The FocusBehavior.LostFocusCommand only can be attached to an UIElement");

            if (e.OldValue != null)
                control.LostFocus -= HandleLostFocus;
            if (e.NewValue != null)
                control.LostFocus += HandleLostFocus;
        }

        private static void HandleLostFocus(object sender, RoutedEventArgs e)
        {
            var command = GetLostFocusCommand((DependencyObject) sender);
            var commandParameter = GetLostFocusCommandParameter((DependencyObject) sender);
            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        /// <summary>
        ///     Gets the command to be executed when the element got the focus.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.GotFocusCommand property value for the element.</returns>
        public static ICommand GetGotFocusCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(GotFocusCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be executed when the control got the focus.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.GotFocusCommand value.</param>
        public static void SetGotFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(GotFocusCommandProperty, value);
        }

        /// <summary>
        ///     Gets the parameter to be passed with the FocusBehavior.GotFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.GotFocusCommandParameter property value for the element.</returns>
        public static object GetGotFocusCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(GotFocusCommandParameterProperty);
        }

        /// <summary>
        ///     Sets the parameter to be passed with the FocusBehavior.GotFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.GotFocusCommandParameter value.</param>
        public static void SetGotFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(GotFocusCommandParameterProperty, value);
        }

        private static void OnGotFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement control))
                throw new InvalidOperationException("The FocusBehavior.LostFocusCommand only can be attached to an UIElement");

            if (e.OldValue != null)
                control.GotFocus -= HandleGotFocus;
            if (e.NewValue != null)
                control.GotFocus += HandleGotFocus;
        }

        private static void HandleGotFocus(object sender, RoutedEventArgs e)
        {
            var command = GetGotFocusCommand((DependencyObject) sender);
            var commandParameter = GetGotFocusCommandParameter((DependencyObject) sender);
            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        /// <summary>
        ///     Gets the command to be executed when the application is not the foreground application anymore.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.ApplicationLostFocusCommand property value for the element.</returns>
        public static ICommand GetApplicationLostFocusCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ApplicationLostFocusCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be executed when the application is not the foreground application anymore.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.ApplicationLostFocusCommand value.</param>
        public static void SetApplicationLostFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ApplicationLostFocusCommandProperty, value);
        }

        /// <summary>
        ///     Gets the parameter to be passed with the FocusBehavior.ApplicationLostFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.ApplicationLostFocusCommandParameter property value for the element.</returns>
        public static object GetApplicationLostFocusCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(ApplicationLostFocusCommandParameterProperty);
        }

        /// <summary>
        ///     Sets the parameter to be passed with the FocusBehavior.ApplicationLostFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.ApplicationLostFocusCommandParameter value.</param>
        public static void SetApplicationLostFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ApplicationLostFocusCommandParameterProperty, value);
        }

        /// <summary>
        ///     Gets the command to be executed when the application become the foreground application.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.ApplicationGotFocusCommand property value for the element.</returns>
        public static ICommand GetApplicationGotFocusCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ApplicationGotFocusCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to be executed when the application become the foreground application.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.ApplicationGotFocusCommand value.</param>
        public static void SetApplicationGotFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ApplicationGotFocusCommandProperty, value);
        }

        /// <summary>
        ///     Gets the parameter to be passed with the FocusBehavior.ApplicationGotFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The FocusBehavior.ApplicationGotFocusCommandParameter property value for the element.</returns>
        public static object GetApplicationGotFocusCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(ApplicationGotFocusCommandParameterProperty);
        }

        /// <summary>
        ///     Sets the parameter to be passed with the FocusBehavior.ApplicationGotFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed FocusBehavior.ApplicationGotFocusCommandParameter value.</param>
        public static void SetApplicationGotFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ApplicationGotFocusCommandParameterProperty, value);
        }

        private static void OnApplicationFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var watcher = GetGlobalEventWatcher(d);
            watcher?.Dispose();

            if (e.NewValue == null)
                return;

            watcher = new GlobalEventWatcher(d);
            SetGlobalEventWatcher(d, watcher);
        }

        private static GlobalEventWatcher GetGlobalEventWatcher(DependencyObject obj)
        {
            return (GlobalEventWatcher) obj.GetValue(GlobalEventWatcherProperty);
        }

        private static void SetGlobalEventWatcher(DependencyObject obj, GlobalEventWatcher value)
        {
            obj.SetValue(GlobalEventWatcherProperty, value);
        }

        private class GlobalEventWatcher
        {
            private DependencyObject _owner;

            public GlobalEventWatcher(DependencyObject owner)
            {
                _owner = owner;
                Application.Current.Activated += HandleApplicationActivated;
                Application.Current.Deactivated += HandleApplicationDeactivated;
            }

            public void Dispose()
            {
                Application.Current.Activated -= HandleApplicationActivated;
                Application.Current.Deactivated -= HandleApplicationDeactivated;
                _owner = null;
            }

            private void HandleApplicationActivated(object sender, EventArgs e)
            {
                var command = GetApplicationGotFocusCommand(_owner);
                var commandParameter = GetApplicationGotFocusCommandParameter(_owner);
                if (command != null && command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
            }

            private void HandleApplicationDeactivated(object sender, EventArgs e)
            {
                var command = GetApplicationLostFocusCommand(_owner);
                var commandParameter = GetApplicationLostFocusCommandParameter(_owner);
                if (command != null && command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
            }
        }
    }
}