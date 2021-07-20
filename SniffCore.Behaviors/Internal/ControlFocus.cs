//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SniffCore.Behaviors.Internal
{
    internal static class ControlFocus
    {
        internal static void GiveFocus(UIElement element)
        {
            element.Dispatcher.BeginInvoke(new Action(delegate
                {
                    element.Focus();
                    Keyboard.Focus(element);
                }),
                DispatcherPriority.Render);
        }

        internal static void GiveFocus(UIElement element, Action actionOnFocus)
        {
            element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    actionOnFocus();
                    element.Focus();
                    Keyboard.Focus(element);
                }),
                DispatcherPriority.Render);
        }
    }
}