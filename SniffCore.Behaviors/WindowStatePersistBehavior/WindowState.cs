//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Windows;

// ReSharper disable CheckNamespace

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     A window size and position state.
    /// </summary>
    public sealed class WindowState
    {
        /// <summary>
        ///     Gets or sets the indicator if the window is fullscreen or not.
        /// </summary>
        public bool? IsFullscreen { get; set; }

        /// <summary>
        ///     Gets or sets the position of the window.
        /// </summary>
        public Point? Position { get; set; }

        /// <summary>
        ///     Gets or sets the size of the window.
        /// </summary>
        public Size? Size { get; set; }
    }
}