using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{

	private Transform _player;
	private GameObject _telekensisControl;
    private float _deathTimer;

    //private float _deathInterval = 3.0f;

	public float playerZPosition = -2.8f;
	public int lives = 3;
	public float highestPoint = 0.0f;
    public float timeBetweenDeaths = 3.0f;
    public Vector3 playerSpawnPosition;
    public bool playerIsDead;
	public bool movedFromSpawnPosition;
	public bool initiatingRestart;

    public static GameController Instance { get; private set;}

	//public delegate void PlayerIsDead();
	//public static event PlayerIsDead On_PlayerIsDead;

	// Subscribe to events
	void OnEnable()
	{
		KillBox.On_PlayerDeath += HandleOn_PlayerDeath;
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
		KillBox.On_PlayerDeath -= HandleOn_PlayerDeath;
	}

	void Awake ()
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
	}

	// Use this for initialization
	void Start () 
	{
		_player = GameObject.Find ("Player").transform;
		_telekensisControl = GameObject.Find("RotationControl");
	}

	void OnGUI()
	{

		var guiStyleHeightMeter =  new GUIStyle(){ alignment = TextAnchor.UpperRight, fontSize = 24};
		guiStyleHeightMeter.normal.textColor = Color.white;

		var guiStyleLivesMeter =  new GUIStyle(){ alignment = TextAnchor.UpperLeft, fontSize = 24};
		guiStyleLivesMeter.normal.textColor = Color.white;

	    var guiStyleDeathTitle = new GUIStyle() {alignment = TextAnchor.MiddleCenter, fontSize = 40, fontStyle = FontStyle.Bold};
	    guiStyleDeathTitle.normal.textColor = Color.red;

        var guiStyleDeathTimer = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 24 };
        guiStyleDeathTimer.normal.textColor = Color.white;

		GUI.Label(new Rect(Screen.width - 110, 10, 100, 25), highestPoint + "m", guiStyleHeightMeter);
		GUI.Label(new Rect(10, 10, 150, 25), "Lives x " + lives, guiStyleLivesMeter);

	    if (playerIsDead && lives >= 0)
	    {
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "You died!", guiStyleDeathTitle);
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 15, 200, 50), "Next life in: " + Mathf.Round(_deathTimer) + "s", guiStyleDeathTimer);
	    }

        if (playerIsDead && lives <= -1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "You died!", guiStyleDeathTitle);
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 15, 200, 50), "New game starts in: " + Mathf.Round(_deathTimer) + "s", guiStyleDeathTimer);
        }


	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_player.position.y > highestPoint)
		{
			highestPoint = Mathf.Round(_player.position.y);
		}

	    // time delay between player deaths
	    if (playerIsDead)
	    {
			_telekensisControl.SetActive(false);
			if (lives <= -1) 
			{
				initiatingRestart = true;
			}

			if (!initiatingRestart)
			{
				_player.transform.position = (movedFromSpawnPosition) ? _player.transform.position : playerSpawnPosition;
			}

            _deathTimer -= Time.deltaTime;
	        if (!(_deathTimer <= 0)) return;
	        if (lives <= -1)
	        {
				Restart();
	        }
	        else
	        {
	            _deathTimer = timeBetweenDeaths;
	            playerIsDead = false; 
				initiatingRestart = false;
	        }
	    }
	    else
	    {
			_telekensisControl.SetActive(true);
			_deathTimer = timeBetweenDeaths;
			movedFromSpawnPosition = false;
			initiatingRestart = false;
	    }
	}

    static void Restart()
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	void HandleOn_PlayerDeath (float spawnYPosition)
	{
		lives--;
		//Debug.Log ("Lives: " + lives);
	    playerIsDead = true;
	    playerSpawnPosition = new Vector3(0, spawnYPosition, playerZPosition);
	    //_player.transform.position = new Vector3 (0, spawnYPosition, _playerZPosition);
	}
}
