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
    public class ChoiceSelectorTest: SelectorTestBase<ChoiceSelectorModel>
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

        protected override string SelectorKind => BuiltInSelectors.Choice;

        protected override string XmlResult => @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <layers>
    <layer>
      <id>test-layer</id>
      <enabled>true</enabled>
      <ts:choice>
        <ts:choice-selection>
          <ts:choice-matches>
            <ts:class>tag-A</ts:class>
          </ts:choice-matches>
          <ts:basic>
            <ts:prefix>choice-A.</ts:prefix>
          </ts:basic>
        </ts:choice-selection>
        <ts:choice-selection>
          <ts:choice-matches>
            <ts:class>tag-B</ts:class>
          </ts:choice-matches>
          <ts:basic>
            <ts:prefix>choice-B.</ts:prefix>
          </ts:basic>
        </ts:choice-selection>
      </ts:choice>
    </layer>
  </layers>
</tile-set>";

        protected override string YamlResult => @"kind: Selector-Specification
renderLayers:
  - id: test-layer
    enabled: true
    match:
      kind: choice
      choices:
        - matchedTags:
            - tag-A
          selector:
            kind: basic
            prefix: choice-A.
        - matchedTags:
            - tag-B
          selector:
            kind: basic
            prefix: choice-B.
";

        [Test]
        public void ValidateSelectorModelEquality()
        {
            var sm = new ChoiceSelectorModel();
            sm.Equals(new ChoiceSelectorModel()).Should().BeTrue();

            var sm2 = new ChoiceSelectorModel()
            {
                Choices =
                {
                    new ChoiceDefinition()
                    {
                        MatchedTags = { "Tag-A" },
                        Selector = new BasicSelectorModel()
                        {
                            Prefix = "choice-A."
                        }
                    },
                    new ChoiceDefinition()
                    {
                        MatchedTags = { "Tag-B" },
                        Selector = new BasicSelectorModel()
                        {
                            Prefix = "choice-B."
                        }
                    }
                }
            };
            sm2.Equals(new ChoiceSelectorModel()).Should().BeFalse();
        }

        protected override (ChoiceSelectorModel, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher()
        {
            var sm = new ChoiceSelectorModel()
            {
                Choices =
                {
                    new ChoiceDefinition()
                    {
                        MatchedTags = { "tag-A" },
                        Selector = new BasicSelectorModel()
                        {
                            Prefix = "choice-A."
                        }
                    },
                    new ChoiceDefinition()
                    {
                        MatchedTags = { "tag-B" },
                        Selector = new BasicSelectorModel()
                        {
                            Prefix = "choice-B."
                        }
                    }
                }
            };

            var factory = new MatcherFactory<EntityClassification16>();
            factory.WithDefaultMatchers();

            var spriteMatcher = factory.CreateTagMatcher(sm, MatchFactoryContextFixture?.FactoryContext ?? throw new NullReferenceException());
            return (sm, spriteMatcher);
        }

        [Test]
        public void ValidateMatching()
        {
            var (_, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("tag-A"), new ContinuousMapCoordinate(0f, 0f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeTrue();
            resultCollector.Should().BeEquivalentTo((SpriteTag.Create("choice-A.", input.TagData.ToString(), null), SpritePosition.Whole, input.Position));

        }
        
        [Test]
        public void ValidateNonMatching()
        {
            var (_, spriteMatcher) = CreateSpriteMatcher();
            var input = SpriteMatcherInput.From(GraphicTag.From("tag-C"), new ContinuousMapCoordinate(0f, 0f));
            var resultCollector = new List<(SpriteTag tag, SpritePosition spriteOffset, ContinuousMapCoordinate pos)>();

            spriteMatcher.Match(input, 0, resultCollector).Should().BeFalse();
        }
    }
}