﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LegendPeak.Platforms;

public class PlatformController : MonoBehaviour 
{
	//private Transform _currentPlatformObject;
	private int _currentPlatform;
	private PlatformBuilder _platformBuilder;
	
	public static PlatformController Instance { get; private set;}
	public IDictionary<int, GameObject> levelPlatforms { get; private set;}
	//public int maxPlatformsForLevel = 100;
	public int checkpointBuffer = 3;
	public float startingYAxisValue = 1.0f;
	public int platformSpawnBuffer = 3;
	public float platformSpacing = 2.1f;
	public float maxRotationLeft = 50.0f;
	public float maxRotationRight = 310.0f;
	
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
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		SpawnPlatforms ();
	}
	
	// Spawn platforms based on player location
	void SpawnPlatforms()
	{
		//if (levelPlatforms.Count == maxPlatformsForLevel) return;
		
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
					
					newPlatform = (GameObject)Instantiate (Resources.Load<GameObject> (_platformBuilder.GetPlatformPrefabByNumber(i)), 
					                                       new Vector3 (0, yAxisMultiplier, 0), Quaternion.identity);

					yAxisMultiplier += newPlatform.transform.localScale.y + platformSpacing;
					
					newPlatform.name = string.Format ("Platform_{0}", i);
					newPlatform.transform.localRotation = Quaternion.AngleAxis(GetRandomRotation(), Vector3.up);
					levelPlatforms.Add (i, newPlatform);
					
					On_NewPlatform(newPlatform.transform.position.y);
				}
			}
		}
	}

	private float GetRandomRotation()
	{
		var options = new Dictionary<int, float> ();
		options.Add (1, maxRotationLeft);
		options.Add (2, maxRotationRight);
		var leftOrRight = Random.Range (1, 3);
		if (leftOrRight == 1) 
		{
			return Random.Range(0, options [leftOrRight]);
		}
		else
		{
			return Random.Range(360f, options [leftOrRight]);
		}
	}
	
	void HandlePlatformReached(Transform platform)
	{
		//Debug.Log (platform.parent.name);
		if (platform.parent != null) 
		{
			_currentPlatform = int.Parse(platform.parent.name.Split('_')[1]);
			
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
			//Debug.Log("Removed Platform_" + item);
		}
		
		
	}
}
