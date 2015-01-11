using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace LegendPeak.Platforms
{
    public class PlatformBuilder
    {
        //private static
        /// <summary>
        /// The _height level descriptions.
        /// </summary>
        private readonly IDictionary<string, PlatformRangeInfo> _heightLevelDescriptions;

        public PlatformBuilder()
        {
            _heightLevelDescriptions = new Dictionary<string, PlatformRangeInfo>();
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("ForestSlowLimit", new PlatformRangeInfo(20, 0, 1)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("ForestFastLimit", new PlatformRangeInfo(80, 0, 2)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("CloudSlowLimit", new PlatformRangeInfo(130, 0, 4)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("CloudFast", new PlatformRangeInfo(180, 0, 4)));
			_heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("StratosphereSlowLimit", new PlatformRangeInfo(240, 0, 6)));
			_heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("StratosphereFastLimit", new PlatformRangeInfo(300, 0, 6)));

        }

        public int GetPlatformPrefabByNumber(int platformNumber)
        {
            var platformDescription = _heightLevelDescriptions.FirstOrDefault(i => i.Value.heightRange >= platformNumber);
            if (platformDescription.Value != null)
            {
                return GetPrefab(platformDescription.Value.prefabRangeStart, platformDescription.Value.prefabRangeEnd);
            }
            // if there are no defined height levels, generate the last range forever
            else
            {
				var lastRange = _heightLevelDescriptions.First(i => i.Key == "StratosphereFastLimit");
                return GetPrefab(lastRange.Value.prefabRangeStart, lastRange.Value.prefabRangeEnd);
            }
        }

        private int GetPrefab(int start, int end)
        {
            return Random.Range(start, end);
            //string prefab = string.Format("Prefabs/Platforms/{0}_Platform_Wood", range);
            //return prefab;
        }

    }
}
