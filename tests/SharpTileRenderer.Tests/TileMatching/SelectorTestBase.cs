using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.DataSets;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpTileRenderer.TileMatching.Selectors;
using SharpTileRenderer.Xml.TileMatching;
using SharpTileRenderer.Yaml.TileMatching;
using System.Collections.Generic;
using System.IO;

namespace SharpTileRenderer.Tests.TileMatching
{
    public class InMemoryContentLoader : IContentLoader
    {
        readonly Dictionary<ContentUri, string> contents;

        public InMemoryContentLoader()
        {
            this.contents = new Dictionary<ContentUri, string>();
        }

        public InMemoryContentLoader WithData(ContentUri uri, string data)
        {
            this.contents[uri] = data;
            return this;
        }

        public TextReader LoadText(ContentUri name)
        {
            if (contents.TryGetValue(name, out var data))
            {
                return new StringReader(data);
            }

            throw new IOException($"Unable to locate data for {name}");
        }
    }
    
    public abstract class SelectorTestBase<TSelector>
        where TSelector: ISelectorModel
    {
        protected MatchFactoryContextFixture? MatchFactoryContextFixture;

        [SetUp]
        public void SetUp()
        {
            MatchFactoryContextFixture = MatchFactoryContextFixture.CreateDefault();
            MatchFactoryContextFixture.DataSetProducer.WithDataSet("context-data", CreateDataSet);
        }

        protected abstract ITileDataSet<GraphicTag, Unit> CreateDataSet();
        
        [Test]
        public void ValidateSelectorBuilding()
        {
            var (_, spriteMatcher) = CreateSpriteMatcher();
            spriteMatcher.MatcherType.Should().Be(SelectorKind);
            spriteMatcher.IsThreadSafe.Should().Be(ThreadSafe);
        }

        [Test]
        public void ValidateSelectorModelKind()
        {
            var (sm, _) = CreateSpriteMatcher();
            sm.Kind.Should().Be(SelectorKind);
        }

        protected abstract (TSelector, ISpriteMatcher<GraphicTag>) CreateSpriteMatcher();

        protected virtual bool ThreadSafe => true;
        protected abstract string SelectorKind { get; }
        protected abstract string XmlResult { get; }
        protected abstract string YamlResult { get; }
        
        
        [Test]
        public void ValidateXmlParsing()
        {
            var parser = new XmlTileMatcherModelParser()
                         .ConfigureBuiltInSelectorReadHandlers()
                         .ConfigureBuiltInDataSetReadHandlers();
            var loader = new InMemoryContentLoader().WithData(ContentUri.MakeRelative("test.xml"), XmlResult);

            var (sm, _) = CreateSpriteMatcher();

            var value = parser.Load(loader, ContentUri.MakeRelative("test.xml"));
            value.RenderLayers[0].Match.Should().Be(sm);
        }

        [Test]
        public void ValidateXmlWriting()
        {
            var (sm, _) = CreateSpriteMatcher();

            var model = new TileMatcherModel()
            {
                RenderLayers =
                {
                    new RenderLayerModel()
                    {
                        Match = sm,
                        Id = "test-layer"
                    }
                }
            };

            var writer = new XmlTileMatcherModelWriter();
            writer.ConfigureBuiltInSelectorWriteHandlers();
            writer.ConfigureBuiltInDataSetWriteHandlers();
            var result = writer.Write(model);
            result.Should().BeEquivalentTo(XmlResult);
        }

        [Test]
        public void ValidateYamlParsing()
        {
            var parser = new YamlTileMatcherModelParser()
                         .ConfigureBuiltInSelectorReadHandlers()
                         .ConfigureBuiltInDataSetReadHandlers();

            var (sm, _) = CreateSpriteMatcher();

            var value = parser.Read(YamlResult);
            value.RenderLayers[0].Match.Should().Be(sm);
        }

        [Test]
        public void ValidateYamlWriting()
        {
            var (sm, _) = CreateSpriteMatcher();

            var model = new TileMatcherModel()
            {
                RenderLayers =
                {
                    new RenderLayerModel()
                    {
                        Match = sm,
                        Id = "test-layer"
                    }
                }
            };

            var writer = new YamlTileMatcherModelWriter();
            var result = writer.Write(model);
            result.Should().Be(YamlResult);
        }
    }
}