using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformSpawnControl : MonoBehaviour {

	private Transform _playerObject;
	private string _currentLevel;
	private IDictionary<string, GameObject> _levelPlatforms;
	private Transform _currentPlatformObject;
	private int _currentPlatform;

	public int maxPlatformsForLevel = 100;
	public int checkpointInterval = 3;
	public float startingYAxisValue = 1.0f;
	public int platformPlayerBuffer = 3;

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
		_levelPlatforms = new Dictionary<string, GameObject>();
	}

	// Use this for initialization
	void Start () {
		// spawn initial platforms on start
		float yAxisMultiplier = startingYAxisValue;
		for (int i=1; i<=platformPlayerBuffer; i++) 
		{

			GameObject initialPlatform = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/ProtoPlatformStandard"), 
			                                                      new Vector3 (0, 0, 0), Quaternion.identity);

			initialPlatform.transform.Translate(initialPlatform.transform.position.x, yAxisMultiplier, initialPlatform.transform.position.z);
			yAxisMultiplier += initialPlatform.transform.localScale.y + 2.1f;

			initialPlatform.name = string.Format("Platform_{0}", i);
			_levelPlatforms.Add (initialPlatform.name, initialPlatform);
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

		//for (int i=_currentPlatform; i<=_currentPlatform + platformPlayerBuffer; i++) {

			var nextPlatform = _currentPlatform + 1;
			GameObject newPlatform;
			if (!_levelPlatforms.TryGetValue (string.Format ("Platform_{0}", nextPlatform), out newPlatform)) 
			{
				//Debug.Log("Spawn platform: " + nextPlatform);
				float currentPlatformY = _currentPlatformObject.position.y;
				float yAxisMultiplier = _currentPlatformObject.localScale.y + 2.1f;
				newPlatform = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/ProtoPlatformStandard"), 
				                                      new Vector3 (0, currentPlatformY + yAxisMultiplier, 0), Quaternion.identity);
				newPlatform.name = string.Format("Platform_{0}", nextPlatform);
				
				_levelPlatforms.Add(string.Format("Platform_{0}", nextPlatform), newPlatform);
			}
		//}
	}

	void HandlePlatformReached(Transform platform)
	{
		//Debug.Log (platform.parent.name);
		if (platform.parent != null) 
		{
			_currentPlatformObject = _levelPlatforms[platform.parent.name].transform;
			_currentPlatform = int.Parse(platform.parent.name.Split('_')[1]);
			//Debug.Log ("Current platform:" + _currentPlatform);

		}
	}
}
