using SharpTileRenderer.TileMatching.Model.Selectors;
using System;
using System.Linq;
using System.Xml.Linq;

namespace SharpTileRenderer.Xml.TileMatching
{
    public static class BuiltInXmlSelectorHandlers
    {
        public static TSettings ConfigureBuiltInSelectorReadHandlers<TSettings>(this TSettings c)
            where TSettings : IXmlTileMatcherModelReaderSettings
        {
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.Basic, ParseBasicSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.Corner, ParseCornerSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.CellGroup, ParseCellGroupSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.List, ParseListSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.Choice, ParseChoiceSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.NeighbourOverlay, ParseNeighbourOverlaySelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.Cardinal, ParseCardinalSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.Diagonal, ParseDiagonalSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.RiverOutlet, ParseRiverOutletSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.RoadCorner, ParseRoadCornerSelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.RoadParity, ParseRoadParitySelectorModel);
            c.RegisterSelectorHandler(XmlTileSelectorModelTags.NS + BuiltInSelectors.QuantityChoice, ParseQuantityChoiceSelectorModel);
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

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.Basic);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            return element;
        }

        public static ISelectorModel ParseBasicSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            return new BasicSelectorModel()
            {
                Prefix = prefix
            };
        }

        static XElement? WriteCornerSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not CornerSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.Corner);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.AddStringElement(XmlTileSelectorModelTags.defaultClassTag, m.DefaultClass);
            element.Add(m.Matches.AddStringList(XmlTileSelectorModelTags.NS + "matches", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        public static ISelectorModel ParseCornerSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var defaultClass = (string?)element.Element(XmlTileSelectorModelTags.defaultClassTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new CornerSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
                DefaultClass = defaultClass
            };
            
            element.ParseStringListElement(XmlTileSelectorModelTags.NS + "matches", XmlTileSelectorModelTags.matchClassTag, result.Matches);
            return result;
        }

        static XElement? WriteCellGroupSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not CellGroupSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.CellGroup);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.AddStringElement(XmlTileSelectorModelTags.defaultClassTag, m.DefaultClass);
            element.Add(m.Matches.AddStringList(XmlTileSelectorModelTags.NS + "matches", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        public static ISelectorModel ParseCellGroupSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var defaultClass = (string?)element.Element(XmlTileSelectorModelTags.defaultClassTag);
            var direction = element.ParseEnumElement<CellGroupNavigationDirection>(XmlTileSelectorModelTags.NS + "direction") ?? CellGroupNavigationDirection.Up;
            var retval = new CellGroupSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
                DefaultClass = defaultClass,
                Direction = direction,
            };
            element.ParseStringListElement(XmlTileSelectorModelTags.NS + "matches", XmlTileSelectorModelTags.matchClassTag, retval.Matches);
            return retval;
        }

        static XElement? WriteListSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not ListSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.List);
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

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.Choice);
            foreach (var choice in m.Choices)
            {
                var choiceElement = new XElement(XmlTileSelectorModelTags.NS + "choice-selection");
                choiceElement.Add(choice.MatchedTags.AddStringList(XmlTileSelectorModelTags.NS + "choice-matches", XmlTileSelectorModelTags.matchClassTag));
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
            foreach (var selectorNode in element.Elements(XmlTileSelectorModelTags.NS + "choice-selection"))
            {
                var cd = ParseChoiceDefinition(selectorNode, context);
                retval.Choices.Add(cd);
            }

            return retval;
        }

        static ChoiceDefinition ParseChoiceDefinition(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var cd = new ChoiceDefinition();
            var graphicTag = element.Element(XmlTileSelectorModelTags.NS + "choice-matches");
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

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.QuantityChoice);
            foreach (var choice in m.Choices)
            {
                var choiceElement = new XElement(XmlTileSelectorModelTags.NS + "quantity-selection");
                choiceElement.AddStringElement(XmlTileSelectorModelTags.NS + "quantity", $"{choice.MatchedQuantity}");
                var selector = context.WriteSelector(choice.Selector);
                if (selector == null)
                {
                    throw new Exception();
                }

                choiceElement.Add(new XElement(XmlTileSelectorModelTags.NS + "selector", selector));
                element.Add(choiceElement);
            }

            return element;
        }

        public static ISelectorModel ParseQuantityChoiceSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var retval = new QuantitySelectorModel();
            foreach (var selectorNode in element.Elements(XmlTileSelectorModelTags.NS + "quantity-selection"))
            {
                var cd = ParseQuantityChoiceDefinition(selectorNode, context);
                retval.Choices.Add(cd);
            }

            return retval;
        }

        static QuantitySelectorDefinition ParseQuantityChoiceDefinition(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var cd = new QuantitySelectorDefinition();
            cd.MatchedQuantity= ((int?)element.Element(XmlTileSelectorModelTags.NS + "quantity")) ?? throw new XmlParseException("Mandatory element 'quantity' is missing", element);

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

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.NeighbourOverlay);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.NS + "match-self", XmlTileSelectorModelTags.matchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.NS + "match-with", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        static ISelectorModel ParseNeighbourOverlaySelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'context-data-set' is missing", element);
            var result = new NeighbourOverlaySelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-with"));
            return result;
        }

        static XElement? WriteCardinalSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not CardinalSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.Cardinal);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.NS + "match-self", XmlTileSelectorModelTags.matchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.NS + "match-with", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        static ISelectorModel ParseCardinalSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new CardinalSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-with"));
            return result;
        }

        static XElement? WriteRiverOutletSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not RiverOutletSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.RiverOutlet);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.NS + "match-self", XmlTileSelectorModelTags.matchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.NS + "match-with", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        static ISelectorModel ParseRiverOutletSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new RiverOutletSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-with"));
            return result;
        }

        static XElement? WriteRoadCornerSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not RoadCornerSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.RoadCorner);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.NS + "match-self", XmlTileSelectorModelTags.matchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.NS + "match-with", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        static ISelectorModel ParseRoadCornerSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new RoadCornerSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-with"));
            return result;
        }

        static XElement? WriteDiagonalSelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not DiagonalSelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.Diagonal);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.NS + "match-self", XmlTileSelectorModelTags.matchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.NS + "match-with", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        static ISelectorModel ParseDiagonalSelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new DiagonalSelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-with"));
            return result;
        }

        static XElement? WriteRoadParitySelectorModel(ISelectorModel model, IXmlTileMatcherModelWriterContext context)
        {
            if (model is not RoadParitySelectorModel m)
            {
                return null;
            }

            var element = new XElement(XmlTileSelectorModelTags.NS + BuiltInSelectors.RoadParity);
            element.AddStringElement(XmlTileSelectorModelTags.prefixTag, m.Prefix);
            element.AddStringElement(XmlTileSelectorModelTags.contextDataSetTag, m.ContextDataSet);
            element.Add(m.MatchSelf.AddStringList(XmlTileSelectorModelTags.NS + "match-self", XmlTileSelectorModelTags.matchClassTag));
            element.Add(m.MatchWith.AddStringList(XmlTileSelectorModelTags.NS + "match-with", XmlTileSelectorModelTags.matchClassTag));
            return element;
        }

        static ISelectorModel ParseRoadParitySelectorModel(XElement element, IXmlTileMatcherModelReaderContext context)
        {
            var prefix = (string?)element.Element(XmlTileSelectorModelTags.prefixTag);
            var matchDataSet = (string?)element.Element(XmlTileSelectorModelTags.contextDataSetTag) ?? throw new XmlParseException("Mandatory element 'match-data-set' is missing", element);
            var result = new RoadParitySelectorModel()
            {
                Prefix = prefix,
                ContextDataSet = matchDataSet,
            };
            result.MatchSelf.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-self"));
            result.MatchWith.ParseStringList(element.Element(XmlTileSelectorModelTags.NS + "match-with"));
            return result;
        }
    }
}