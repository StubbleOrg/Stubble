using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stubble.Core.Imported;
using Stubble.Core.Tokens;
using Xunit;

namespace Stubble.Core.Tests.Tokens
{
    public class TokenTest
    {
        [Fact]
        public void SectionToken_Uniqueness()
        {
            var str = new StringSlice("foo");
            var literal = new LiteralToken
            {
                Content = new[] { str },
                ContentStartPosition = 0,
                ContentEndPosition = 2,
                Indent = 0,
                IsClosed = true,
                IsWhitespace = false
            };

            var blockToken = new SectionToken();
            blockToken.Children.Add(literal);

            var blockToken2 = new SectionToken();
            blockToken2.Children.Add(literal);

            Assert.False(blockToken.Equals(null));
            Assert.StrictEqual(blockToken, blockToken2);
            Assert.Equal(blockToken.GetHashCode(), blockToken2.GetHashCode());
        }

        [Fact]
        public void CommentToken_Uniqueness()
        {
            var comment = new CommentToken
            {
                Content = new StringSlice("comment"),
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 6,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 6
            };

            var comment2 = new CommentToken
            {
                Content = new StringSlice("comment"),
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 6,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 6
            };

            Assert.False(comment.Equals(null));
            Assert.Equal(comment, comment2);
            Assert.Equal(comment.GetHashCode(), comment2.GetHashCode());
        }

        [Fact]
        public void DelimiterToken_Uniqueness()
        {
            var delimiterToken = new DelimiterToken
            {
                StartTag = "{{",
                EndTag = "}}",
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 6,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 6
            };

            var delimiterToken2 = new DelimiterToken
            {
                StartTag = "{{",
                EndTag = "}}",
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 6,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 6
            };

            Assert.False(delimiterToken.Equals(null));
            Assert.Equal(delimiterToken, delimiterToken2);
            Assert.Equal(delimiterToken.GetHashCode(), delimiterToken2.GetHashCode());
        }

        [Fact]
        public void InterpolationToken_Uniqueness()
        {
            var interpolationToken = new InterpolationToken
            {
                Content = new StringSlice("foo"),
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 2,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 2
            };

            var interpolationToken2 = new InterpolationToken
            {
                Content = new StringSlice("foo"),
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 2,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 2
            };

            Assert.False(interpolationToken.Equals(null));
            Assert.Equal(interpolationToken, interpolationToken2);
            Assert.Equal(interpolationToken.GetHashCode(), interpolationToken2.GetHashCode());
        }

        [Fact]
        public void InvertedSectionToken_Uniqueness()
        {
            var str = new StringSlice("foo");
            var literal = new LiteralToken
            {
                Content = new[] { str },
                ContentStartPosition = 0,
                ContentEndPosition = 2,
                Indent = 0,
                IsClosed = true,
                IsWhitespace = false
            };

            var blockToken = new InvertedSectionToken
            {
                SectionName = "section"
            };
            blockToken.Children.Add(literal);

            var blockToken2 = new InvertedSectionToken
            {
                SectionName = "section"
            };
            blockToken2.Children.Add(literal);

            Assert.Equal("section", blockToken.Identifier);
            Assert.False(blockToken.Equals(null));
            Assert.StrictEqual(blockToken, blockToken2);
            Assert.Equal(blockToken.GetHashCode(), blockToken2.GetHashCode());
        }

        [Fact]
        public void LiteralToken_Uniqueness()
        {
            var str = new StringSlice("foo");
            var literal = new LiteralToken
            {
                Content = new[] { str },
                ContentStartPosition = 0,
                ContentEndPosition = 2,
                Indent = 0,
                IsClosed = true,
                IsWhitespace = false
            };

            var literal2 = new LiteralToken
            {
                Content = new[] { str },
                ContentStartPosition = 0,
                ContentEndPosition = 2,
                Indent = 0,
                IsClosed = true,
                IsWhitespace = false
            };

            var literal3 = new LiteralToken
            {
                Content = new[] { new StringSlice("foobar") },
                ContentStartPosition = 0,
                ContentEndPosition = 5,
                Indent = 0,
                IsClosed = true,
                IsWhitespace = false
            };

            Assert.False(literal.Equals(null));
            Assert.Equal(literal, literal2);
            Assert.NotEqual(literal, literal3);
            Assert.Equal(literal.GetHashCode(), literal2.GetHashCode());
            Assert.NotEqual(literal.GetHashCode(), literal3.GetHashCode());
        }

        [Fact]
        public void PartialToken_Uniqueness()
        {
            var partialToken = new PartialToken
            {
                Content = new StringSlice("partial"),
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 6,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 6
            };

            var partialToken2 = new PartialToken
            {
                Content = new StringSlice("partial"),
                Indent = 0,
                ContentStartPosition = 0,
                ContentEndPosition = 6,
                IsClosed = true,
                TagStartPosition = 0,
                TagEndPosition = 6
            };

            Assert.False(partialToken.Equals(null));
            Assert.Equal(partialToken, partialToken2);
            Assert.Equal(partialToken.GetHashCode(), partialToken2.GetHashCode());
        }
    }
}
