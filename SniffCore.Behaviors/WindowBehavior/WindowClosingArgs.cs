//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;

// ReSharper disable CheckNamespace

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Holds the information if the window can be closed or not. This object is used by the <see cref="WindowBehavior" />.
    /// </summary>
    public class WindowClosingArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowClosingArgs" /> class.
        /// </summary>
        public WindowClosingArgs(object parameter)
        {
            Parameter = parameter;
        }

        /// <summary>
        ///     Gets or sets the value to define if the closing process has to be canceled and the window to stay open.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        ///     Gets the additional command parameter.
        /// </summary>
        public object Parameter { get; }
    }
}