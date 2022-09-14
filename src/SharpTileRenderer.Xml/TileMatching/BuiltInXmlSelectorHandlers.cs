using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public static class BuiltInXmlSelectorHandlers
    {
        public static TSettings ConfigureBuiltInSelectorReadHandlers<TSettings>(this TSettings c)
            where TSettings : IXmlTileMatcherModelReaderSettings
        {
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Basic, ParseBasicSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Corner, ParseCornerSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.CellGroup, ParseCellGroupSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.List, ParseListSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Choice, ParseChoiceSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.NeighbourOverlay, ParseNeighbourOverlaySelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Cardinal, ParseCardinalSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Diagonal, ParseDiagonalSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.RiverOutlet, ParseRiverOutletSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.RoadCorner, ParseRoadCornerSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.RoadParity, ParseRoadParitySelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.Ns + BuiltInSelectors.QuantityChoice, ParseQuantityChoiceSelectorModel);
            return c;
        }

        public static TSettings ConfigureBuiltInSelectorWriteHandlers<TSettings>(this TSettings c)
            where TSettings : IXmlTileMatcherModelWriterSettings
        {
            c.RegisterSelectorHandler(BuiltInSelectors.Basic, WriteBasicSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.Cardinal, WriteCardinalSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.Diagonal, WriteDiagonalSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.Corner, WriteCornerSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.CellGroup, WriteCellGroupSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.List, WriteListSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.Choice, WriteChoiceSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.NeighbourOverlay, WriteNeighbourOverlaySelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.RoadCorner, WriteRoadCornerSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.RiverOutlet, WriteRiverOutletSelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.RoadParity, WriteRoadParitySelectorModel);
            c.RegisterSelectorHandler(BuiltInSelectors.QuantityChoice, WriteQuantityChoiceSelectorModel);
            return c;
        }

        static XElement? WriteBasicSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not BasicSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Basic);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.SuffixTag, m.Suffix);
            return element;
        }

        public static ISelectorModel ParseBasicSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var suffix = (string?)element.Element(XmlTileSelectorModelTags.SuffixTag);
            return new BasicSelectorModel()
            {
                Prefix = prefix,
                Suffix = suffix
            };
        }

        static XElement? WriteCornerSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not CornerSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Corner);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.AddStringElement(XmlTileSelectorModelTags.DefaultClassTag, m.DefaultClass);
            element.Add(m.Matches.AddStringList(XmlTileSelectorModelTags.Ns + "matches", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        public static ISelectorModel ParseCornerSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var defaultClass = (string?)element.Element(XmlTileSelectorModelTags.DefaultClassTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new CornerSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
                DefaultClass = defaultClass
            };
            
            element.ParseStringListElement(XmlTileSelectorModelTags.Ns + "matches", XmlTileSelectorModelTags.MatchClassTag, result.Matches);
            return result;
        }

        static XElement? WriteCellGroupSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not CellGroupSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.CellGroup);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.AddStringElement(XmlTileSelectorModelTags.DefaultClassTag, m.DefaultClass);
            element.Add(m.Matches.AddStringList(XmlTileSelectorModelTags.Ns + "matches", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        public static ISelectorModel ParseCellGroupSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var defaultClass = (string?)element.Element(XmlTileSelectorModelTags.DefaultClassTag);
            var direction = element.ParseEnumElement<CellGroupNavigationDirection>(XmlTileSelectorModelTags.Ns + "direction") ?? CellGroupNavigationDirection.Up;
            var retval = new CellGroupSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
                DefaultClass = defaultClass,
                Direction = direction,
            };
            element.ParseStringListElement(XmlTileSelectorModelTags.Ns + "matches", XmlTileSelectorModelTags.MatchClassTag, retval.Matches);
            return retval;
        }

        static XElement? WriteListSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not ListSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.List);
            foreach (var selectorModel in m.Selectors)
            {
                element.Add(context.WriteSelector(selectorModel));
            }

            return element;
        }

        public static ISelectorModel ParseListSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var selectorNodes = element.Elements();
            var retval = new ListSelectorModel();
            foreach (var selectorNode in selectorNodes)
            {
                if (context.ParseSelector(selectorNode).TryGetValue(out var selector))
                {
                    retval.Selectors.Add(selector);
                }
            }

            return retval;
        }

        static XElement? WriteChoiceSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not ChoiceSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Choice);
            foreach (var choice in m.Choices)
            {
                var choiceElement = new XElement(XmlTileSelectorModelTags.Ns + "choice-selection");
                choiceElement.Add(choice.MatchedTags.AddStringList(XmlTileSelectorModelTags.Ns + "choice-matches", XmlTileSelectorModelTags.MatchClassTag));
                var selector = context.WriteSelector(choice.Selector);
                if (selector == null)
                {
                    throw new Exception();
                }

                choiceElement.Add(selector);
                element.Add(choiceElement);
            }

            return element;
        }

        public static ISelectorModel ParseChoiceSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var retval = new ChoiceSelectorModel();
            foreach (var selectorNode in element.Elements(XmlTileSelectorModelTags.Ns + "choice-selection"))
            {
                var cd = ParseChoiceDefinition(selectorNode, context);
                retval.Choices.Add(cd);
            }

            return retval;
        }

        static ChoiceDefinition ParseChoiceDefinition(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var cd = new ChoiceDefinition();
            var graphicTag = element.Element(XmlTileSelectorModelTags.Ns + "choice-matches");
            cd.MatchedTags.ParseStringList(graphicTag);

            if (!element.ParseAnySelector(context).TryGetValue(out var selector))
            {
                throw new XmlParseException("Invalid selector", element);
            }

            cd.Selector = selector;
            return cd;
        }

        static XElement? WriteQuantityChoiceSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not QuantitySelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.QuantityChoice);
            foreach (var choice in m.Choices)
            {
                var choiceElement = new XElement(XmlTileSelectorModelTags.Ns + "quantity-selection");
                choiceElement.AddStringElement(XmlTileSelectorModelTags.Ns + "quantity", $"{choice.MatchedQuantity}");
                var selector = context.WriteSelector(choice.Selector);
                if (selector == null)
                {
                    throw new Exception();
                }

                choiceElement.Add(new XElement(XmlTileSelectorModelTags.Ns + "selector", selector));
                element.Add(choiceElement);
            }

            return element;
        }

        public static ISelectorModel ParseQuantityChoiceSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var retval = new QuantitySelectorModel();
            foreach (var selectorNode in element.Elements(XmlTileSelectorModelTags.Ns + "quantity-selection"))
            {
                var cd = ParseQuantityChoiceDefinition(selectorNode, context);
                retval.Choices.Add(cd);
            }

            return retval;
        }

        static QuantitySelectorDefinition ParseQuantityChoiceDefinition(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var cd = new QuantitySelectorDefinition();
            cd.MatchedQuantity= ((int?)element.Element(XmlTileSelectorModelTags.Ns + "quantity")) ?? throw new XmlParseException("Mandatory element 'quantity' is missing", element);

            if (!element.ParseAnySelector(context).TryGetValue(out var selector))
            {
                throw new XmlParseException("Invalid selector", element);
            }

            cd.Selector = selector;
            return cd;
        }

        static XElement? WriteNeighbourOverlaySelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not NeighbourOverlaySelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.NeighbourOverlay);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.AddStringElement(XmlTileSelectorModelTags.ForceGraphicTag, m.ForceGraphic);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.Ns + "match-self", XmlTileSelectorModelTags.MatchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.Ns + "match-with", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        static ISelectorModel ParseNeighbourOverlaySelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'context-data-set' is missing", element);
            var result = new NeighbourOverlaySelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.ForceGraphic = (string?)element.Element(XmlTileSelectorModelTags.ForceGraphicTag);
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-with"));
            return result;
        }

        static XElement? WriteCardinalSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not CardinalSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Cardinal);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.Ns + "match-self", XmlTileSelectorModelTags.MatchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.Ns + "match-with", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        static ISelectorModel ParseCardinalSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new CardinalSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-with"));
            return result;
        }

        static XElement? WriteRiverOutletSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not RiverOutletSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.RiverOutlet);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.Ns + "match-self", XmlTileSelectorModelTags.MatchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.Ns + "match-with", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        static ISelectorModel ParseRiverOutletSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new RiverOutletSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-with"));
            return result;
        }

        static XElement? WriteRoadCornerSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not RoadCornerSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.RoadCorner);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.Ns + "match-self", XmlTileSelectorModelTags.MatchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.Ns + "match-with", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        static ISelectorModel ParseRoadCornerSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new RoadCornerSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-with"));
            return result;
        }

        static XElement? WriteDiagonalSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not DiagonalSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.Diagonal);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.Ns + "match-self", XmlTileSelectorModelTags.MatchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.Ns + "match-with", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        static ISelectorModel ParseDiagonalSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new DiagonalSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-with"));
            return result;
        }

        static XElement? WriteRoadParitySelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not RoadParitySelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.Ns + BuiltInSelectors.RoadParity);
            element.AddStringElement(XmlTileSelectorModelTags.PrefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.ContextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.Ns + "match-self", XmlTileSelectorModelTags.MatchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.Ns + "match-with", XmlTileSelectorModelTags.MatchClassTag));
            return element;
        }

        static ISelectorModel ParseRoadParitySelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.PrefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.ContextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new RoadParitySelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.Ns + "match-with"));
            return result;
        }
    }
}