// <copyright file="ParserOutput.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stubble.Core.Classes
{
    /// <summary>
    /// A base class representing output from the parser
    /// </summary>
    public class ParserOutput : IEquatable<ParserOutput>
    {
        /// <summary>
        /// Gets or sets the type of Token
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the value of the Token
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the start position of the Token in the string
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the end position of the Token in the string
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets or sets the child tokens for the Token
        /// </summary>
        public List<ParserOutput> ChildTokens { get; set; }

        /// <summary>
        /// Gets or sets the position of the Token's parent end section
        /// </summary>
        public int ParentSectionEnd { get; set; }

        /// <summary>
        /// An <see cref="IEquatable{ParserOutput}"/> method for comparing ParserOutputs
        /// </summary>
        /// <param name="other">The <see cref="ParserOutput"/> instance to compare against</param>
        /// <returns>If the two instances are equal</returns>
        public bool Equals(ParserOutput other)
        {
            if (!TokenType.Equals(other.TokenType))
            {
                return false;
            }

            if (!Value.Equals(other.Value))
            {
                return false;
            }

            if (!Start.Equals(other.Start))
            {
                return false;
            }

            if (!End.Equals(other.End))
            {
                return false;
            }

            if (!ParentSectionEnd.Equals(other.ParentSectionEnd))
            {
                return false;
            }

            if (ChildTokens != null && other.ChildTokens != null)
            {
                if (ChildTokens.Count != other.ChildTokens.Count)
                {
                    return false;
                }

                return !ChildTokens.Where((token, i) => !token.Equals(other.ChildTokens[i])).Any();
            }

            return !(ChildTokens == null & other.ChildTokens != null) && !(ChildTokens != null & other.ChildTokens == null);
        }

        /// <summary>
        /// Checks if the object is <see cref="ParserOutput"/> and passes into standard
        /// comparison if it is
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>If the two instances are equal</returns>
        public override bool Equals(object obj)
        {
            var a = obj as ParserOutput;
            return a != null && Equals(a);
        }

        /// <summary>
        /// Returns a unique enough hashcode for storing hashes of ParserOutput
        /// </summary>
        /// <returns>The hashcode for the instance</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) + TokenType.GetHashCode();
                hash = (13 * hash) + Value.GetHashCode();
                hash = (13 * hash) + Start.GetHashCode();
                hash = (13 * hash) + End.GetHashCode();
                hash = (13 * hash) + ParentSectionEnd.GetHashCode();
                return hash;
            }
        }
    }
}
