//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;

namespace SniffCore.Behaviors.Internal
{
    internal class Callback
    {
        internal Callback(int? listenMessageId, Action<NotifyEventArgs> callback)
        {
            Action = callback;
            ListenMessageId = listenMessageId;
        }

        internal Action<NotifyEventArgs> Action { get; }

        internal int? ListenMessageId { get; }
    }
}