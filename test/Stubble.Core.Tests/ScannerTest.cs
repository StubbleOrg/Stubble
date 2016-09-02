// <copyright file="ScannerTest.cs" company="Stubble Authors">
// Copyright (c) Stubble Authors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Xunit;

namespace Stubble.Core.Tests
{
    public class ScannerTest
    {
        [Fact]
        public void It_Can_Be_Instantiated()
        {
            var scanner = new Scanner("");
            Assert.True(scanner.EOS);
        }

        [Fact]
        public void It_Should_Match_Whole_String()
        {
            var scanner = new Scanner("a b c");
            var match = scanner.Scan("a b c");
            Assert.Equal(match, scanner.Template);
            Assert.True(scanner.EOS);
        }

        [Fact]
        public void Scan_Should_Match_When_Regex_Starts_At_Zero()
        {
            var scanner = new Scanner("a b c");
            var match = scanner.Scan("a");
            Assert.Equal(match, "a");
            Assert.Equal(scanner.Pos, 1);
        }

        [Fact]
        public void Scan_Should_Return_Empty_When_Regex_Match_Is_Greater_Than_Zero()
        {
            var scanner = new Scanner("a b c");
            var match = scanner.Scan("b");
            Assert.Equal(match, "");
            Assert.Equal(scanner.Pos, 0);
        }

        [Fact]
        public void Scan_Should_Return_Empty_When_Regex_Does_Not_Match()
        {
            var scanner = new Scanner("a b c");
            var match = scanner.Scan("z");
            Assert.Equal(match, "");
            Assert.Equal(scanner.Pos, 0);
        }

        [Fact]
        public void ScanUntil_Should_Return_Empty_When_Regex_Matches_At_Zero_Index()
        {
            var scanner = new Scanner("a b c");
            var match = scanner.ScanUntil("a");
            Assert.Equal(match, "");
            Assert.Equal(scanner.Pos, 0);
        }

        [Fact]
        public void ScanUntil_Should_Return_String_Up_To_Index_When_Matched()
        {
            var scanner = new Scanner("a b c");
            var match = scanner.ScanUntil("b");
            Assert.Equal(match, "a ");
            Assert.Equal(scanner.Pos, 2);
        }

        [Fact]
        public void ScanUntil_Should_Return_Whole_String_When_Not_Matched()
        {
            var scanner = new Scanner("a b c");
            var match = scanner.ScanUntil("z");
            Assert.Equal(match, scanner.Template);
            Assert.True(scanner.EOS);
        }
    }
}
