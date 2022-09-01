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
    public class RiverOutletSelectorTest: SelectorTestBase<RiverOutletSelectorModel>
    {
        protected override string SelectorKind => BuiltInSelectors.RiverOutlet;
        protected override string XmlResult => @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <layers>
    <layer>
      <id>test-layer</id>
      <enabled>true</enabled>
      <ts:river-outlet>
        <ts:prefix>prefix.</ts:prefix>
        <ts:context-data-set>context-data</ts:context-data-set>
        <ts:match-self>
          <ts:class>A-class</ts:class>
        </ts:match-self>
        <ts:match-with>
          <ts:class>A-class</ts:class>
          <ts:class>B-class</ts:class>
        </ts:match-with>
      </ts:river-outlet>
    </layer>
  </layers>
</tile-set>";
        
        protected override string YamlResult => @"kind: Selector-Specification
renderLayers:
  - id: test-layer
    enabled: true
    match:
      kind: river-outlet
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

        protected  override  (RiverOutletSelectorModel, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher()
        {
            var sm = new RiverOutletSelectorModel()
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
            var sm = new RiverOutletSelectorModel();
            sm.Equals(new RiverOutletSelectorModel()).Should().BeTrue();

            var sm2 = new RiverOutletSelectorModel()
            {
                MatchSelf = { "A-class" },
                MatchWith = { "A-class", "B-class" },
                Prefix = "prefix.",
                ContextDataSet = "context-data"
            };
            sm2.Equals(new RiverOutletSelectorModel()).Should().BeFalse();
            sm2.Equals(new RiverOutletSelectorModel() { Prefix = "prefix" }).Should().BeFalse();
            sm2.Equals(new RiverOutletSelectorModel()
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
                               (SpriteTag.Create(sm.Prefix, "tag-A", ".s"), SpritePosition.Whole, new ContinuousMapCoordinate(5, 4)),
                               (SpriteTag.Create(sm.Prefix, "tag-A", ".n"), SpritePosition.Whole, new ContinuousMapCoordinate(5, 6)),
                               (SpriteTag.Create(sm.Prefix, "tag-A", ".e"), SpritePosition.Whole, new ContinuousMapCoordinate(4, 5))
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