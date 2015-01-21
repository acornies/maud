using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using LegendPeak;
using LegendPeak.Platforms;

public class PlatformController : MonoBehaviour
{
	private Image _timedDestroyAlert;

	public int TutorialHoldPlatform = 59;
    //private Transform _currentPlatformObject;
    private int _currentPlatform;
    private PlatformBuilder _platformBuilder;
    private Vector3[] _orbitAxis;
    private float _timedDestroyTimer;

    public static PlatformController Instance { get; private set; }
    public IDictionary<int, GameObject> levelPlatforms { get; private set; }
    //public int maxPlatformsForLevel = 100;
    public int checkpointBuffer = 3;
    public float startingYAxisValue = 1.0f;
    public int platformSpawnBuffer = 3;
    public float platformSpacing = 2.1f;
    public float maxRotationLeft = 50.0f;
    public float maxRotationRight = 310.0f;
    public float platformSpawnInterval = 1.0f;
    public float trunkZAxis = 3.8f;

    public GameObject[] platformTypes;

	public float timedDestroySpeed = 1f;
	public bool useTimedDestroy;
    public GameObject timedDestroyCameraTarget;

    public delegate void ReachedNextCheckpoint(int platform);
    public static event ReachedNextCheckpoint On_ReachedCheckpoint;

    public delegate void NewPlatform(float yPosition, float maxYCamera);
    public static event NewPlatform On_NewPlatform;

	public delegate void TimedDestroy(GameObject objectToDestroy);
	public static event TimedDestroy OnTimedDestroy;

	public delegate void TimedDestroyGameOver();
	public static event TimedDestroyGameOver OnTimedDestroyGameOver;

    // Subscribe to events
    void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandlePlatformReached;
        CameraMovement.On_DestroyLowerPlatforms += HandleDestroyLowerPlatforms;
		CameraMovement.OnMovePlayerToGamePosition += HandleMovePlayerToGamePosition;
		MusicController.OnFastMusicStart += HandleOnFastMusicStart;
		MusicController.OnFastMusicStop += HandleOnFastMusicStop;
		GameController.OnGameOver += HandleOnGameOver;
		GameController.OnPlayerReward += HandleOnPlayerReward;
    }

    void HandleOnFastMusicStop ()
    {
		useTimedDestroy = false;
    }

    void HandleOnFastMusicStart (float timedSpeed)
    {
		if (GameController.Instance.gameState == LegendPeak.GameState.Over) return;

		useTimedDestroy = true;
		timedDestroySpeed = timedSpeed;

        var bottom = levelPlatforms.Keys.Min() + 1;
		var buffer = levelPlatforms [bottom];
        
		//_timedDestroyTimer = timedDestroySpeed;
		// TODO move to camera movement
		if (timedDestroyCameraTarget == null)
		{
			timedDestroyCameraTarget = new GameObject("TimedDestroyCameraTarget");
		}
        timedDestroyCameraTarget.transform.position = buffer.transform.position;
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
		CameraMovement.OnMovePlayerToGamePosition -= HandleMovePlayerToGamePosition;
		MusicController.OnFastMusicStart -= HandleOnFastMusicStart;
		MusicController.OnFastMusicStop -= HandleOnFastMusicStop;
		GameController.OnGameOver -= HandleOnGameOver;
		GameController.OnPlayerReward -= HandleOnPlayerReward;
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

        _platformBuilder = new PlatformBuilder();
        levelPlatforms = new Dictionary<int, GameObject>();

        _orbitAxis = new Vector3[2];
        _orbitAxis[0] = Vector3.up;
        _orbitAxis[1] = Vector3.down;

		_timedDestroyAlert = GameObject.Find ("DestroyAlert").GetComponent<Image>();
    }

    // Use this for initialization
    IEnumerator Start()
    {
        var startingPlatforms = GameObject.FindGameObjectsWithTag("Rotatable");
        foreach (var platform in startingPlatforms)
        {
            var key = platform.name.Split('_')[1];
            var value = platform;
            levelPlatforms.Add(int.Parse(key), value);
        }
        levelPlatforms.OrderBy(x => x.Key);

        while (true)
        {
            yield return StartCoroutine("SpawnPlatforms");
        }
    }

	void Update()
	{
		//EnsureTimedDestroyZones ();

		if ( useTimedDestroy)
		{
			_timedDestroyTimer -= Time.deltaTime;
			if (_timedDestroyTimer <= 0)
			{
				TimedDestroyer();
				_timedDestroyTimer = timedDestroySpeed;
			}
		}
	}

	void OnGUI()
	{
		if (useTimedDestroy && !_timedDestroyAlert.enabled)
		{
			_timedDestroyAlert.enabled = true;
		}
		else if (!useTimedDestroy && _timedDestroyAlert.enabled) 
		{
			_timedDestroyAlert.enabled = false;
		}
	}

	public bool InTimedDestroyZone
	{
		get {
			// TODO: don't reference Music Controller here
			if (_currentPlatform > MusicController.Instance.forestMusicSlowLimit 
			    && _currentPlatform < MusicController.Instance.forestMusicFastLimit)
			{
				return true;
			}
			else if (_currentPlatform > MusicController.Instance.cloudMusicSlowLimit 
			         && _currentPlatform < MusicController.Instance.cloudMusicFastLimit)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public int GetCurrentPlatformNumber()
	{
		return _currentPlatform;
	}

    // Spawn platforms based on player location
    IEnumerator SpawnPlatforms()
    {
        //_timer -= Time.deltaTime;
        var highestPlatformIndex = (levelPlatforms.Keys.Count > 0) ? levelPlatforms.Keys.Max() : 0;
        //Debug.Log("Highest platform is: " + highestPlatformIndex);

        if (_currentPlatform + platformSpawnBuffer > highestPlatformIndex 
		    // spawn platforms if timed destroys has caught up to platformSpawnBuffer
		    || levelPlatforms.Keys.Min() + platformSpawnBuffer > highestPlatformIndex) 
        {
            //GameObject highestPlatformObj;
            GameObject highestPlatform = null;
            float yAxisMultiplier;
            levelPlatforms.OrderBy(p => p.Key);

            int nextPlatform = highestPlatformIndex + 1;
            if (levelPlatforms.TryGetValue(highestPlatformIndex, out highestPlatform))
            {
                yAxisMultiplier = highestPlatform.transform.position.y + highestPlatform.transform.localScale.y +
                                  platformSpacing;
            }
            else
            {
                yAxisMultiplier = startingYAxisValue;
            }

            var toRange = nextPlatform + platformSpawnBuffer;
            if (nextPlatform <= toRange)
            {

                GameObject newPlatform;
                //if (!(timer <= 0)) return;

                if (!levelPlatforms.TryGetValue(nextPlatform, out newPlatform))
                {
                    //TODO: Fix with convention
                    if (nextPlatform == TutorialHoldPlatform) // insert hold tutorial
                    {
                        newPlatform =
                            (GameObject) Instantiate(Resources.Load<GameObject>("Prefabs/Platforms/Tutorial_Hold"),
                                new Vector3(0, yAxisMultiplier, trunkZAxis), Quaternion.identity);

                        //Debug.Log("Loaded hold tutorial platform");
                    }
                    else
                    {
                        newPlatform =
                       (GameObject)Instantiate(platformTypes[_platformBuilder.GetPlatformPrefabByNumber(nextPlatform)],
                           new Vector3(0, yAxisMultiplier, trunkZAxis), Quaternion.identity);
                    }        

                    //yAxisMultiplier += newPlatform.transform.localScale.y + platformSpacing;

                    newPlatform.name = string.Format("Platform_{0}", nextPlatform);
                    newPlatform.transform.localRotation = Quaternion.AngleAxis(GetRandomRotation(nextPlatform), Vector3.up);
                    AdjustProperties(newPlatform, nextPlatform);
                    levelPlatforms.Add(nextPlatform, newPlatform);

                    On_NewPlatform(newPlatform.transform.position.y, levelPlatforms[nextPlatform - 2].transform.position.y);
                    //_timer = platformSpawnInterval;
                }
            }
        }

        //Debug.Log("Platform creation finished. Waiting " + platformSpawnInterval + "s");
        yield return new WaitForSeconds(platformSpawnInterval);
    }

    private void AdjustProperties(GameObject newPlatform, int index)
    {
        UpAndDown upAndDownComponent = newPlatform.transform.GetComponent<UpAndDown>();
        Drop dropComponent = newPlatform.transform.GetComponent<Drop>();
        Orbit orbitComponent = newPlatform.transform.GetComponent<Orbit>();
		Vector3 orbitAxis = Vector3.zero;

        // set platform number agnostic values
        if (orbitComponent != null)
        {
            var rnd = new System.Random();
            orbitAxis = _orbitAxis[rnd.Next(_orbitAxis.Length)];
        }

        if (index > 30)
        {
            if (dropComponent != null) //TODO: change to Editor value
            {
                dropComponent.enabled = true;
            }
        }

        if (index > 80)
        {
            if (orbitComponent != null) //TODO: change to Editor value
            {
                orbitComponent.rotationSpeed = Random.Range(10f, 20f) * orbitAxis.y;
                //orbitComponent.stopTime = Random.Range(0.5f, 1f);
            }
        }

        if (index > 130)
        {
            if (upAndDownComponent != null) //TODO: change to Editor value
            {
                upAndDownComponent.speed = Random.Range(2f, 4f);
                upAndDownComponent.cameraSpeed = upAndDownComponent.speed/2;
                upAndDownComponent.waitTime = 0.2f;
            }

            if (orbitComponent != null) //TODO: change to Editor value
            {
                orbitComponent.rotationSpeed = Random.Range(30f, 40f) * orbitAxis.y;
                orbitComponent.stopTime = 0.5f;
            }
        }
    }

    private float GetRandomRotation(int index)
    {
        if (index == TutorialHoldPlatform)
        {
            return 60f;
        }

        if (index > 60)
        {
            maxRotationLeft = 90f;
            maxRotationRight = 270f;
        }

        if (index > 100)
        {
            maxRotationLeft = 180f;
            maxRotationRight = 180f;
        }

        var options = new Dictionary<int, float> { { 1, maxRotationLeft }, { 2, maxRotationRight } };
        var leftOrRight = Random.Range(1, 3);
        return Random.Range(leftOrRight == 1 ? 0 : 360f, options[leftOrRight]);
    }

    void HandlePlatformReached(Transform platform, Transform player)
    {
        //Debug.Log (platform.parent.name);
        if (platform.parent == null) return;

        _currentPlatform = int.Parse(platform.parent.name.Split('_')[1]);

        //Debug.Log ("Current platform: " + _currentPlatform);

        int newcheckpoint = (_currentPlatform - checkpointBuffer);

        On_ReachedCheckpoint(useTimedDestroy ? _currentPlatform : newcheckpoint);
    }

    void HandleDestroyLowerPlatforms(int platformIndex)
    {
		//DisableChildPlatformsUnderCheckpoint(childPlatformToDeleteIndex);

		if (useTimedDestroy) return;
		//Debug.Log ("Destroy platforms under: " + platformIndex);

        var buffer = (from platform in levelPlatforms where platform.Key <= platformIndex select platform.Key).ToList();

        foreach (var item in buffer)
        {
			var platformToDestroy = levelPlatforms[item];
			levelPlatforms.Remove(item);
			Destroy(platformToDestroy.gameObject);
            //Debug.Log("Removed Platform_" + item);
        }
    }

    /*private void DisableChildPlatformsUnderCheckpoint(int platformIndex)
    {
        GameObject parent;
        if (!this.levelPlatforms.TryGetValue(platformIndex, out parent)) return;
        if (parent == null) return;
        var child = parent.transform.Find("Cube");
        if (child == null) return;
        //Destroy(child.gameObject);
		child.collider.enabled = false;
    }*/

	void HandleMovePlayerToGamePosition(Vector3 newPosition)
	{
		platformSpawnBuffer = 8;
	}

	void TimedDestroyer()
	{
		if (GameController.Instance.gameState == LegendPeak.GameState.Over) return;

		var bottom = levelPlatforms.Keys.Min();
		var buffer = levelPlatforms [bottom];
		//var platformToDestroy = levelPlatforms[buffer];
		levelPlatforms.Remove(bottom);
		//Destroy(platformToDestroy.gameObject);

		if (OnTimedDestroy != null)
		{
			OnTimedDestroy(buffer);
		}

		//Debug.Log ("Timed destroy speed: " + timedDestroySpeed + "s");
		if (bottom == _currentPlatform + checkpointBuffer + 1) // TODO editable
		{
			Debug.Log ("Next death should be game over.");
			if (OnTimedDestroyGameOver != null)
			{
				OnTimedDestroyGameOver();
			}
			//useTimedDestroy = false;
		}
	}

	void HandleOnGameOver()
	{
		//useTimedDestroy = false;
	}

	void HandleOnPlayerReward()
	{
		if (InTimedDestroyZone && !useTimedDestroy)
		{
			useTimedDestroy = true;
			timedDestroySpeed = MusicController.Instance.forestMusicFastDestroySpeed;// TODO clean this shit up
		}
	}
}
