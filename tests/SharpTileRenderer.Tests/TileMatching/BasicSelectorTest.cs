using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.Navigation;
using SharpTileRenderer.Tests.Fixtures;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors;
using System;
using System.Collections.Generic;

namespace SharpTileRenderer.Tests.TileMatching
{
    [TestFixture]
    public class BasicSelectorTest: SelectorTestBase<BasicSelectorModel>
    {
        protected override ITileDataSet<GraphicTag, Unit> CreateDataSet()
        {
            return new ArrayDataSet<GraphicTag, Unit>(DataSetType.TagMap, 1, 1);
        }

        protected override string SelectorKind => BuiltInSelectors.Basic;
        
        [Test]
        public void ValidateSelectorModelEquality()
        {
            var sm = new BasicSelectorModel();
            sm.Equals(new BasicSelectorModel()).Should().BeTrue();

            var sm2 = new BasicSelectorModel()
            {
                Prefix = "prefix"
            };
            sm2.Equals(new BasicSelectorModel()).Should().BeFalse();
            sm2.Equals(new BasicSelectorModel() { Prefix = "prefix" }).Should().BeTrue();
        }

        protected override (BasicSelectorModel, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher()
        {
            var sm = new BasicSelectorModel();
            sm.Prefix = "t.prefix.";

            var factory = new MatcherFactory<EntityClassification16>();
            factory.WithDefaultMatchers();

            var spriteMatcher = factory.CreateTagMatcher(sm, MatchFactoryContextFixture?.FactoryContext ?? throw new NullReferenceException());
            return (sm, spriteMatcher);
        }

        [Test]
        public void ValidateSelection()
        {
            var (sm, spriteMatcher) = CreateSpriteMatcher();

            var input = new SpriteMatcherInput<GraphicTag>(new GraphicTag("tag"), new ContinuousMapCoordinate(10.5f, 2f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeTrue();
            resultCollector.Should().BeEquivalentTo((SpriteTag.Create(sm.Prefix, input.TagData.ToString(), null), SpritePosition.Whole, input.Position));
        }

        protected override string XmlResult => @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <layers>
    <layer>
      <id>test-layer</id>
      <enabled>true</enabled>
      <ts:basic>
        <ts:prefix>t.prefix.</ts:prefix>
      </ts:basic>
    </layer>
  </layers>
</tile-set>";

        protected override string YamlResult => @"kind: Selector-Specification
renderLayers:
  - id: test-layer
    enabled: true
    match:
      kind: basic
      prefix: t.prefix.
";
            
    }
}