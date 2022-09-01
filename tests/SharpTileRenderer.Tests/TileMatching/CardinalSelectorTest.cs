using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Tests.Fixtures;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.TileMatching
{
    [TestFixture]
    public class CardinalSelectorTest: SelectorTestBase<CardinalSelectorModel>
    {
        protected override string XmlResult => @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <layers>
    <layer>
      <id>test-layer</id>
      <enabled>true</enabled>
      <ts:cardinal>
        <ts:prefix>t.prefix.</ts:prefix>
        <ts:context-data-set>context-data</ts:context-data-set>
        <ts:match-self>
          <ts:class>A-class</ts:class>
        </ts:match-self>
        <ts:match-with>
          <ts:class>B-class</ts:class>
        </ts:match-with>
      </ts:cardinal>
    </layer>
  </layers>
</tile-set>";

        protected override string YamlResult => @"kind: Selector-Specification
renderLayers:
  - id: test-layer
    enabled: true
    match:
      kind: cardinal
      prefix: t.prefix.
      contextDataSet: context-data
      matchSelf:
        - A-class
      matchWith:
        - B-class
";


        protected override ITileDataSet<GraphicTag, Unit> CreateDataSet()
        {
            //      .B.
            //      $AC
            //      .BA
            return ArrayDataSet.CreateBasicTagDataSet<Unit>(20, 20)
                               .WithDataAt(0, 0, 0, new GraphicTag("tag-A"))
                               .WithDataAt(5, 5, 0, new GraphicTag("tag-A"))
                               .WithDataAt(6, 6, 0, new GraphicTag("tag-A"))
                               .WithDataAt(4, 5, 0, new GraphicTag("tag-$"))
                               .WithDataAt(5, 4, 0, new GraphicTag("tag-B"))
                               .WithDataAt(5, 6, 0, new GraphicTag("tag-B"))
                               .WithDataAt(6, 5, 0, new GraphicTag("tag-C"));
        }

        protected override string SelectorKind => BuiltInSelectors.Cardinal;

        protected override (CardinalSelectorModel, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher()
        {
            var sm = new CardinalSelectorModel()
            {
                Prefix = "t.prefix.",
                ContextDataSet = "context-data",
                MatchSelf = { "A-class" },
                MatchWith = { "B-class" }
            };

            var factory = new MatcherFactory<EntityClassification16>();
            factory.WithDefaultMatchers();

            var spriteMatcher = factory.CreateTagMatcher(sm, MatchFactoryContextFixture?.FactoryContext ?? throw new NullReferenceException());
            return (sm, spriteMatcher);
        }

        [Test]
        public void ValidateSelectorModelEquality()
        {
            var sm = new CardinalSelectorModel();
            sm.Equals(new CardinalSelectorModel()).Should().BeTrue();

            var sm2 = new CardinalSelectorModel()
            {
                Prefix = "prefix",
                MatchSelf = { "A-class" },
                MatchWith = { "B-class" }
            };
            sm2.Equals(new CardinalSelectorModel()).Should().BeFalse();
            sm2.Equals(new CardinalSelectorModel() { Prefix = "prefix" }).Should().BeFalse();
            sm2.Equals(new CardinalSelectorModel()
               {
                   Prefix = "prefix",
                   MatchSelf = { "A-class" },
                   MatchWith = { "B-class" }
               })
               .Should()
               .BeTrue();
            sm2.Equals(new CardinalSelectorModel()
               {
                   Prefix = "prefix",
                   ContextDataSet = "dataset",
                   MatchSelf = { "A-class" },
                   MatchWith = { "B-class" }
               })
               .Should()
               .BeFalse();
        }

        [Test]
        public void ValidateSelectorMatch()
        {
            var (sm, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("tag-A"), new ContinuousMapCoordinate(5f, 5f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeTrue();
            resultCollector.Should().BeEquivalentTo((SpriteTag.Create(sm.Prefix, input.TagData.ToString(), "_n1e0s1w1"), SpritePosition.Whole, input.Position));
        }

        [Test]
        public void ValidateSelectorMatch_Invalid()
        {
            var (_, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("invalid"), new ContinuousMapCoordinate(4f, 4f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeFalse();
            resultCollector.Should().BeEmpty();
        }

        [Test]
        public void ValidateSelectorMatch_Edge()
        {
            var (sm, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("tag-A"), new ContinuousMapCoordinate(0f, 0f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeTrue();
            resultCollector.Should().BeEquivalentTo((SpriteTag.Create(sm.Prefix, input.TagData.ToString(), "_n0e0s0w0"), SpritePosition.Whole, input.Position));
        }


    }
}