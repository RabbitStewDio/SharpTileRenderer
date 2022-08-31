using SharpTileRenderer.TileMatching.Model.DataSets;
using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpYaml.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public static class YamlExtensions
    {
        public static string? AsString(this YamlElement? e)
        {
            return e is not YamlValue v ? null : v.Value;
        }

        public static string? ParseProperty(this YamlMapping node, string key)
        {
            if (TryParseProperty(node, key, out var x))
            {
                return x;
            }

            return null;
        }

        public static YamlElement? GetProperty(this YamlMapping node, string key)
        {
            if (TryGetProperty(node, key, out var x))
            {
                return x;
            }

            return null;
        }

        public static bool TryGetProperty(this YamlMapping node, string key, [MaybeNullWhen(false)] out YamlElement x)
        {
            return node.TryGetValue(key, out x);
        }

        public static bool TryParseProperty(this YamlMapping node, string key, [MaybeNullWhen(false)] out string x)
        {
            if (!node.TryGetValue(key, out var type))
            {
                x = default;
                return false;
            }

            x = type switch
            {
                YamlValue scalar => scalar.Value,
                _ => throw new ArgumentException()
            };

            return x != null;
        }

        public static void RegisterDataSetHandler<TDataSet>(this IYamlTileMatcherModelReaderSettings s, string id)
            where TDataSet : IDataSetModel
        {
            s.RegisterDataSetHandler(id, DefaultDataSetProducer<TDataSet>());
        }

        public static void RegisterSelectorHandler<TSelector>(this IYamlTileMatcherModelReaderSettings s, string id)
            where TSelector : ISelectorModel
        {
            s.RegisterSelectorHandler(id, DefaultSelectorProducer<TSelector>());
        }


        static Func<YamlMapping, YamlParserContext, IDataSetModel> DefaultDataSetProducer<T>()
            where T : IDataSetModel
        {
            return (element, context) =>
            {
                var dataSetModel = element.ToObject<T>(context.SerializerSettings);
                if (dataSetModel == null)
                {
                    throw new ArgumentException();
                }

                return dataSetModel;
            };
        }

        static Func<YamlMapping, YamlParserContext, ISelectorModel> DefaultSelectorProducer<T>()
            where T : ISelectorModel
        {
            return (element, context) =>
            {
                var selectorModel = element.ToObject<T>(context.SerializerSettings);
                if (selectorModel == null)
                {
                    throw new ArgumentException();
                }

                return selectorModel;
            };
        }
    }
}