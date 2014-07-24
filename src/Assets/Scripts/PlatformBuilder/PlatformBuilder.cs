using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlatformBuilder 
{
	//private static
	/// <summary>
	/// The _height level descriptions.
	/// </summary>
	private IDictionary<string, int> _heightLevelDescriptions;

	public PlatformBuilder()
	{
		_heightLevelDescriptions = new Dictionary<string, int> ();
		_heightLevelDescriptions.Add (new KeyValuePair<string, int> ("Lowest", 100));
		_heightLevelDescriptions.Add (new KeyValuePair<string, int> ("Lower", 200));
	}

	public string GetPlatformPrefabByNumber(int platformNumber)
	{
		var platformDescription = _heightLevelDescriptions.FirstOrDefault(i => i.Value >= platformNumber);
		int range = Random.Range (1, 4);
		string prefab = string.Format ("Prefabs/{0}/{1}_{2}", platformDescription.Key, range, platformDescription.Key);
		return prefab;
	}

	// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame
	//void Update () {
	
	//}
}
