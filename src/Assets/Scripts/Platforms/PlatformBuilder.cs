using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace LegendPeak.Platforms
{
	public class PlatformBuilder 
	{
		//private static
		/// <summary>
		/// The _height level descriptions.
		/// </summary>
		private IDictionary<string, PlatformRangeInfo> _heightLevelDescriptions;

		public PlatformBuilder()
		{
			_heightLevelDescriptions = new Dictionary<string, PlatformRangeInfo> ();
			_heightLevelDescriptions.Add (new KeyValuePair<string, PlatformRangeInfo> ("Lowest", new PlatformRangeInfo(20, 4)));
			_heightLevelDescriptions.Add (new KeyValuePair<string, PlatformRangeInfo> ("Lower", new PlatformRangeInfo(40, 5)));
		}

		public string GetPlatformPrefabByNumber(int platformNumber)
		{
			var platformDescription = _heightLevelDescriptions.FirstOrDefault(i => i.Value.heightRange >= platformNumber);
			if (platformDescription.Value != null)
			{
				return GetPrefab(platformDescription.Value.prefabRange);
			}
			// if there are no defined height levels, generate the last range forever
			else
			{
				return GetPrefab(_heightLevelDescriptions.Last().Value.prefabRange);
			}
		}

		private string GetPrefab(int prefabRange)
		{
			int range = Random.Range (1, prefabRange);
			string prefab = string.Format ("Prefabs/Platforms/{0}_Platform", range);
			return prefab;
		}
	
	}
}
