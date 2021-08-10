//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

// ReSharper disable CheckNamespace

namespace SniffCore.Behaviors
{
    /// <summary>
    ///     Defines when the tooltip has to be created by the <see cref="CutTooltipBehavior" />.
    /// </summary>
    public enum CutTextKind
    {
        /// <summary>
        ///     The auto tooltip is disabled.
        /// </summary>
        None,

        /// <summary>
        ///     The tooltip appears when the text is longer than the available space.
        /// </summary>
        Width,

        /// <summary>
        ///     The tooltip appears when the text height is higher than the available space.
        /// </summary>
        Height,

        /// <summary>
        ///     The tooltip appears when the text length and height is more than the available space.
        /// </summary>
        WithAndHeight
    }
}