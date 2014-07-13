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
		//var currentLevel = Application.loadedLevelName;
		Debug.Log (_currentLevel);
		GameObject firstPlatform = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/ProtoPlatformStandard"), 
		                                                   new Vector3 (0, startingYAxisValue, 0), Quaternion.identity);
		firstPlatform.name = "Platform_1";
		_levelPlatforms.Add (firstPlatform.name, firstPlatform);
		//Debug.Log (_levelPlatforms);
	}
	
	// Update is called once per frame
	void Update () {

		SpawnPlatforms ();
	}

	void SpawnPlatforms()
	{
		//Debug.Log (_currentPlatformInstance);
		var nextPlatform = _currentPlatform + 1;
		GameObject newPlatform;
		if (!_levelPlatforms.TryGetValue (string.Format ("Platform_{0}", nextPlatform), out newPlatform)) 
		{
			//Debug.Log("Spawn next platform");
			float currentPlatformY = _currentPlatformObject.position.y;
			float yAxisMultiplier = _currentPlatformObject.localScale.y + 2.1f;
			newPlatform = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/ProtoPlatformStandard"), 
			                                      new Vector3 (0, currentPlatformY + yAxisMultiplier, 0), Quaternion.identity);
			newPlatform.name = string.Format("Platform_{0}", nextPlatform);

			_levelPlatforms.Add(string.Format("Platform_{0}", nextPlatform), newPlatform);
		}

	}

	void HandlePlatformReached(Transform platform)
	{
		//Debug.Log (platform.parent.name);
		if (platform.parent != null) 
		{
			_currentPlatformObject = _levelPlatforms[platform.parent.name].transform;
			_currentPlatform = int.Parse(platform.parent.name.Split('_')[1]);
		}
		//reachedPlatform = _levelPlatforms[platform.transform.parent.name].name;
		//Debug.Log (reachedPlatform);
		//print (_levelPlatforms);
		/*GameObject platform;
		if (_levelPlatforms.TryGetValue(platformInstance, out platform))
		{
			Debug.Log("got key");
			reachedPlatform = platform.GetInstanceID();
		}
		else 
		{
			Debug.Log("doh");
		}*/
		//currentReachedPlatform = platform..
	}
}
