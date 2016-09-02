// <copyright file="Parser.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Classes.Tokens;
using Stubble.Core.Classes.Tokens.Interface;

namespace Stubble.Core
{
    /// <summary>
    /// Represents the Parser of Mustache Templates
    /// </summary>
    public sealed class Parser
    {
        private Regex openingTagRegex;
        private Regex closingTagRegex;
        private Regex closingCurlyRegex;
        private Tags currentTags;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="cacheLimit">The max size of the template token cache</param>
        /// <param name="registry">The registry instance to use</param>
        public Parser(uint cacheLimit, Registry registry)
        {
            Registry = registry;
            Cache = new LimitedSizeConcurrentDictionary<string, IList<ParserOutput>>((int)cacheLimit);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class with a cache size of 15
        /// </summary>
        /// <param name="registry">The registry instance to use</param>
        public Parser(Registry registry)
            : this(15, registry)
        {
        }

        /// <summary>
        /// Gets the default <see cref="Tags"/> instance
        /// </summary>
        public static Tags DefaultTags { get; } = new Tags("{{", "}}");

        /// <summary>
        /// Gets the Template Token Cache
        /// </summary>
        internal LimitedSizeConcurrentDictionary<string, IList<ParserOutput>> Cache { get; }

        private Registry Registry { get; }

        /// <summary>
        /// Takes a given expression and escapes it
        /// </summary>
        /// <param name="expression">The expression to escape</param>
        /// <returns>The escaped expression</returns>
        public static string EscapeRegexExpression(string expression)
        {
            return ParserStatic.EscapeRegex.Replace(expression, @"\$&");
        }

        /// <summary>
        /// Parses a template, looks up the template in the
        /// template token cache and returns cached version if possible.
        /// </summary>
        /// <param name="template">The template to parse</param>
        /// <returns>A list of tokens parsed from the template</returns>
        public IList<ParserOutput> Parse(string template) => Parse(template, null);

        /// <summary>
        /// Parses a template with the given tags, looks up the template in the
        /// template token cache and returns cached version if possible.
        /// </summary>
        /// <param name="template">The template to parse</param>
        /// <param name="tags">The tags to parse the template with</param>
        /// <returns>A list of tokens parsed from the template</returns>
        public IList<ParserOutput> Parse(string template, Tags tags)
        {
            IList<ParserOutput> tokens;
            var success = Cache.TryGetValue(template, out tokens);
            if (!success)
            {
                tokens = Cache[template] = ParseTemplate(template, tags);
            }

            return tokens;
        }

        /// <summary>
        /// Parses the template into a list of tokens
        /// </summary>
        /// <param name="template">The template to parse</param>
        /// <returns>A list of tokens representing the template</returns>
        public IList<ParserOutput> ParseTemplate(string template)
        {
            return ParseTemplate(template, null);
        }

        /// <summary>
        /// Parses the template with the given tags
        /// </summary>
        /// <param name="template">The template to parse</param>
        /// <param name="tags">The default tags to use during parsing</param>
        /// <exception cref="StubbleException">Unclosed Tag/Unclosed Section/Unopened Section</exception>
        /// <returns>A list of tokens representing the template</returns>
        public IList<ParserOutput> ParseTemplate(string template, Tags tags)
        {
            if (string.IsNullOrEmpty(template))
            {
                return new List<ParserOutput>();
            }

            CompileTags(tags ?? DefaultTags);

            var scanner = new Scanner(template);
            var sections = new Stack<ParserOutput>();
            ParserOutput openSection;
            var tokens = new List<ParserOutput>();
            var spaces = new Stack<int>();
            var hasTag = false;
            var nonSpace = false;
            while (!scanner.EOS)
            {
                var start = scanner.Pos;

                var value = scanner.ScanUntil(openingTagRegex);

                if (!string.IsNullOrEmpty(value))
                {
                    foreach (var c in value)
                    {
                        if (char.IsWhiteSpace(c))
                        {
                            spaces.Push(tokens.Count);
                        }
                        else
                        {
                            nonSpace = true;
                        }

                        var textToken = GetCorrectTypedToken("text", tags);
                        textToken.Value = c.ToString();
                        textToken.Start = start;
                        textToken.End = start + 1;
                        tokens.Add(textToken);
                        start += 1;

                        if (c != '\n')
                        {
                            continue;
                        }

                        if (hasTag && !nonSpace)
                        {
                            while (spaces.Count > 0)
                            {
                                tokens.RemoveAt(spaces.Pop());
                            }
                        }
                        else
                        {
                            spaces = new Stack<int>();
                        }

                        hasTag = false;
                        nonSpace = false;
                    }
                }

                if (string.IsNullOrEmpty(scanner.Scan(openingTagRegex)))
                {
                    break;
                }

                hasTag = true;

                var type = scanner.Scan(Registry.TokenMatchRegex);
                type = string.IsNullOrEmpty(type) ? "name" : type;
                scanner.Scan(ParserStatic.WhitespaceRegex);

                switch (type)
                {
                    case "=":
                        value = scanner.ScanUntil(ParserStatic.EqualsRegex);
                        scanner.Scan(ParserStatic.EqualsRegex);
                        scanner.ScanUntil(closingTagRegex);
                        break;
                    case "{":
                        value = scanner.ScanUntil(closingCurlyRegex);
                        scanner.Scan(ParserStatic.CurlyRegex);
                        scanner.ScanUntil(closingTagRegex);
                        type = "&";
                        break;
                    default:
                        value = scanner.ScanUntil(closingTagRegex);
                        break;
                }

                if (string.IsNullOrEmpty(scanner.Scan(closingTagRegex)))
                {
                    throw new StubbleException($"Unclosed Tag at {scanner.Pos}");
                }

                var token = GetCorrectTypedToken(type, currentTags);
                token.Value = value;
                token.Start = start;
                token.End = scanner.Pos;
                tokens.Add(token);

                if (token is ISection)
                {
                    sections.Push(token);
                }
                else if (token is INonSpace)
                {
                    nonSpace = true;
                }
                else
                {
                    switch (type)
                    {
                        case "/":
                            if (sections.Count == 0)
                            {
                                throw new StubbleException($"Unopened Section '{value}' at {start}");
                            }

                            openSection = sections.Pop();

                            if (openSection.Value != token.Value)
                            {
                                throw new StubbleException($"Unclosed Section '{openSection.Value}' at {start}");
                            }

                            break;
                        case "=":
                            CompileTags(value);
                            break;
                    }
                }
            }

            // Make sure there are no open sections when we're done.
            if (sections.Count > 0)
            {
                openSection = sections.Pop();
                throw new StubbleException($"Unclosed Section '{openSection.Value}' at {scanner.Pos}");
            }

            return NestTokens(SquishTokens(tokens));
        }

        private static IEnumerable<ParserOutput> SquishTokens(IEnumerable<ParserOutput> tokens)
        {
            using (var iterator = tokens.GetEnumerator())
            {
                RawValueToken lastItem = null;
                while (iterator.MoveNext())
                {
                    var item = iterator.Current;
                    if (lastItem != null && item is RawValueToken)
                    {
                        lastItem.ValueBuilder.Append(item.Value);
                        lastItem.End = item.End;
                        continue;
                    }

                    lastItem = item as RawValueToken;
                    yield return item;
                }
            }
        }

        private static IList<ParserOutput> NestTokens(IEnumerable<ParserOutput> tokens)
        {
            var nestedTokens = new List<ParserOutput>();
            var collector = nestedTokens;
            var sections = new Stack<ParserOutput>();

            foreach (var token in tokens)
            {
                if (token is ISection)
                {
                    collector.Add(token);
                    sections.Push(token);
                    collector = token.ChildTokens = new List<ParserOutput>();
                }
                else if (token.TokenType == "/")
                {
                    var section = sections.Pop();
                    section.ParentSectionEnd = token.Start;
                    collector = sections.Count > 0 ? sections.Peek().ChildTokens : nestedTokens;
                }
                else
                {
                    collector.Add(token);
                }
            }

            return nestedTokens;
        }

        private void CompileTags(string value)
        {
            CompileTags(new Tags(ParserStatic.SpaceRegex.Split(value)));
        }

        private void CompileTags(Tags tags)
        {
            currentTags = tags;
            TagRegexes tagRegexes;
            var tagString = tags.ToString();
            if (!ParserStatic.TagRegexCache.TryGetValue(tagString, out tagRegexes))
            {
                tagRegexes = new TagRegexes(
                    new Regex(EscapeRegexExpression(tags.StartTag) + @"\s*"),
                    new Regex(@"\s*" + EscapeRegexExpression(tags.EndTag)),
                    new Regex(@"\s*" + EscapeRegexExpression("}" + tags.EndTag)));
                ParserStatic.AddToRegexCache(tagString, tagRegexes);
            }

            openingTagRegex = tagRegexes.OpenTag;
            closingTagRegex = tagRegexes.CloseTag;
            closingCurlyRegex = tagRegexes.ClosingTag;
        }

        private ParserOutput GetCorrectTypedToken(string tokenType, Tags tags)
        {
            return Registry.TokenGetters.ContainsKey(tokenType) ?
                Registry.TokenGetters[tokenType](tokenType, tags)
                : new ParserOutput { TokenType = tokenType };
        }

        /// <summary>
        /// Represents a structure to store the regexes to find Tags Open,
        /// Close and Closing tokens
        /// </summary>
        internal struct TagRegexes : IEquatable<TagRegexes>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TagRegexes"/> struct.
            /// </summary>
            /// <param name="openTag">The open tag Regex</param>
            /// <param name="closeTag">The close tag Regex</param>
            /// <param name="closingTag">The closing tag Regex</param>
            public TagRegexes(Regex openTag, Regex closeTag, Regex closingTag)
            {
                OpenTag = openTag;
                CloseTag = closeTag;
                ClosingTag = closingTag;
            }

            /// <summary>
            /// Gets the OpenTag Regex
            /// </summary>
            internal Regex OpenTag { get; }

            /// <summary>
            /// Gets the CloseTag Regex
            /// </summary>
            internal Regex CloseTag { get; }

            /// <summary>
            /// Gets the ClosingTag Regex
            /// </summary>
            internal Regex ClosingTag { get; }

            /// <summary>
            /// Determines whether the specified TagRegexes's are considered equal
            /// </summary>
            /// <param name="other">The TagRegexes to compare with the current instance</param>
            /// <returns>true if both instances are the same; or false if not</returns>
            public bool Equals(TagRegexes other)
            {
                return Equals(OpenTag, other.OpenTag) && Equals(CloseTag, other.CloseTag) && Equals(ClosingTag, other.ClosingTag);
            }

            /// <summary>
            /// Determines whether the specified object is considered equal to the
            /// current TagRegexes instance
            /// </summary>
            /// <param name="obj">The object to compare with the current instance</param>
            /// <returns>true if object is a tag regex and equal to instance; false if not</returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                return obj is TagRegexes && Equals((TagRegexes)obj);
            }

            /// <summary>
            /// A hash function for the structure
            /// </summary>
            /// <returns>The hashcode for the object</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = OpenTag?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ (CloseTag?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (ClosingTag?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }
        }

        /// <summary>
        /// Represents a collection of static parser fields and functions
        /// </summary>
        internal static class ParserStatic
        {
            private static int regexCacheSize = 4;

            /// <summary>
            /// Gets the Whitespace Regex
            /// </summary>
            public static Regex WhitespaceRegex { get; } = new Regex(@"\s*", RegexOptions.Compiled);

            /// <summary>
            /// Gets the Space Regex
            /// </summary>
            public static Regex SpaceRegex { get; } = new Regex(@"\s+", RegexOptions.Compiled);

            /// <summary>
            /// Gets the Equals Regex
            /// </summary>
            public static Regex EqualsRegex { get; } = new Regex(@"\s*=", RegexOptions.Compiled);

            /// <summary>
            /// Gets the Curly Brace Regex
            /// </summary>
            public static Regex CurlyRegex { get; } = new Regex(@"\s*\}", RegexOptions.Compiled);

            /// <summary>
            /// Gets the Escape Regex
            /// </summary>
            public static Regex EscapeRegex { get; } = new Regex(@"[\-\[\]{}()*+?.,\^$|#\s]", RegexOptions.Compiled);

            /// <summary>
            /// Gets the Tag Regex Cache for parsing Tokens
            /// </summary>
            public static ConcurrentDictionary<string, TagRegexes> TagRegexCache { get; } = new ConcurrentDictionary<string, TagRegexes>(
                new Dictionary<string, TagRegexes>
                {
                    {
                        "{{ }}", new TagRegexes(
                            new Regex(@"\{\{\s*", RegexOptions.Compiled),
                            new Regex(@"\s*\}\}", RegexOptions.Compiled),
                            new Regex(@"\s*\}\}\}", RegexOptions.Compiled))
                    }
                });

            /// <summary>
            /// Gets or sets the regex cache size, if value is set to less than
            /// <see cref="TagRegexCache"/> size then TagRegexes are removed from
            /// last forwards
            /// </summary>
            public static int RegexCacheSize
            {
                get
                {
                    return regexCacheSize;
                }

                set
                {
                    regexCacheSize = value;
                    if (TagRegexCache.Count <= regexCacheSize)
                    {
                        return;
                    }

                    while (TagRegexCache.Count > regexCacheSize)
                    {
                        TagRegexes outVal;
                        TagRegexCache.TryRemove(TagRegexCache.Last().Key, out outVal);
                    }
                }
            }

            /// <summary>
            /// Add a TagRegex to the TagRegexCache
            /// </summary>
            /// <param name="dictionaryKey">the key for the TagRegexes</param>
            /// <param name="regex">The Tag Regexes to cache</param>
            public static void AddToRegexCache(string dictionaryKey, TagRegexes regex)
            {
                if (TagRegexCache.Count >= regexCacheSize)
                {
                    TagRegexes outValue;
                    TagRegexCache.TryRemove(TagRegexCache.Last().Key, out outValue);
                }

                TagRegexCache.AddOrUpdate(dictionaryKey, regex, (key, existingVal) => regex);
            }
        }
    }
}
