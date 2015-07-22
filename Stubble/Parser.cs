using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Stubble.Core.Classes;
using Stubble.Core.Classes.Exceptions;
using Stubble.Core.Classes.Tokens;

namespace Stubble.Core
{
    public sealed class Parser
    {
        #region Static Regex
        private static readonly Regex WhitespaceRegex = new Regex(@"\s*", RegexOptions.Compiled);
        private static readonly Regex SpaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex EqualsRegex = new Regex(@"\s*=", RegexOptions.Compiled);
        private static readonly Regex CurlyRegex = new Regex(@"\s*\}", RegexOptions.Compiled);
        private static readonly Regex EscapeRegex = new Regex(@"[\-\[\]{}()*+?.,\^$|#\s]", RegexOptions.Compiled);
        #endregion

        #region Static Tag Cache
        internal static readonly ConcurrentDictionary<string, TagRegexes> TagRegexCache = new ConcurrentDictionary<string, TagRegexes>
        (
            new Dictionary<string, TagRegexes>
            {
                { "{{ }}", new TagRegexes()
                    {
                        OpenTag = new Regex(@"\{\{\s*"),
                        CloseTag = new Regex(@"\s*\}\}"),
                        ClosingTag = new Regex(@"\s*\}\}\}")
                    }
                }
            }
        );

        internal static int regexCacheSize = 4;
        public static int RegexCacheSize
        {
            get { return regexCacheSize; }
            set
            {
                regexCacheSize = value;
                if (TagRegexCache.Count <= regexCacheSize) return;
                while (TagRegexCache.Count > regexCacheSize)
                {
                    TagRegexes outVal;
                    TagRegexCache.TryRemove(TagRegexCache.Last().Key, out outVal);
                }
            }
        }

        internal struct TagRegexes
        {
            internal Regex OpenTag { get; set; }
            internal Regex CloseTag { get; set; }
            internal Regex ClosingTag { get; set; }
        }
        #endregion

        public static readonly Tags DefaultTags = new Tags("{{", "}}");

        private Regex _openingTagRegex;
        private Regex _closingTagRegex;
        private Regex _closingCurlyRegex;
        private Tags _currentTags;
        private readonly Registry _registry;

        public Parser(Registry registry)
        {
            _registry = registry;
        }

        public IList<ParserOutput> ParseTemplate(string template)
        {
            return ParseTemplate(template, null);
        }

        public IList<ParserOutput> ParseTemplate(string template, Tags tags)
        {
            if (string.IsNullOrEmpty(template))
                return new List<ParserOutput>();

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

                var value = scanner.ScanUntil(_openingTagRegex);

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

                        if (c != '\n') continue;
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

                if (string.IsNullOrEmpty(scanner.Scan(_openingTagRegex)))
                    break;

                hasTag = true;

                var type = scanner.Scan(_registry.TokenMatchRegex);
                type = string.IsNullOrEmpty(type) ? "name" : type;
                scanner.Scan(WhitespaceRegex);

                switch (type)
                {
                    case "=":
                        value = scanner.ScanUntil(EqualsRegex);
                        scanner.Scan(EqualsRegex);
                        scanner.ScanUntil(_closingTagRegex);
                        break;
                    case "{":
                        value = scanner.ScanUntil(_closingCurlyRegex);
                        scanner.Scan(CurlyRegex);
                        scanner.ScanUntil(_closingTagRegex);
                        type = "&";
                        break;
                    default:
                        value = scanner.ScanUntil(_closingTagRegex);
                        break;
                }

                if (string.IsNullOrEmpty(scanner.Scan(_closingTagRegex)))
                {
                    throw new Exception("Unclosed Tag at " + scanner.Pos);
                }

                var token = GetCorrectTypedToken(type, _currentTags);
                token.Value = value;
                token.Start = start;
                token.End = scanner.Pos;
                tokens.Add(token);

                if (token is ISection)
                {
                    sections.Push(token);
                }
                else if(token is INonSpace)
                {
                    nonSpace = true;
                }
                else switch (type)
                {
                    case "/":
                        if (sections.Count == 0)
                        {
                            throw new StubbleException("Unopened Section '" + value + "' at " + start);
                        }
                        openSection = sections.Pop();

                        if (openSection.Value != token.Value)
                        {
                            throw new StubbleException("Unclosed Section '" + openSection.Value + "' at " + start);
                        }
                        break;
                    case "=":
                        CompileTags(value);
                        break;
                }
            }

            // Make sure there are no open sections when we're done.
            if (sections.Count > 0)
            {
                openSection = sections.Pop();
                throw new StubbleException("Unclosed Section '" + openSection.Value + "' at " + scanner.Pos);
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
            CompileTags(new Tags(SpaceRegex.Split(value)));
        }

        private void CompileTags(Tags tags)
        {
            _currentTags = tags;
            TagRegexes tagRegexes;
            var tagString = tags.ToString();
            if (!TagRegexCache.TryGetValue(tagString, out tagRegexes))
            {
                tagRegexes = new TagRegexes()
                {
                    OpenTag = new Regex(EscapeRegexExpression(tags.StartTag) + @"\s*"),
                    CloseTag = new Regex(@"\s*" + EscapeRegexExpression(tags.EndTag)),
                    ClosingTag = new Regex(@"\s*" + EscapeRegexExpression("}" + tags.EndTag))
                };
                AddToRegexCache(tagString, tagRegexes);
            }

            _openingTagRegex = tagRegexes.OpenTag;
            _closingTagRegex = tagRegexes.CloseTag;
            _closingCurlyRegex = tagRegexes.ClosingTag;
        }

        public static string EscapeRegexExpression(string expression)
        {
            return EscapeRegex.Replace(expression, @"\$&");
        }

        private ParserOutput GetCorrectTypedToken(string tokenType, Tags currentTags)
        {
            return _registry.TokenGetters.ContainsKey(tokenType) ?
                _registry.TokenGetters[tokenType](tokenType, currentTags)
                : new ParserOutput { TokenType = tokenType };
        }

        internal static void AddToRegexCache(string dictionaryKey, TagRegexes regex)
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
