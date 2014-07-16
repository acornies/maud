﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformSpawnControl : MonoBehaviour {
	
	private string _currentLevel;
	private Transform _currentPlatformObject;
	private int _currentPlatform;

	public static PlatformSpawnControl Instance { get; private set;}
	public IDictionary<int, GameObject> levelPlatforms { get; private set;}
	public int maxPlatformsForLevel = 100;
	public int checkpointBuffer = 3;
	public float startingYAxisValue = 1.0f;
	public int platformSpawnBuffer = 3;
	public float platformSpacing = 2.1f;

	public delegate void ReachedNextCheckpoint(int platform);
	public static event ReachedNextCheckpoint On_ReachedCheckpoint;

	// Subscribe to events
	void OnEnable()
	{
		PlayerMovement.On_PlatformReached += HandlePlatformReached;
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

		_currentLevel = Application.loadedLevelName;
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

		if (_currentPlatform + platformSpawnBuffer >= levelPlatforms.Count) 
		{
			int nextPlatform = levelPlatforms.Count + 1;
			//GameObject highestPlatformObj;
			GameObject highestPlatform = null;
			float yAxisMultiplier;
			if (levelPlatforms.TryGetValue(levelPlatforms.Count, out highestPlatform))
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

				if (!levelPlatforms.TryGetValue (i, out newPlatform)) {
						Debug.Log ("Spawn platform: " + i + " of " + toRange);
						newPlatform = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/ProtoPlatformStandard"), 
	                                              new Vector3 (0, yAxisMultiplier, 0), Quaternion.identity);

						//newPlatform.transform.Translate (newPlatform.transform.position.x, yAxisMultiplier, newPlatform.transform.position.z);
						yAxisMultiplier += newPlatform.transform.localScale.y + platformSpacing;

						newPlatform.name = string.Format ("Platform_{0}", i);
						levelPlatforms.Add (i, newPlatform);
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
			_currentPlatformObject = levelPlatforms[_currentPlatform].transform;

			Debug.Log ("Current platform: " + _currentPlatform);

			On_ReachedCheckpoint(_currentPlatform - checkpointBuffer);

		}
	}
}
