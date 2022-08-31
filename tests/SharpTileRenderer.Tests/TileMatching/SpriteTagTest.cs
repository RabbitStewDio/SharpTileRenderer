using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.TileMatching;
using System;
using System.Text.RegularExpressions;

namespace SharpTileRenderer.Tests.TileMatching
{
    public class SpriteTagTest
    {
        [Test]
        public void Parse()
        {
            SpriteTag.Parse("p1.prefix.base_suffix").Should().Contain(SpriteTag.Create("p1.prefix.", "base", "_suffix"));
            SpriteTag.Parse("prefix.base_suffix").Should().Contain(SpriteTag.Create("prefix.", "base", "_suffix"));
            SpriteTag.Parse("base_suffix").Should().Contain(SpriteTag.Create(null, "base", "_suffix"));
            SpriteTag.Parse("base").Should().Contain(SpriteTag.Create(null, "base", null));
            SpriteTag.Parse("").Should().BeEmpty();
        }

        [Test]
        public void TestRegEx()
        {
            var re = new Regex(@"(?<prefix>.+\.)?(?<base>[^_]+)(?<suffix>_.+)?");
            var match = re.Match("aaa.aaaa.bbbb_cccc_ccc");
            for (var i = 0; i < match.Groups.Count; i++)
            {
                var g = match.Groups[i];
                Console.WriteLine($"{i}: {g.Success} - {g.Value}");
            }
        }
    }
}