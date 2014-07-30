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

	    private IList<int> _platformPrefabs;

	    public PlatformBuilder()
	    {
	    }

	    public PlatformBuilder(int maxPlatformPrefabRange)
		{
	        _platformPrefabs = new List<int>();
            for (int i = 1; i <= maxPlatformPrefabRange; i++)
	        {
	            _platformPrefabs.Add(i);
	        }
            
            _heightLevelDescriptions = new Dictionary<string, PlatformRangeInfo> ();
			_heightLevelDescriptions.Add (new KeyValuePair<string, PlatformRangeInfo> ("One", new PlatformRangeInfo(1, _platformPrefabs.TakeWhile(i => i >= 1 && i < 2))));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Two", new PlatformRangeInfo(15, _platformPrefabs.TakeWhile(i => i >= 1 && i < 3))));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Three", new PlatformRangeInfo(30, _platformPrefabs.TakeWhile(i => i >= 1 && i < 4))));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Four", new PlatformRangeInfo(40, _platformPrefabs.TakeWhile(i => i >= 1 && i < 5))));
            _heightLevelDescriptions.Add(new KeyValuePair<string, PlatformRangeInfo>("Five", new PlatformRangeInfo(50, _platformPrefabs.TakeWhile(i => i >= 1 && i < 6))));
		}

		public string GetPlatformPrefabByNumber(int platformNumber)
		{
			var platformDescription = _heightLevelDescriptions.FirstOrDefault(i => i.Value.heightRange >= platformNumber);
			if (platformDescription.Value != null)
			{
				return GetPrefab(platformDescription.Value.prefabRange.ToList());
			}
			// if there are no defined height levels, generate the last range forever
			else
			{
				return GetPrefab(_heightLevelDescriptions.Last().Value.prefabRange.ToList());
			}
		}

		private string GetPrefab(IList<int> prefabRange)
		{
			int range = Random.Range (prefabRange.First(), prefabRange.Last());
			string prefab = string.Format ("Prefabs/Platforms/{0}_Platform", range);
			return prefab;
		}
	
	}
}
