using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformSpawnControl : MonoBehaviour {

	private Transform _playerObject;
	private string _currentLevel;
	private IDictionary<int, GameObject> _levelPlatforms;
	private Transform _currentPlatformObject;
	private int _currentPlatform;

	public int maxPlatformsForLevel = 100;
	public int checkpointInterval = 3;
	public float startingYAxisValue = 1.0f;
	public int platformBuffer = 3;
	public float platformSpacing = 2.1f;

	// Subscribe to events
	void OnEnable(){
		//EasyJoystick.On_JoystickTouchUp += On_JoystickTap;
		PlayerMovement.On_PlatformReached += HandlePlatformReached;
	}
	
	void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		//EasyJoystick.On_JoystickTouchUp -= On_JoystickTap;
		PlayerMovement.On_PlatformReached -= HandlePlatformReached;
	}

	void Awake()
	{
		_playerObject = GameObject.Find ("Player").transform;
		_currentLevel = Application.loadedLevelName;
		_levelPlatforms = new Dictionary<int, GameObject>();
	}

	// Use this for initialization
	void Start () {
		// spawn initial platforms on start
		float yAxisMultiplier = startingYAxisValue;
		for (int i=1; i<=3; i++) 
		{

			GameObject initialPlatform = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/ProtoPlatformStandard"), 
			                                                      new Vector3 (0, 0, 0), Quaternion.identity);

			initialPlatform.transform.Translate(initialPlatform.transform.position.x, yAxisMultiplier, initialPlatform.transform.position.z);
			yAxisMultiplier += initialPlatform.transform.localScale.y + platformSpacing;

			initialPlatform.name = string.Format("Platform_{0}", i);
			_levelPlatforms.Add (i, initialPlatform);
		}
	}
	
	// Update is called once per frame
	void Update () {

		SpawnPlatforms ();
	}
	
	// Spawn platforms based on player location
	void SpawnPlatforms()
	{
		if (_levelPlatforms.Count == maxPlatformsForLevel) return;

		if (_currentPlatform + platformBuffer >= _levelPlatforms.Count + 1) 
		{
			var nextPlatform = _levelPlatforms.Count + 1;
			var highestPlatform = _levelPlatforms [_levelPlatforms.Count].transform;
			float yAxisMultiplier = highestPlatform.position.y + highestPlatform.localScale.y + platformSpacing;
			int toRange = nextPlatform + platformBuffer;
			for (int i=nextPlatform; i<=toRange; i++) {

				GameObject newPlatform;

				if (!_levelPlatforms.TryGetValue (i, out newPlatform)) {
						Debug.Log ("Spawn platforms: " + i + ", " + toRange);
						newPlatform = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/ProtoPlatformStandard"), 
	                                              new Vector3 (0, yAxisMultiplier, 0), Quaternion.identity);

						//newPlatform.transform.Translate (newPlatform.transform.position.x, yAxisMultiplier, newPlatform.transform.position.z);
						yAxisMultiplier += newPlatform.transform.localScale.y + platformSpacing;

						newPlatform.name = string.Format ("Platform_{0}", i);
						_levelPlatforms.Add (i, newPlatform);
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
			_currentPlatformObject = _levelPlatforms[_currentPlatform].transform;

			Debug.Log ("Current platform:" + _currentPlatform);

		}
	}
}
