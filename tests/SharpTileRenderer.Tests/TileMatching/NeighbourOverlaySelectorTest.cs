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
    public class NeighbourOverlaySelectorTest: SelectorTestBase<NeighbourOverlaySelectorModel>
    {
        protected override string SelectorKind => BuiltInSelectors.NeighbourOverlay;
        protected override string XmlResult => @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <layers>
    <layer>
      <id>test-layer</id>
      <enabled>true</enabled>
      <ts:neighbour-overlay>
        <ts:prefix>prefix.</ts:prefix>
        <ts:context-data-set>context-data</ts:context-data-set>
        <ts:match-self>
          <ts:class>A-class</ts:class>
        </ts:match-self>
        <ts:match-with>
          <ts:class>A-class</ts:class>
          <ts:class>B-class</ts:class>
        </ts:match-with>
      </ts:neighbour-overlay>
    </layer>
  </layers>
</tile-set>";
        
        protected override string YamlResult => @"kind: Selector-Specification
renderLayers:
  - id: test-layer
    enabled: true
    match:
      kind: neighbour-overlay
      prefix: prefix.
      contextDataSet: context-data
      matchSelf:
        - A-class
      matchWith:
        - A-class
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

        protected override (NeighbourOverlaySelectorModel, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher()
        {
            var sm = new NeighbourOverlaySelectorModel()
            {
                MatchSelf = { "A-class" },
                MatchWith = { "A-class", "B-class" },
                Prefix = "prefix.",
                ContextDataSet = "context-data"
            };

            var factory = new MatcherFactory<EntityClassification16>();
            factory.WithDefaultMatchers();

            var spriteMatcher = factory.CreateTagMatcher(sm, MatchFactoryContextFixture?.FactoryContext ?? throw new NullReferenceException());
            return (sm, spriteMatcher);
        }

        [Test]
        public void ValidateSelectorModelEquality()
        {
            var sm = new NeighbourOverlaySelectorModel();
            sm.Equals(new NeighbourOverlaySelectorModel()).Should().BeTrue();

            var sm2 = new NeighbourOverlaySelectorModel()
            {
                MatchSelf = { "A-class" },
                MatchWith = { "A-class", "B-class" },
                Prefix = "prefix.",
                ContextDataSet = "context-data"
            };
            sm2.Equals(new NeighbourOverlaySelectorModel()).Should().BeFalse();
            sm2.Equals(new NeighbourOverlaySelectorModel() { Prefix = "prefix" }).Should().BeFalse();
            sm2.Equals(new NeighbourOverlaySelectorModel()
               {
                   MatchSelf = { "A-class" },
                   MatchWith = { "A-class", "B-class" },
                   Prefix = "prefix.",
                   ContextDataSet = "context-data"
               })
               .Should()
               .BeTrue();
        }

        [Test]
        public void ValidateSelectorMatch()
        {
            var (sm, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("tag-A"), new ContinuousMapCoordinate(5f, 5f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeTrue();
            resultCollector.Should()
                           .BeEquivalentTo(
                               (SpriteTag.Create(sm.Prefix, "tag-A", ".n"), SpritePosition.Whole, input.Position),
                               (SpriteTag.Create(sm.Prefix, "tag-A", ".se"), SpritePosition.Whole, input.Position),
                               (SpriteTag.Create(sm.Prefix, "tag-A", ".s"), SpritePosition.Whole, input.Position),
                               (SpriteTag.Create(sm.Prefix, "tag-A", ".w"), SpritePosition.Whole, input.Position)
                           );
        }

        [Test]
        public void ValidateSelectorInvalid()
        {
            var (_, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("invalid"), new ContinuousMapCoordinate(5f, 5f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeFalse();
        }
        
    }
}