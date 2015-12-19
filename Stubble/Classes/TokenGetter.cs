// <copyright file="TokenGetter.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Classes
{
    public struct TokenGetter
    {
        public string TokenType { get; set; }

        public Func<string, Tags, ParserOutput> Getter { get; set; }
    }
}
