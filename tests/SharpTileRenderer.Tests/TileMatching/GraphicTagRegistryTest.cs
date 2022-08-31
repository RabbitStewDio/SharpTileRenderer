using FluentAssertions;
using NUnit.Framework;
using SharpTileRenderer.TexturePack;
using SharpTileRenderer.TileMatching;
using SharpTileRenderer.TileMatching.Model;
using SharpTileRenderer.TileMatching.Model.Meta;
using SharpTileRenderer.Xml.TileMatching;
using SharpTileRenderer.Yaml;
using SharpTileRenderer.Yaml.TileMatching;
using SharpYaml.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpTileRenderer.Tests.TileMatching
{
    [TestFixture]
    public class GraphicTagRegistryTest
    {
        [Test]
        public void ValidateRegistration()
        {
            var tagRegistry = new GraphicTagMetaDataRegistry<EntityClassification32>();
            tagRegistry.Register(GraphicTag.From("tag"));
            tagRegistry.KnownTags.Should().Contain(GraphicTag.From("tag"));
        }

        [Test]
        public void ValidateRegistrationWithFlags()
        {
            var tagRegistry = new GraphicTagMetaDataRegistry<EntityClassification32>();
            tagRegistry.Register(GraphicTag.From("tag")).WithFlag("flag");
            tagRegistry.HasFlag(GraphicTag.From("tag"), "flag").Should().BeTrue();
            tagRegistry.HasFlag(GraphicTag.From("tag"), "not flag").Should().BeFalse();
        }

        [Test]
        public void ValidateRegistrationWithClass()
        {
            var classRegistry = new EntityClassificationRegistry<EntityClassification32>();
            var tagRegistry = new GraphicTagMetaDataRegistry<EntityClassification32>(classRegistry);
            tagRegistry.Register(GraphicTag.From("tag")).WithClassification("class");
            classRegistry.KnownClasses.TryGetValue("class", out var classReg).Should().BeTrue();
            tagRegistry.QueryClasses(GraphicTag.From("tag")).Should().Be(classReg);
        }

        const string YamlResult = @"kind: Selector-Specification
tags:
  - id: GraphicTag-A
  - id: GraphicTag-B
    classes:
      - Class-A
      - Class-B
  - id: GraphicTag-C
    flags:
      - Flag-A
      - Flag-B
  - id: GraphicTag-C
    properties:
      PropertyName-A: PropertyValue
      PropertyName-B: PropertyValue2
  - id: GraphicTag-D
    classes:
      - Class-A
      - Class-B
    properties:
      PropertyName-A: PropertyValue
      PropertyName-B: PropertyValue2
    flags:
      - Flag-A
      - Flag-B
";
        
        const string XmlResult = @"<tile-set xmlns:ts=""https://docs.rabbit-stew-dio.com/xml-schema/tileselector/v1/"" xmlns=""https://docs.rabbit-stew-dio.com/xml-schema/tilematcher/v1/"">
  <tags>
    <graphic-tag>
      <id>GraphicTag-A</id>
    </graphic-tag>
    <graphic-tag>
      <id>GraphicTag-B</id>
      <classes>
        <class>Class-A</class>
        <class>Class-B</class>
      </classes>
    </graphic-tag>
    <graphic-tag>
      <id>GraphicTag-C</id>
      <flags>
        <flag>Flag-A</flag>
        <flag>Flag-B</flag>
      </flags>
    </graphic-tag>
    <graphic-tag>
      <id>GraphicTag-C</id>
      <properties>
        <property>
          <name>PropertyName-A</name>
          <value>PropertyValue</value>
        </property>
        <property>
          <name>PropertyName-B</name>
          <value>PropertyValue2</value>
        </property>
      </properties>
    </graphic-tag>
    <graphic-tag>
      <id>GraphicTag-D</id>
      <flags>
        <flag>Flag-A</flag>
        <flag>Flag-B</flag>
      </flags>
      <classes>
        <class>Class-A</class>
        <class>Class-B</class>
      </classes>
      <properties>
        <property>
          <name>PropertyName-A</name>
          <value>PropertyValue</value>
        </property>
        <property>
          <name>PropertyName-B</name>
          <value>PropertyValue2</value>
        </property>
      </properties>
    </graphic-tag>
  </tags>
</tile-set>";
        
        [Test]
        public void ValidateXmlReading()
        {
            var parser = new XmlTileMatcherModelParser()
                         .ConfigureBuiltInSelectorReadHandlers()
                         .ConfigureBuiltInDataSetReadHandlers();

            var model = CreateTileMatcherModel();
            var loader = new InMemoryContentLoader().WithData(ContentUri.MakeRelative("test.xml"), XmlResult);

            var value = parser.Load(loader, ContentUri.MakeRelative("test.xml"));
            value.Tags.Should().BeEquivalentTo(model.Tags);
        }

        [Test]
        public void ValidateXmlWriting()
        {
            var model = CreateTileMatcherModel();

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

            var model = CreateTileMatcherModel();

            var value = parser.Read(YamlResult);
            value.Tags.Should().BeEquivalentTo(model.Tags);
        }

        [Test]
        public void ValidateYamlWriting()
        {
            var model = CreateTileMatcherModel();

            var writer = new YamlTileMatcherModelWriter();
            var result = writer.Write(model);
            result.Should().Be(YamlResult);
        }

        [Test]
        public void ValidateYamlParsingTagDef()
        {
            var yaml = @"id: GraphicTag-D
classes:
      - Class-A
      - Class-B
properties:
      PropertyName-A: PropertyValue
      PropertyName-B: PropertyValue2
flags:
      - Flag-A
      - Flag-B";

            var cfg = YamlSerializerSettings.CreateDefaultSerializerSettings();
            var stream = YamlStream.Load(new StringReader(yaml));
            var root = stream[0].Contents as YamlMapping ?? throw new Exception();
            var parsed = root.ToObject<GraphicTagDefinitionModel>(cfg);
            var expected = CreateTileMatcherModel().Tags[4];
            parsed.Should().Be(expected);
        }
        
        static TileMatcherModel CreateTileMatcherModel()
        {
            var model = new TileMatcherModel()
            {
                Tags =
                {
                    new GraphicTagDefinitionModel()
                    {
                        Id = "GraphicTag-A"
                    },
                    new GraphicTagDefinitionModel()
                    {
                        Id = "GraphicTag-B",
                        Classes = { "Class-A", "Class-B" }
                    },
                    new GraphicTagDefinitionModel()
                    {
                        Id = "GraphicTag-C",
                        Flags = { "Flag-A", "Flag-B" }
                    },
                    new GraphicTagDefinitionModel()
                    {
                        Id = "GraphicTag-C",
                        Properties =
                        {
                            new KeyValuePair<string, string>("PropertyName-A", "PropertyValue"),
                            new KeyValuePair<string, string>("PropertyName-B", "PropertyValue2")
                        }
                    },
                    new GraphicTagDefinitionModel()
                    {
                        Id = "GraphicTag-D",
                        Classes = { "Class-A", "Class-B" },
                        Flags = { "Flag-A", "Flag-B" },
                        Properties =
                        {
                            new KeyValuePair<string, string>("PropertyName-A", "PropertyValue"),
                            new KeyValuePair<string, string>("PropertyName-B", "PropertyValue2")
                        }
                    },
                }
            };
            return model;
        }
    }
}