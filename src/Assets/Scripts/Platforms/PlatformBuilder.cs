using System;
using System.Linq;
using System.Collections;
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
            _heightLevelDescriptions = new Dictionary<string, PlatformRangeInfo> ();
			_heightLevelDescriptions.Add (new KeyValuePair<string, PlatformRangeInfo> ("One", new PlatformRangeInfo(10, 1, 1)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Two", new PlatformRangeInfo(20, 2, 2)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Three", new PlatformRangeInfo(30, 3, 3)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Four", new PlatformRangeInfo(40, 1, 5)));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Five", new PlatformRangeInfo(50, 1, 6)));
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
			    var lastRange = _heightLevelDescriptions.Last();
                return GetPrefab(lastRange.Value.prefabRangeStart, lastRange.Value.prefabRangeEnd);
			}
		}

		private string GetPrefab(int start, int end)
		{
			int range = Random.Range (start, end);
			string prefab = string.Format ("Prefabs/Platforms/{0}_Platform", range);
			return prefab;
		}
	
	}
}
