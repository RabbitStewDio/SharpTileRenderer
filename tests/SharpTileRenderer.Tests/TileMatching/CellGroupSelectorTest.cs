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
    public class CellGroupSelectorTest: SelectorTestBase<CellGroupSelectorModel>
    {
        protected override ITileDataSet<GraphicTag, Unit> CreateDataSet()
        {
            //      AB.
            //      $AC
            //      .BA
            return ArrayDataSet.CreateBasicTagDataSet<Unit>(20, 20)
                               .WithDataAt(0, 0, 0, new GraphicTag("tag-A"))
                               .WithDataAt(4, 4, 0, new GraphicTag("tag-A"))
                               .WithDataAt(5, 4, 0, new GraphicTag("tag-B"))
                               .WithDataAt(4, 5, 0, new GraphicTag("tag-$"))
                               .WithDataAt(5, 5, 0, new GraphicTag("tag-A"))
                               .WithDataAt(6, 5, 0, new GraphicTag("tag-C"))
                               .WithDataAt(5, 6, 0, new GraphicTag("tag-B"))
                               .WithDataAt(6, 6, 0, new GraphicTag("tag-A"));
        }
        
        [Test]
        public void ValidateSelectorModelEquality()
        {
            var sm = new CellGroupSelectorModel();
            sm.Equals(new CellGroupSelectorModel()).Should().BeTrue();

            var sm2 = new CellGroupSelectorModel()
            {
                Prefix = "prefix",
                Matches = { "A-class", "B-class" },
            };
            sm2.Equals(new CellGroupSelectorModel()).Should().BeFalse();
            sm2.Equals(new CellGroupSelectorModel() { Prefix = "prefix" }).Should().BeFalse();
            sm2.Equals(new CellGroupSelectorModel()
               {
                   Prefix = "prefix",
                   Matches = { "A-class", "B-class" },
               })
               .Should()
               .BeTrue();
            sm2.Equals(new CellGroupSelectorModel()
               {
                   Prefix = "prefix",
                   ContextDataSet = "dataset",
                   Matches = { "A-class", "B-class" },
               })
               .Should()
               .BeFalse();
        }
        
        [Test]
        public void ValidateSelectorMatch_Complete()
        {
            // Matches with 4,4; 5,4; 4,5; 5,5
            var (sm, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("tag-A"), new ContinuousMapCoordinate(5f, 5f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeTrue();
            resultCollector.Should().BeEquivalentTo((SpriteTag.Create(null, sm.Prefix, "_A_B_A_A"), SpritePosition.CellMap, input.Position));
        }

        [Test]
        public void ValidateSelectorMatch_Incomplete()
        {
            // Matches with 4,3; 5,3; 4,4; 5,4
            //
            // Cells with y=3 do not have data. Thus the match is incomplete

            // This test encounters an unknown tile (tag-C) that does not define any matched classes.
            // This is an error in itself and will make matching fail unless there is a default match class
            // defined (which is not in this test)
            var (_, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("ignored"), new ContinuousMapCoordinate(5f, 4f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeFalse();
        }

        [Test]
        public void ValidateSelectorMatch_Invalid()
        {
            // Matches with tag-C at 6,5. This tag has none of the matching classes, and we do not define
            // a default match. So there will be no sprite produced here.
            var (_, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("ignored"), new ContinuousMapCoordinate(6f, 6f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeFalse();
        }

        [Test]
        public void ValidateSelectorMatch_Edge()
        {
            // Matches into undefined cells. Should simply not throw an exception, it wont match anything either.
            var (_, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("ignored"), new ContinuousMapCoordinate(0f, 0f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeFalse();
        }

        protected override (CellGroupSelectorModel, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher()
        {
            var sm = new CellGroupSelectorModel()
            {
                Matches = { "A-class", "B-class" },
                Prefix = "t.prefix.",
                ContextDataSet = "context-data"
            };

            var factory = new MatcherFactory<EntityClassification16>();
            factory.WithDefaultMatchers();

            var spriteMatcher = factory.CreateTagMatcher(sm, MatchFactoryContextFixture?.FactoryContext ?? throw new NullReferenceException());
            return (sm, spriteMatcher);
        }

        protected override string XmlResult => @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <layers>
    <layer>
      <id>test-layer</id>
      <enabled>true</enabled>
      <ts:cell-group>
        <ts:prefix>t.prefix.</ts:prefix>
        <ts:context-data-set>context-data</ts:context-data-set>
        <ts:matches>
          <ts:class>A-class</ts:class>
          <ts:class>B-class</ts:class>
        </ts:matches>
      </ts:cell-group>
    </layer>
  </layers>
</tile-set>";

        protected override string YamlResult => @"kind: Selector-Specification
renderLayers:
  - id: test-layer
    enabled: true
    match:
      kind: cell-group
      prefix: t.prefix.
      contextDataSet: context-data
      matches:
        - A-class
        - B-class
      direction: Up
";

        protected override string SelectorKind => BuiltInSelectors.CellGroup;
    }
}