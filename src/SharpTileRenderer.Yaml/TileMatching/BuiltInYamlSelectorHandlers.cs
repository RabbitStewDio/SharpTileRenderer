using SharpTileRenderer.TileMatching.Model.Selectors;
using SharpYaml;
using SharpYaml.Model;

namespace SharpTileRenderer.Yaml.TileMatching
{
    public static class BuiltInYamlSelectorHandlers
    {
        public static T ConfigureBuiltInSelectorReadHandlers<T>(this T c)
            where T: IYamlTileMatcherModelReaderSettings
        {
            c.RegisterSelectorHandler<BasicSelectorModel>(BuiltInSelectors.Basic);
            c.RegisterSelectorHandler<CornerSelectorModel>(BuiltInSelectors.Corner);
            c.RegisterSelectorHandler<CellGroupSelectorModel>(BuiltInSelectors.CellGroup);
            c.RegisterSelectorHandler<CardinalSelectorModel>(BuiltInSelectors.Cardinal);
            c.RegisterSelectorHandler<RiverOutletSelectorModel>(BuiltInSelectors.RiverOutlet);
            c.RegisterSelectorHandler<RoadCornerSelectorModel>(BuiltInSelectors.RoadCorner);
            c.RegisterSelectorHandler<DiagonalSelectorModel>(BuiltInSelectors.Diagonal);
            c.RegisterSelectorHandler<NeighbourOverlaySelectorModel>(BuiltInSelectors.NeighbourOverlay);
            c.RegisterSelectorHandler<RoadParitySelectorModel>(BuiltInSelectors.RoadParity);
            c.RegisterSelectorHandler(BuiltInSelectors.List, ParseListSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.Choice, ParseChoiceSelectorModel);
            return c;
        }

        public static ISelectorModel ParseListSelectorModel(YamlMapping node, YamlParserContext context)
        {
            var retval = new ListSelectorModel();
            if (node[context.SerializerSettings.NamingConvention.Convert(nameof(ListSelectorModel.Selectors))] is not YamlSequence nodes)
            {
                return retval;
            }

            foreach (var n in nodes)
            {
                if (n is not YamlMapping child)
                {
                    continue;
                }

                var childSelector = context.ParseSelector(child);
                retval.Selectors.Add(childSelector);
            }

            return retval;
        }

        public static ISelectorModel ParseChoiceSelectorModel(YamlMapping node, YamlParserContext context)
        {
            var retval = new ChoiceSelectorModel();
            if (node[context.SerializerSettings.NamingConvention.Convert(nameof(ChoiceSelectorModel.Choices))] is not YamlSequence nodes)
            {
                return retval;
            }
            
            foreach (var n in nodes)
            {
                if (n is not YamlMapping child)
                {
                    continue;
                }

                retval.Choices.Add(ParseChoice(child, context));
            }

            return retval;
        }

        static ChoiceDefinition ParseChoice(YamlMapping node, YamlParserContext context)
        {
            if (node[context.SerializerSettings.NamingConvention.Convert(nameof(ChoiceDefinition.MatchedTags))] is not YamlSequence matcherNodes)
            {
                throw new YamlException();
            }

            var cd = new ChoiceDefinition();
            foreach (var matcherNode in matcherNodes)
            {
                if (matcherNode is YamlValue value)
                {
                    cd.MatchedTags.Add(value.Value);
                }
            }

            if (node[context.SerializerSettings.NamingConvention.Convert(nameof(ChoiceDefinition.Selector))] is not YamlMapping selector)
            {
                throw new YamlException();
            }

            cd.Selector = context.ParseSelector(selector);
            return cd;
        }
    }
}