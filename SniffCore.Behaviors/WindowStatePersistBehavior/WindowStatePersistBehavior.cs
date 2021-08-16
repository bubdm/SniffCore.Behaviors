//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.ComponentModel;
using System.Windows;
using SniffCore.Behaviors.Internal;

// ReSharper disable CheckNamespace

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Applies window state, position and size after its loaded and back when its closing.
    /// </summary>
    /// <example>
    ///     <code lang="XAML">
    /// <![CDATA[
    /// <Window controls:WindowStatePersistBehavior.WindowState="{Binding WindowState}"
    ///         controls:WindowBehavior.ClosedCommand="{Binding ClosedCommand}">
    /// </Window>
    /// ]]>
    /// </code>
    ///     <code lang="csharp">
    /// <![CDATA[
    /// public class ViewModel : ObservableObject, IAsyncLoader
    /// {
    ///     private WindowState _windowState;
    /// 
    ///     private ViewModel()
    ///     {
    ///         ClosedCommand = new AsyncDelegateCommand(ClosedAsync);
    ///     }
    /// 
    ///     public IDelegateCommand ClosedCommand;
    /// 
    ///     public WindowState WindowState
    ///     {
    ///         get => _windowState;
    ///         set => NotifyAndSetIfChanged(ref _windowState, value);
    ///     }
    /// 
    ///     public async Task LoadAsync()
    ///     {
    ///         WindowState = await _loader.GetSettings();
    ///     }
    /// 
    ///     public async Task ClosedAsync()
    ///     {
    ///         await _loader.SaveSettings(WindowState);
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public sealed class WindowStatePersistBehavior
    {
        /// <summary>
        ///     Identifies the <see cref="GetWindowState(DependencyObject)" />
        ///     <see cref="SetWindowState(DependencyObject, WindowState)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty WindowStateProperty =
            DependencyProperty.RegisterAttached("WindowState", typeof(WindowState), typeof(WindowStatePersistBehavior), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnWindowStateChanged));

        private static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached("Behavior", typeof(WindowStatePersistBehavior), typeof(WindowStatePersistBehavior), new PropertyMetadata(null));

        private readonly Window _window;
        private WorkingIndicator _selfSet;

        private WindowStatePersistBehavior(Window window)
        {
            _window = window;
        }

        /// <summary>
        ///     Gets the state applied to the window.
        /// </summary>
        /// <param name="obj">The window which state to read.</param>
        /// <returns>The state applied to the window.</returns>
        public static WindowState GetWindowState(DependencyObject obj)
        {
            return (WindowState) obj.GetValue(WindowStateProperty);
        }

        /// <summary>
        ///     Sets the state applied to the window.
        /// </summary>
        /// <param name="obj">The window which state to set.</param>
        /// <param name="value">he state to apply to the window.</param>
        public static void SetWindowState(DependencyObject obj, WindowState value)
        {
            obj.SetValue(WindowStateProperty, value);
        }

        private static void OnWindowStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Window window))
                throw new InvalidOperationException("The WindowStatePersistBehavior.WindowState can be attached to a Window only.");

            var behavior = GetBehavior(window);
            if (behavior == null)
                behavior = new WindowStatePersistBehavior(window);

            behavior.Update(e.NewValue as WindowState, e.NewValue as WindowState);
        }

        private void Update(WindowState oldWindowState, WindowState newWindowState)
        {
            if (WorkingIndicator.IsActive(_selfSet))
                return;

            if (!_window.IsLoaded)
            {
                _window.Loaded += OnWindowLoaded;
                return;
            }

            if (oldWindowState != null)
                _window.Closing -= OnWindowClosing;

            if (newWindowState != null)
            {
                Apply(newWindowState);
                _window.Closing += OnWindowClosing;
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _window.Loaded -= OnWindowLoaded;
            Apply(GetWindowState(_window));
        }

        private void Apply(WindowState newValue)
        {
            if (newValue == null)
                return;

            if (newValue.Position.HasValue)
            {
                _window.Left = newValue.Position.Value.X;
                _window.Top = newValue.Position.Value.Y;
            }

            if (newValue.Size.HasValue)
            {
                _window.Height = newValue.Size.Value.Height;
                _window.Width = newValue.Size.Value.Width;
            }

            if (newValue.IsFullscreen.HasValue)
                _window.WindowState = newValue.IsFullscreen.Value ? System.Windows.WindowState.Maximized : System.Windows.WindowState.Normal;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var newState = new WindowState
            {
                IsFullscreen = _window.WindowState == System.Windows.WindowState.Maximized,
                Position = new Point(_window.Left, _window.Top),
                Size = new Size(_window.Height, _window.Height)
            };

            using (_selfSet = new WorkingIndicator())
            {
                SetWindowState(_window, newState);
            }
        }

        private static WindowStatePersistBehavior GetBehavior(DependencyObject obj)
        {
            return (WindowStatePersistBehavior) obj.GetValue(BehaviorProperty);
        }

        private static void SetBehavior(DependencyObject obj, WindowStatePersistBehavior value)
        {
            obj.SetValue(BehaviorProperty, value);
        }
    }
}