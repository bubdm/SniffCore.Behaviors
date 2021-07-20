//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;
using System.Windows;

namespace SniffCore.Behaviors.Internal
{
    internal sealed class NotifyEventArgs : EventArgs
    {
        internal NotifyEventArgs(Window observedWindow, int messageId)
        {
            ObservedWindow = observedWindow;
            MessageId = messageId;
        }

        internal Window ObservedWindow { get; }

        internal int MessageId { get; }
    }
}