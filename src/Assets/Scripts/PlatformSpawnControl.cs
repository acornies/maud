using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlatformSpawnControl : MonoBehaviour 
{
	//private Transform _currentPlatformObject;
	private int _currentPlatform;
	private PlatformBuilder _platformBuilder;

	public static PlatformSpawnControl Instance { get; private set;}
	public IDictionary<int, GameObject> levelPlatforms { get; private set;}
	public int maxPlatformsForLevel = 100;
	public int checkpointBuffer = 3;
	public float startingYAxisValue = 1.0f;
	public int platformSpawnBuffer = 3;
	public float platformSpacing = 2.1f;

	public delegate void ReachedNextCheckpoint(int platform);
	public static event ReachedNextCheckpoint On_ReachedCheckpoint;

	public delegate void NewPlatform(float yPosition);
	public static event NewPlatform On_NewPlatform;

	// Subscribe to events
	void OnEnable()
	{
		PlayerMovement.On_PlatformReached += HandlePlatformReached;
		CameraMovement.On_DestroyLowerPlatforms += HandleDestroyLowerPlatforms;
	}

	void OnDisable()
	{
		UnsubscribeEvent();
	}
	
	void OnDestroy()
	{
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent()
	{
		PlayerMovement.On_PlatformReached -= HandlePlatformReached;
		CameraMovement.On_DestroyLowerPlatforms -= HandleDestroyLowerPlatforms;
	}

	void Awake()
	{
		if (Instance == null)
		{
			//DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		_platformBuilder = new PlatformBuilder ();
		levelPlatforms = new Dictionary<int, GameObject>();

	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		SpawnPlatforms ();
	}
	
	// Spawn platforms based on player location
	void SpawnPlatforms()
	{
		if (levelPlatforms.Count == maxPlatformsForLevel) return;

		var highestPlatformIndex = (levelPlatforms.Keys.Count > 0) ? levelPlatforms.Keys.Max() : 0;
		//Debug.Log("Highest platform is: " + highestPlatformIndex);

		if (_currentPlatform + platformSpawnBuffer >= highestPlatformIndex) 
		{
			//GameObject highestPlatformObj;
			GameObject highestPlatform = null;
			float yAxisMultiplier;
			levelPlatforms.OrderBy(p => p.Key);

			int nextPlatform = highestPlatformIndex + 1;
			if (levelPlatforms.TryGetValue(highestPlatformIndex, out highestPlatform))
			{
				//= _levelPlatforms [_levelPlatforms.Count].transform;
				yAxisMultiplier = highestPlatform.transform.position.y + highestPlatform.transform.localScale.y + platformSpacing;
			}
			else
			{
				yAxisMultiplier = startingYAxisValue;
			}
			
			int toRange = nextPlatform + platformSpawnBuffer;
			for (int i=nextPlatform; i<=toRange; i++) {

				GameObject newPlatform;

				if (!levelPlatforms.TryGetValue (i, out newPlatform)) 
				{
					Debug.Log ("Spawn platform: " + i + " of " + toRange);
					/*newPlatform = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/ProtoPlatformStandard"), 
                                              new Vector3 (0, yAxisMultiplier, 0), Quaternion.identity);*/

					newPlatform = (GameObject)Instantiate (Resources.Load<GameObject> (_platformBuilder.GetPlatformPrefabByNumber(i)), 
					                                       new Vector3 (0, yAxisMultiplier, 0), Quaternion.identity);

					//newPlatform.transform.Translate (newPlatform.transform.position.x, yAxisMultiplier, newPlatform.transform.position.z);
					yAxisMultiplier += newPlatform.transform.localScale.y + platformSpacing;

					newPlatform.name = string.Format ("Platform_{0}", i);
					levelPlatforms.Add (i, newPlatform);

					On_NewPlatform(newPlatform.transform.position.y);
				}
			}
		}
	}

	void HandlePlatformReached(Transform platform)
	{
		//Debug.Log (platform.parent.name);
		if (platform.parent != null) 
		{
			_currentPlatform = int.Parse(platform.parent.name.Split('_')[1]);
			//_currentPlatformObject = levelPlatforms[_currentPlatform].transform;

			Debug.Log ("Current platform: " + _currentPlatform);

			On_ReachedCheckpoint(_currentPlatform - checkpointBuffer);
		}
	}

	void HandleDestroyLowerPlatforms(int platformIndex)
	{
		//Debug.Log ("Destroy platforms under: " + platformIndex);

		var buffer = new List<int> ();
		foreach (var platform in levelPlatforms) 
		{
			if (platform.Key <= platformIndex) 
			{
				buffer.Add(platform.Key);
			}
		}

		foreach (var item in buffer) 
		{
			levelPlatforms.Remove(item);
			Destroy(GameObject.Find("Platform_" + item));
			Debug.Log("Removed Platform_" + item);
		}


	}
}
