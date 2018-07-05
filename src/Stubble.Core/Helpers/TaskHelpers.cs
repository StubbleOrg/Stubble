// <copyright file="TaskHelpers.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;

namespace Stubble.Core.Helpers
{
    /// <summary>
    /// Helpers for tasks
    /// </summary>
    public static class TaskHelpers
    {
        /// <summary>
        /// Gets a static completed task
        /// </summary>
        public static Task CompletedTask { get; }
            =
#if NETSTANDARD2_0
            Task.CompletedTask;
#else
            Task.FromResult(true);
#endif
    }
}
