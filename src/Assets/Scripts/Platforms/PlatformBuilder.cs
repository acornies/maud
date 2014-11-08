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
            //_heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("ForestSlowLimit", new PlatformRangeInfo(50, 1, 3)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("ForestFastLimit", new PlatformRangeInfo(100, 1, 4)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("CloudSlowLimit", new PlatformRangeInfo(200, 1, 5)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("CloudFast", new PlatformRangeInfo(300, 1, 6)));
			_heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("CloudFastLimit", new PlatformRangeInfo(400, 1, 7)));

        }

        public string GetPlatformPrefabByNumber(int platformNumber)
        {
            var platformDescription = _heightLevelDescriptions.FirstOrDefault(i => i.Value.heightRange >= platformNumber);
            if (platformDescription.Value != null)
            {
                return GetPrefab(platformDescription.Value.prefabRangeStart, platformDescription.Value.prefabRangeEnd);
            }
            // if there are no defined height levels, generate the last range forever
            else
            {
                var lastRange = _heightLevelDescriptions.First(i => i.Key == "CloudFastLimit");
                return GetPrefab(lastRange.Value.prefabRangeStart, lastRange.Value.prefabRangeEnd);
            }
        }

        private string GetPrefab(int start, int end)
        {
            int range = Random.Range(start, end);
            string prefab = string.Format("Prefabs/Platforms/{0}_Platform_Wood", range);
            return prefab;
        }

    }
}
