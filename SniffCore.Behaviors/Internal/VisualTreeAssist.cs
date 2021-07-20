//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Windows;
using System.Windows.Media;

namespace SniffCore.Behaviors.Internal
{
    internal static class VisualTreeAssist
    {
        internal static TChildType FindChild<TChildType>(DependencyObject item) where TChildType : DependencyObject
        {
            if (item == null)
                return default;

            var childrenCount = VisualTreeHelper.GetChildrenCount(item);
            for (var i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(item, i);
                if (child is TChildType childType)
                    return childType;

                var foundChild = FindChild<TChildType>(child);
                if (foundChild != null)
                    return foundChild;
            }

            return default;
        }
    }
}