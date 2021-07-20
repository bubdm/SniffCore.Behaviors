//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Holds the information if the window can be closed or not. This object is used by the <see cref="WindowBehavior" />.
    /// </summary>
    public class WindowDialogResultArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowDialogResultArgs" /> class.
        /// </summary>
        public WindowDialogResultArgs(object parameter)
        {
            Parameter = parameter;
            DialogResult = true;
        }

        /// <summary>
        ///     Gets or sets the value which indicates how to close the dialog if <see cref="Cancel" /> is false. The default is
        ///     true.
        /// </summary>
        public bool DialogResult { get; set; }

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