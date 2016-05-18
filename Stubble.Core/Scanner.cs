// <copyright file="Scanner.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.RegularExpressions;

namespace Stubble.Core
{
    /// <summary>
    /// Represents the Scanner which takes a template and split it using Regex into strings
    /// which can be parsed into Tokens
    /// </summary>
    public sealed class Scanner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scanner"/> class.
        /// </summary>
        /// <param name="template">The template to scan</param>
        public Scanner(string template)
        {
            Template = template;
            Tail = template;
            Pos = 0;
        }

        /// <summary>
        /// Gets the template
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Gets the tail of the template
        /// </summary>
        public string Tail { get; private set; }

        /// <summary>
        /// Gets the index position where the scanner currently is in the template
        /// </summary>
        public int Pos { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the scanner has reached the end of the template
        /// </summary>
        public bool EOS => string.IsNullOrEmpty(Tail);

        /// <summary>
        /// Uses the expression to scan the tail of the template
        /// returning the match
        /// </summary>
        /// <param name="expression">The Regex expression to use on the tail</param>
        /// <returns>The matched string or <see cref="string.Empty"/> if none matched</returns>
        public string Scan(Regex expression)
        {
            var matched = expression.Match(Tail);

            if (!matched.Success || matched.Index != 0)
            {
                return string.Empty;
            }

            var result = matched.Value;
            Tail = Tail.Substring(result.Length);
            Pos += result.Length;

            return result;
        }

        /// <summary>
        /// Parses the expression into a Regex and uses it to scan the tail
        /// of the template
        /// </summary>
        /// <param name="expression">The Regex expression to use on the tail</param>
        /// <returns>The matched string or <see cref="string.Empty"/> if none matched</returns>
        public string Scan(string expression)
        {
            return Scan(new Regex(expression));
        }

        /// <summary>
        /// Scans the template until the Regex match and returns the string
        /// until the match
        /// </summary>
        /// <param name="expression">The Regex to use for scanning until matched</param>
        /// <returns>The template up until the matched string or <see cref="string.Empty"/> if none matched</returns>
        public string ScanUntil(Regex expression)
        {
            var regexMatch = expression.Match(Tail);
            var index = regexMatch.Index;
            string match;

            if (!regexMatch.Success)
            {
                match = Tail;
                Tail = string.Empty;
            }
            else if (index == 0)
            {
                match = string.Empty;
            }
            else
            {
                match = Tail.Substring(0, index);
                Tail = Tail.Substring(index);
            }

            Pos += match.Length;
            return match;
        }

        /// <summary>
        /// Turns the expression into a Regex and scans the template until the Regex match and returns the string
        /// until the match
        /// </summary>
        /// <param name="expression">The expression to turn into a Regex to use for scanning until matched</param>
        /// <returns>The template up until the matched string or <see cref="string.Empty"/> if none matched</returns>
        public string ScanUntil(string expression)
        {
            return ScanUntil(new Regex(expression));
        }
    }
}
