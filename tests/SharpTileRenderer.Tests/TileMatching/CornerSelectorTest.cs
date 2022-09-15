using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Tests.Fixtures;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.TileMatching.Selectors.BuiltIn;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.TileMatching
{
    public class CornerSelectorTest: SelectorTestBase<CornerSelectorModel>
    {
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

        protected override string SelectorKind => BuiltInSelectors.Corner;
        protected override string XmlResult => @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <layers>
    <layer>
      <id>test-layer</id>
      <enabled>true</enabled>
      <sort-order>TopDownLeftRight</sort-order>
      <ts:corner>
        <ts:prefix>prefix.</ts:prefix>
        <ts:context-data-set>context-data</ts:context-data-set>
        <ts:default-class>B-class</ts:default-class>
        <ts:matches>
          <ts:class>A-class</ts:class>
          <ts:class>B-class</ts:class>
        </ts:matches>
      </ts:corner>
    </layer>
  </layers>
</tile-set>";
        
        protected override string YamlResult => @"kind: Selector-Specification
renderLayers:
  - id: test-layer
    enabled: true
    sortingOrder: TopDownLeftRight
    match:
      kind: corner
      prefix: prefix.
      contextDataSet: context-data
      matches:
        - A-class
        - B-class
      defaultClass: B-class
";

        protected override (CornerSelectorModel, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher()
        {
            var sm = new CornerSelectorModel()
            {
                Matches = { "A-class", "B-class" },
                Prefix = "prefix.",
                ContextDataSet = "context-data",
                DefaultClass = "B-class"
            };

            var factory = new MatcherFactory<EntityClassification16>();
            factory.WithDefaultMatchers();

            var spriteMatcher = factory.CreateTagMatcher(sm, MatchFactoryContextFixture?.FactoryContext ?? throw new NullReferenceException());
            return (sm, spriteMatcher);
        }

        [Test]
        public void ValidateSelectorModelEquality()
        {
            var sm = new CornerSelectorModel();
            sm.Equals(new CornerSelectorModel()).Should().BeTrue();

            var sm2 = new CornerSelectorModel()
            {
                Matches = { "A-class", "B-class" },
                Prefix = "prefix.",
                ContextDataSet = "context-data"
            };
            sm2.Equals(new CornerSelectorModel()).Should().BeFalse();
            sm2.Equals(new CornerSelectorModel() { Prefix = "prefix" }).Should().BeFalse();
            sm2.Equals(new CornerSelectorModel()
               {
                   Matches = { "A-class", "B-class" },
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
                               // Matches B.C -> class-B, class-B via default, class-B via default
                               (SpriteTag.Create(sm.Prefix, input.TagData.ToString(), "_cell_u_B_B_B"), SpritePosition.Up, input.Position),
                               // Matches CAB -> class-B via default, class-A, class-B
                               (SpriteTag.Create(sm.Prefix, input.TagData.ToString(), "_cell_r_B_A_B"), SpritePosition.Right, input.Position),
                               // Matches B.$ -> class-B, class-B via default, class-A
                               (SpriteTag.Create(sm.Prefix, input.TagData.ToString(), "_cell_d_B_B_A"), SpritePosition.Down, input.Position),
                                // Matches $.B -> clsas-A, class-B via default, class-B
                               (SpriteTag.Create(sm.Prefix, input.TagData.ToString(), "_cell_l_A_B_B"), SpritePosition.Left, input.Position)
                           );
        }

        [Test]
        public void ValidateSelectorInvalid()
        {
            var sm = new CornerSelectorModel()
            {
                Matches = { "A-class", "B-class" },
                Prefix = "prefix.",
                ContextDataSet = "context-data",
            };
            var factory = new MatcherFactory<EntityClassification16>();
            factory.WithDefaultMatchers();

            var spriteMatcher = factory.CreateTagMatcher(sm, MatchFactoryContextFixture?.FactoryContext ?? throw new NullReferenceException());
            var input = SpriteMatcherInput.From(GraphicTag.From("invalid"), new ContinuousMapCoordinate(5f, 5f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeFalse();
        }
    }
}