//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System;

namespace SniffCore.Behaviors.Internal
{
    internal class WorkingIndicator : IDisposable
    {
        private bool _flag;

        internal WorkingIndicator()
        {
            _flag = true;
        }

        public void Dispose()
        {
            _flag = false;
        }

        internal static bool IsActive(WorkingIndicator indicator)
        {
            return indicator?._flag == true;
        }
    }
}