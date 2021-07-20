//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SniffCore.Behaviors.Internal;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Brings the feature to a <see cref="Window" /> to bind loading and closing action or easy close with dialog result.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <Window controls:WindowBehavior.ClosingCommand="{Binding ClosingCommand}">
    /// 
    ///     <Button Content="Close" controls:WindowBehavior.DialogResult="True" />
    ///     
    ///     <Button Content="Try Close" controls:WindowBehavior.DialogResultCommand="{Binding TryCloseCommand}" />
    /// 
    /// </Window>
    /// ]]>
    /// </code>
    ///     <code lang="csharp">
    /// <![CDATA[
    /// public class MainViewModel : ObservableObject
    /// {
    ///     public MainViewModel()
    ///     {
    ///         TryCloseCommand = new DelegateCommand<WindowClosingArgs>(TryClose);
    ///     }
    /// 
    ///     public DelegateCommand<WindowClosingArgs> TryCloseCommand { get; private set; }
    /// 
    ///     private void TryClose(WindowClosingArgs e)
    ///     {
    ///         // Ask user if really close
    ///         e.Cancel = true;
    /// 
    ///         //e.DialogResult = false;
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public static class WindowBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetDialogResult(DependencyObject)" />
        ///     <see cref="SetDialogResult(DependencyObject, bool?)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(WindowBehavior), new UIPropertyMetadata(OnDialogResultChanged));

        /// <summary>
        ///     Identifies the <see cref="GetDialogResultCommand(DependencyObject)" />
        ///     <see cref="SetDialogResultCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DialogResultCommandProperty =
            DependencyProperty.RegisterAttached("DialogResultCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnDialogResultChanged));

        /// <summary>
        ///     Identifies the <see cref="GetDialogResultCommandParameter(DependencyObject)" />
        ///     <see cref="SetDialogResultCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DialogResultCommandParameterProperty =
            DependencyProperty.RegisterAttached("DialogResultCommandParameter", typeof(object), typeof(WindowBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetClosingCommand(DependencyObject)" />
        ///     <see cref="SetClosingCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ClosingCommandProperty =
            DependencyProperty.RegisterAttached("ClosingCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnClosingCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetClosingCommandParameter(DependencyObject)" />
        ///     <see cref="SetClosingCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ClosingCommandParameterProperty =
            DependencyProperty.RegisterAttached("ClosingCommandParameter", typeof(object), typeof(WindowBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetClosedCommand(DependencyObject)" />
        ///     <see cref="SetClosedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ClosedCommandProperty =
            DependencyProperty.RegisterAttached("ClosedCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnClosedCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetClosedCommandParameter(DependencyObject)" />
        ///     <see cref="SetClosedCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ClosedCommandParameterProperty =
            DependencyProperty.RegisterAttached("ClosedCommandParameter", typeof(object), typeof(WindowBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetLoadedCommand(DependencyObject)" />
        ///     <see cref="SetLoadedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.RegisterAttached("LoadedCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnLoadedCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetLoadedCommandParameter(DependencyObject)" />
        ///     <see cref="SetLoadedCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LoadedCommandParameterProperty =
            DependencyProperty.RegisterAttached("LoadedCommandParameter", typeof(object), typeof(WindowBehavior), new UIPropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetIsClose(DependencyObject)" /> <see cref="SetIsClose(DependencyObject, bool)" />
        ///     attached property.
        /// </summary>
        public static readonly DependencyProperty IsCloseProperty =
            DependencyProperty.RegisterAttached("IsClose", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(OnIsCloseCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetIsCloseCommand(DependencyObject)" />
        ///     <see cref="SetIsCloseCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty IsCloseCommandProperty =
            DependencyProperty.RegisterAttached("IsCloseCommand", typeof(ICommand), typeof(WindowBehavior), new PropertyMetadata(OnIsCloseCommandChanged));

        /// <summary>
        ///     Identifies the <see cref="GetDialogResultCommandParameter(DependencyObject)" />
        ///     <see cref="SetDialogResultCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty IsCloseCommandParameterProperty =
            DependencyProperty.RegisterAttached("IsCloseCommandParameter", typeof(object), typeof(WindowBehavior), new PropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="GetWinApiMessages(DependencyObject)" />
        ///     <see cref="SetWinApiMessages(DependencyObject, string)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty WinApiMessagesProperty =
            DependencyProperty.RegisterAttached("WinApiMessages", typeof(string), typeof(WindowBehavior), new UIPropertyMetadata(OnWinApiMessagesChanged));

        /// <summary>
        ///     Identifies the <see cref="GetWinApiCommand(DependencyObject)" />
        ///     <see cref="SetWinApiCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty WinApiCommandProperty =
            DependencyProperty.RegisterAttached("WinApiCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnWinApiCommandChanged));

        private static readonly DependencyProperty ObserverProperty =
            DependencyProperty.RegisterAttached("Observer", typeof(WindowObserver), typeof(WindowBehavior), new UIPropertyMetadata(null));

        /// <summary>
        ///     Gets the dialog result from a button to be called on the owner window.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.DialogResult property value for the element.</returns>
        public static bool? GetDialogResult(DependencyObject obj)
        {
            return (bool?) obj.GetValue(DialogResultProperty);
        }

        /// <summary>
        ///     Attaches the dialog result to a button to be called on the owner window.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.DialogResult value.</param>
        public static void SetDialogResult(DependencyObject obj, bool? value)
        {
            obj.SetValue(DialogResultProperty, value);
        }

        /// <summary>
        ///     Gets the dialog result command from a button to get the dialog result called on the owner window.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.DialogResultCommand property value for the element.</returns>
        public static ICommand GetDialogResultCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(DialogResultCommandProperty);
        }

        /// <summary>
        ///     Attaches the dialog result command to a button to get the dialog result called on the owner window.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.DialogResultCommand value.</param>
        public static void SetDialogResultCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DialogResultCommandProperty, value);
        }

        private static void OnDialogResultChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ButtonBase button))
                throw new InvalidOperationException("'WindowBehavior.DialogResultCommand' only can be attached to a 'ButtonBase' object");

            button.Click += DialogResultButton_Click;
        }

        /// <summary>
        ///     Gets the dialog result command parameter passed with the WindowDialogResultArgs.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.DialogResultCommandParameter property value for the element.</returns>
        public static object GetDialogResultCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(DialogResultCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the dialog result command parameter to be passed with the WindowDialogResultArgs.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.DialogResultCommandParameter value.</param>
        public static void SetDialogResultCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(DialogResultCommandParameterProperty, value);
        }

        private static void DialogResultButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var window = Window.GetWindow(button);
            if (window == null)
                return;

            var resultCommand = GetDialogResultCommand(button);
            var args = new WindowDialogResultArgs(GetDialogResultCommandParameter(button));
            if (resultCommand != null && resultCommand.CanExecute(args))
            {
                resultCommand.Execute(args);
                if (!args.Cancel)
                    window.DialogResult = args.DialogResult;
            }
            else
            {
                window.DialogResult = GetDialogResult(button);
            }
        }

        /// <summary>
        ///     Gets the command from a window which get called when the window closes. A WindowClosingArgs is passed as a
        ///     parameter to change the dialog result and cancel the close.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.ClosingCommand property value for the element.</returns>
        public static ICommand GetClosingCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ClosingCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to a window which get called when the window closes. A WindowClosingArgs is passed as a
        ///     parameter to change the dialog result and cancel the close.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.ClosingCommand value.</param>
        public static void SetClosingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClosingCommandProperty, value);
        }

        /// <summary>
        ///     Gets the closing command parameter passed with the WindowClosingArgs.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.ClosingCommandParameter property value for the element.</returns>
        public static object GetClosingCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(ClosingCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the closing command parameter to be passed with the WindowClosingArgs.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.ClosingCommandParameter value.</param>
        public static void SetClosingCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ClosingCommandParameterProperty, value);
        }

        private static void OnClosingCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Window window))
                throw new InvalidOperationException("'WindowBehavior.ClosingCommand' only can be attached to a 'Window' object");

            window.Closing += Window_Closing;
        }

        private static void Window_Closing(object sender, CancelEventArgs e)
        {
            var window = (DependencyObject) sender;
            var command = GetClosingCommand(window);
            var args = new WindowClosingArgs(GetClosingCommandParameter(window));
            if (command != null && command.CanExecute(args))
            {
                command.Execute(args);
                e.Cancel = args.Cancel;
            }
        }

        /// <summary>
        ///     Gets the command from a window which get called when the window has been closed.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.ClosedCommand property value for the element.</returns>
        public static ICommand GetClosedCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(ClosedCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to a window which get called when the window closes.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.ClosingCommand value.</param>
        public static void SetClosedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClosedCommandProperty, value);
        }

        /// <summary>
        ///     Gets the closed command parameter passed with the ClosedCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.ClosedCommandParameter property value for the element.</returns>
        public static object GetClosedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(ClosedCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the closed command parameter to be passed with the ClosedCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.ClosedCommandParameter value.</param>
        public static void SetClosedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ClosedCommandParameterProperty, value);
        }

        private static void OnClosedCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Window window))
                throw new InvalidOperationException("'WindowBehavior.ClosedCommand' only can be attached to a 'Window' object");

            window.Closed += Window_Closed;
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            var window = (DependencyObject) sender;
            var command = GetClosedCommand(window);
            var parameter = GetClosedCommandParameter(window);
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }

        /// <summary>
        ///     Gets the command from a window which get called when the window is loaded.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.LoadedCommand property value for the element.</returns>
        public static ICommand GetLoadedCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(LoadedCommandProperty);
        }

        /// <summary>
        ///     Attaches the command to a window which get called when the window is loaded.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.LoadedCommand value.</param>
        public static void SetLoadedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadedCommandProperty, value);
        }

        /// <summary>
        ///     Gets the command parameter from a window which is passed by the
        ///     DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.LoadedCommandParameter property value for the element.</returns>
        public static object GetLoadedCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(LoadedCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the command parameter from a window which is passed by the
        ///     DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.LoadedCommandParameter value.</param>
        public static void SetLoadedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(LoadedCommandParameterProperty, value);
        }

        private static void OnLoadedCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is FrameworkElement frameworkElement))
                throw new InvalidOperationException("'WindowBehavior.LoadedCommand' only can be attached to a 'FrameworkElement' object");

            frameworkElement.Loaded += FrameworkElement_Loaded;
        }

        private static void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var dependencyObject = (DependencyObject) sender;
            var command = GetLoadedCommand(dependencyObject);
            var parameter = GetLoadedCommandParameter(dependencyObject);
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }

        /// <summary>
        ///     Gets a value from a button that indicates that the window have to be closed when the button is pressed without
        ///     using the dialog result.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.IsClose property value for the element.</returns>
        public static bool GetIsClose(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsCloseProperty);
        }

        /// <summary>
        ///     Attaches a value from a button that indicates that the window have to be closed when the button is pressed without
        ///     using the dialog result.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.IsClose value.</param>
        public static void SetIsClose(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCloseProperty, value);
        }

        /// <summary>
        ///     Gets the IsClose command from a button to close the owner window.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.IsCloseCommand property value for the element.</returns>
        public static ICommand GetIsCloseCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(IsCloseCommandProperty);
        }

        /// <summary>
        ///     Attaches the IsClose command to a button to close the owner window.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.IsCloseCommand value.</param>
        public static void SetIsCloseCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(IsCloseCommandProperty, value);
        }

        private static void OnIsCloseCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ButtonBase button))
                throw new InvalidOperationException("'WindowBehavior.IsCloseCommand' only can be attached to a 'ButtonBase' object");

            button.Click += IsCloseButton_Click;
        }

        /// <summary>
        ///     Gets the IsClose command parameter passed with the IsCloseCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.IsCloseCommandParameter property value for the element.</returns>
        public static object GetIsCloseCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(IsCloseCommandParameterProperty);
        }

        /// <summary>
        ///     Attaches the IsClose command parameter to be passed with the IsCloseCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.IsCloseCommandParameter value.</param>
        public static void SetIsCloseCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(IsCloseCommandParameterProperty, value);
        }

        private static void IsCloseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var window = Window.GetWindow(button);
            if (window == null)
                return;

            var resultCommand = GetIsCloseCommand(button);
            var args = new WindowIsCloseArgs(GetIsCloseCommandParameter(button));
            if (resultCommand != null && resultCommand.CanExecute(args))
            {
                resultCommand.Execute(args);
                if (!args.Cancel)
                    window.Close();
            }
            else
            {
                if (GetIsClose(button))
                    window.Close();
            }
        }

        /// <summary>
        ///     Gets a list of hex values of a WinAPI messages to listen and forwarded to the WindowBehavior.WinApiCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.WinApiMessages property value for the element.</returns>
        public static string GetWinApiMessages(DependencyObject obj)
        {
            return (string) obj.GetValue(WinApiMessagesProperty);
        }

        /// <summary>
        ///     Attaches a list of hex values of a WinAPI messages to listen and forwarded to the WindowBehavior.WinApiCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.WinApiMessages value.</param>
        public static void SetWinApiMessages(DependencyObject obj, string value)
        {
            obj.SetValue(WinApiMessagesProperty, value);
        }

        private static void OnWinApiMessagesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var messages = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(messages))
                return;

            var observer = GetOrCreateObsever(sender);
            if (observer == null)
                return;

            observer.ClearCallbacks();

            if (messages.ToLower().Trim() == "all")
                observer.AddCallback(EventNotified);
            else
                foreach (var id in StringToIntegerList(messages))
                    observer.AddCallbackFor(id, EventNotified);
        }

        private static void EventNotified(NotifyEventArgs e)
        {
            var command = GetWinApiCommand(e.ObservedWindow);
            if (command != null && command.CanExecute(e))
                command.Execute(e);
        }

        private static IEnumerable<int> StringToIntegerList(string messages)
        {
            var idTexts = messages.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            var ids = new List<int>();
            foreach (var idText in idTexts)
                try
                {
                    ids.Add(int.TryParse(idText, NumberStyles.HexNumber, new CultureInfo(1033), out var id) ? id : Convert.ToInt32(idText, 16));
                }
                catch
                {
                    throw new ArgumentException("The attached WinApiMessages cannot be parsed to a list of integers. Supported are just integer numbers separated by a semicolon, e.g. '3;42' or hex values (base of 16) like '0x03;0x2A'.");
                }

            return ids;
        }

        /// <summary>
        ///     Gets a command which get called if one of the message attached by the WindowBehavior.WinApiMessages occurs.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The WindowBehavior.WinApiCommand property value for the element.</returns>
        public static ICommand GetWinApiCommand(DependencyObject obj)
        {
            return (ICommand) obj.GetValue(WinApiCommandProperty);
        }

        /// <summary>
        ///     Attaches a command which get called if one of the message attached by the WindowBehavior.WinApiMessages occurs.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed WindowBehavior.WinApiCommand value.</param>
        public static void SetWinApiCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(WinApiCommandProperty, value);
        }

        private static void OnWinApiCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                GetOrCreateObsever(sender);
        }

        private static WindowObserver GetOrCreateObsever(DependencyObject sender)
        {
            var observer = GetObserver(sender);
            if (observer == null)
                if (sender is Window window)
                {
                    observer = new WindowObserver(window);
                    SetObserver(sender, observer);
                }

            return observer;
        }

        private static WindowObserver GetObserver(DependencyObject obj)
        {
            return (WindowObserver) obj.GetValue(ObserverProperty);
        }

        private static void SetObserver(DependencyObject obj, WindowObserver value)
        {
            obj.SetValue(ObserverProperty, value);
        }
    }
}