//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace SniffCore.Behaviors.Internal
{
    internal class WindowObserver
    {
        private readonly List<Callback> _callbacks;
        private readonly Window _observedWindow;

        internal WindowObserver(Window observedWindow)
        {
            _callbacks = new List<Callback>();

            _observedWindow = observedWindow ?? throw new ArgumentNullException(nameof(observedWindow));
            if (!observedWindow.IsLoaded)
                observedWindow.Loaded += WindowLoaded;
            else
                HookIn();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ((Window) sender).Loaded -= WindowLoaded;

            HookIn();
        }

        private void HookIn()
        {
            var handle = new WindowInteropHelper(_observedWindow).Handle;
            HwndSource.FromHwnd(handle).AddHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            NotifyCallbacks(msg);

            return (IntPtr) 0;
        }

        internal void AddCallback(Action<NotifyEventArgs> callback)
        {
            AddCallbackFor(null, callback);
        }

        internal void AddCallbackFor(int? messageId, Action<NotifyEventArgs> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            _callbacks.Add(new Callback(messageId, callback));
        }

        private void NotifyCallbacks(int message)
        {
            for (var i = 0; i < _callbacks.Count; i++)
                if (_callbacks[i].ListenMessageId == null ||
                    _callbacks[i].ListenMessageId == message)
                    _callbacks[i].Action(new NotifyEventArgs(_observedWindow, message));
        }

        internal void ClearCallbacks()
        {
            _callbacks.Clear();
        }
    }
}