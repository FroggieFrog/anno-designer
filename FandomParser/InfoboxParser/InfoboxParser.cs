using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FandomParser.Core;
using FandomParser.Core.Models;
using InfoboxParser.Models;
using InfoboxParser.Parser;

[assembly: InternalsVisibleTo("InfoboxParser.Tests")]

namespace InfoboxParser
{
    public class InfoboxParser
    {
        private readonly ICommons _commons;
        private readonly ITitleParserSingle _titleParserSingle;
        private readonly ISpecialBuildingNameHelper _specialBuildingNameHelper;
        private readonly IRegionHelper _regionHelper;

        private readonly IParser parserSingleRegion;
        private readonly IParser parserOldAndNewWorld;
        private readonly IParserMultipleRegions parserMultipleRegions;
        private readonly List<string> possibleRegions_2Regions;
        private readonly List<string> possibleRegions_3Regions;
        private readonly List<string> possibleRegions_4Regions;

        public InfoboxParser(ICommons commonsToUse, ITitleParserSingle titleParserSingleToUse, ISpecialBuildingNameHelper specialBuildingNameHelperToUse, IRegionHelper regionHelperToUse)
        {
            _commons = commonsToUse;
            _titleParserSingle = titleParserSingleToUse;
            _specialBuildingNameHelper = specialBuildingNameHelperToUse;
            _regionHelper = regionHelperToUse;

            parserSingleRegion = new ParserSingleRegion(_commons, _titleParserSingle);
            parserOldAndNewWorld = new ParserOldAndNewWorld(_commons, _specialBuildingNameHelper, _regionHelper);
            parserMultipleRegions = new ParserMultipleRegions(_commons, _specialBuildingNameHelper, _regionHelper);
            possibleRegions_2Regions = new List<string> { "A", "B" };
            possibleRegions_3Regions = new List<string> { "A", "B", "C" };
            possibleRegions_4Regions = new List<string> { "A", "B", "C", "D" };
        }

        public List<IInfobox> GetInfobox(string wikiText)
        {
            if (string.IsNullOrWhiteSpace(wikiText))
            {
                return null;
            }

            var result = new List<IInfobox>();


            if (wikiText.StartsWith(_commons.InfoboxTemplateStartOldAndNewWorld))
            {
                var infoboxes = parserOldAndNewWorld.GetInfobox(wikiText);

                result.AddRange(infoboxes);
            }
            else if (wikiText.StartsWith(_commons.InfoboxTemplateStart2Regions))
            {
                var infoboxes = parserMultipleRegions.GetInfobox(wikiText, possibleRegions_2Regions);

                result.AddRange(infoboxes);
            }
            else if (wikiText.StartsWith(_commons.InfoboxTemplateStart3Regions))
            {
                var infoboxes = parserMultipleRegions.GetInfobox(wikiText, possibleRegions_3Regions);

                result.AddRange(infoboxes);
            }
            else if (wikiText.StartsWith(_commons.InfoboxTemplateStart4Regions))
            {
                var infoboxes = parserMultipleRegions.GetInfobox(wikiText, possibleRegions_4Regions);

                result.AddRange(infoboxes);
            }
            else if (wikiText.StartsWith(_commons.InfoboxTemplateStart))
            {
                var infoboxes = parserSingleRegion.GetInfobox(wikiText);

                result.AddRange(infoboxes);
            }

            return result;
        }
    }
}
