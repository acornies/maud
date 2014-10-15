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
            //_heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("One", new PlatformRangeInfo(5, 1, 1)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Two", new PlatformRangeInfo(20, 1, 2)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Three", new PlatformRangeInfo(40, 1, 3)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Four", new PlatformRangeInfo(80, 1, 4)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Six", new PlatformRangeInfo(120, 1, 5)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Seven", new PlatformRangeInfo(180, 1, 6)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Eight", new PlatformRangeInfo(220, 2, 6)));
            //_heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Nine", new PlatformRangeInfo(160, 4, 6)));
            //_heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Ten", new PlatformRangeInfo(180, 5, 6)));
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
                var lastRange = _heightLevelDescriptions.First(i => i.Key == "Eight");
                return GetPrefab(lastRange.Value.prefabRangeStart, lastRange.Value.prefabRangeEnd);
            }
        }

        private string GetPrefab(int start, int end)
        {
            int range = Random.Range(start, end);
            string prefab = string.Format("Prefabs/Platforms/{0}_Platform", range);
            return prefab;
        }

    }
}
