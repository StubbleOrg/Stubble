// <copyright file="RenderSettings.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Stubble.Core.Classes
{
    public class RenderSettings
    {
        public bool SkipRecursiveLookup { get; set; }
        public bool ThrowOnDataMiss { get; set; }

        public static RenderSettings GetDefaultRenderSettings()
        {
            return new RenderSettings
            {
                SkipRecursiveLookup = false,
                ThrowOnDataMiss = false
            };
        }
    }
}
