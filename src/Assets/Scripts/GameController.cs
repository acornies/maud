using System;
using EnergyBarToolkit;
using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    private Transform _player;
    private GameObject _telekinesisControl;
    private float _deathTimer;
    private int _previousPlatformNumber;
    private float _resumeTimer;
    private bool _initiatingResume;
    private Camera _mainCamera;
    private GameObject _menuButton;
    private GameObject _restartButton;
    private EnergyBar _powerBar;
    private EnergyBarRenderer _powerBarRenderer;
    private GUIText _heightCounter;

    public bool isPaused;
    public float resumeTime = 1;
    public float playerZPosition = -2.8f;
    public float highestPoint = 0.0f;
    public float powerMeter = 0;
    public float maxPower = 20;
    public float timeBetweenDeaths = 3.0f;
    public Vector3 playerSpawnPosition;
    public bool playerIsDead;
    public bool initiatingRestart;
    public bool useAcceleration;
    public float lifeCost = 5f;
    public float powerAccumulationRate = 0.25f;
    public float resurrectionSpeed = 15f;
    public float deathIconWidth = 20f;
    public float deathIconOffset = 10f;

    public static GameController Instance { get; private set; }

    public delegate void GamePause();
    public static event GamePause OnGamePause;

    public delegate void GameResume();
    public static event GameResume OnGameResume;

    public delegate void GameRestart(int sceneIndex);
    public static event GameRestart OnGameRestart;

    public delegate void GameOver();
    public static event GameOver OnGameOver;

    public delegate void PlayerResurrection();
    public static event PlayerResurrection OnPlayerResurrection;

    // Subscribe to events
    void OnEnable()
    {
        KillBox.On_PlayerDeath += HandleOnPlayerDeath;
        BoundaryController.On_PlayerDeath += HandleOnPlayerDeath;
        TelekinesisController.On_PlayerPowerDeplete += HandleOnPlayerPowerDeplete;
        EasyButton.On_ButtonDown += HandleOnButtonDown;
        OnGamePause += HandleOnGamePause;
        OnGameResume += HandleOnGameResume;
        OnGameOver += HandleOnGameOver;
        PowerUpBehaviour.OnPowerPickUp += HandleOnPowerPickUp;
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
        KillBox.On_PlayerDeath -= HandleOnPlayerDeath;
        BoundaryController.On_PlayerDeath -= HandleOnPlayerDeath;
        TelekinesisController.On_PlayerPowerDeplete -= HandleOnPlayerPowerDeplete;
        EasyButton.On_ButtonDown -= HandleOnButtonDown;
        OnGamePause -= HandleOnGamePause;
        OnGameResume -= HandleOnGameResume;
        OnGameOver -= HandleOnGameOver;
        PowerUpBehaviour.OnPowerPickUp -= HandleOnPowerPickUp;
    }

    void Awake()
    {
        Application.targetFrameRate = 60;

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
    void Start()
    {
        _player = GameObject.Find("Player").transform;
        _telekinesisControl = GameObject.Find("TelekinesisControl");
        _powerBar = GetComponentInChildren<EnergyBar>();
        _powerBarRenderer = GetComponentInChildren<EnergyBarRenderer>();

        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _menuButton = GameObject.Find("MenuButton");
        _restartButton = GameObject.Find("RestartButton");
        _heightCounter = GameObject.Find("HeightCounter").GetComponent<GUIText>();

        if (_restartButton != null)
        {
            _restartButton.GetComponent<EasyButton>().enable = false;
        }
    }

    void OnGUI()
    {

        var guiStyleHeightMeter = new GUIStyle() { alignment = TextAnchor.MiddleRight, fontSize = 24 };
        guiStyleHeightMeter.normal.textColor = Color.white;

        var guiStyleLivesMeter = new GUIStyle() { alignment = TextAnchor.MiddleLeft, fontSize = 24 };
        guiStyleLivesMeter.normal.textColor = Color.white;

        var guiStyleDeathTitle = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 40, fontStyle = FontStyle.Bold };
        guiStyleDeathTitle.normal.textColor = Color.red;

        var guiStyleDeathTimer = new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontSize = 24 };
        guiStyleDeathTimer.normal.textColor = Color.white;

        _heightCounter.text = highestPoint + "m";

        if (playerIsDead && powerMeter >= lifeCost)
        {
            //GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "You died!", guiStyleDeathTitle);
            //GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 15, 200, 50), "Next life in: " + Mathf.Round(_deathTimer) + "s", guiStyleDeathTimer);
        }

        if (powerMeter < lifeCost)
        {
            _powerBarRenderer.texturesForeground[0].color.a = 1f;
        }
        else if (powerMeter >= lifeCost)
        {
            _powerBarRenderer.texturesForeground[0].color.a = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = (isPaused && !_initiatingResume) ? 0 : 1;

        if (isPaused && _initiatingResume)
        {
            _resumeTimer -= Time.deltaTime;
            if (_resumeTimer <= 0)
            {
                isPaused = false;
                _initiatingResume = false;
                if (OnGameResume != null)
                {
                    OnGameResume();
                }
            }
        }
        else
        {
            _resumeTimer = resumeTime;
        }

        if (_player.position.y > highestPoint)
        {
            highestPoint = Mathf.Round(_player.position.y);
        }

        if (PlatformController.Instance.GetCurrentPlatformNumber() > _previousPlatformNumber)
        {
            _previousPlatformNumber = PlatformController.Instance.GetCurrentPlatformNumber();
            powerMeter += powerAccumulationRate;
        }

        powerMeter = Mathf.Clamp(powerMeter, 0, maxPower);

        _powerBar.valueCurrent = Mathf.CeilToInt(powerMeter);
        _powerBar.SetValueMin(0);
        _powerBar.SetValueMax(Mathf.CeilToInt(maxPower));

        // time delay between player deaths
        if (playerIsDead && powerMeter >= lifeCost)
        {
            _telekinesisControl.SetActive(false);

            _deathTimer -= Time.deltaTime;
            if (!(_deathTimer <= 0)) return;

            _telekinesisControl.SetActive(true);
            _deathTimer = timeBetweenDeaths;
            playerIsDead = false;
            initiatingRestart = false;
            if (OnPlayerResurrection != null)
            {
                OnPlayerResurrection();
                powerMeter -= lifeCost;
            }
        }

        else if (playerIsDead && powerMeter < lifeCost)
        {
            initiatingRestart = true;
            _menuButton.GetComponent<EasyButton>().isActivated = false;
            _restartButton.GetComponent<EasyButton>().enable = true;
            if (OnGameOver != null)
            {
                OnGameOver();
            }
        }
        else
        {
            _deathTimer = timeBetweenDeaths;
            //movedFromSpawnPosition = false;
            initiatingRestart = false;
        }
    }

    void Restart()
    {
        //Debug.Log("Calling GameController.Restart");
        _menuButton.GetComponent<EasyButton>().isActivated = false;
        //button.enable = false; 
        if (OnGameRestart != null)
        {
            OnGameRestart(Application.loadedLevel);
        }
    }

    void HandleOnPlayerDeath()
    {
        playerIsDead = true;

        var screenCenterToWorld =
            Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f));

        //playerSpawnPosition = new Vector3(screenCenterToWorld.x, screenCenterToWorld.y, playerZPosition);
        _player.GetComponent<PlayerMovement>().SetSpawnPosition(new Vector3(screenCenterToWorld.x, screenCenterToWorld.y, playerZPosition));
    }

    void HandleOnPlayerPowerDeplete(float amount)
    {
        powerMeter -= amount;
    }

    private void HandleOnButtonDown(string buttonName)
    {
        if (buttonName.Equals("MenuButton"))
        {
            if (!isPaused)
            {
                isPaused = true;
                if (OnGamePause != null)
                {
                    OnGamePause();

                }
            }
            else
            {
                _initiatingResume = true;
            }
        }

        if (buttonName.Equals("RestartButton"))
        {
            Restart();
            _initiatingResume = true;
        }
    }

    void HandleOnGamePause()
    {
        _player.GetComponent<PlayerMovement>().enabled = false;
        _telekinesisControl.SetActive(false);
        _mainCamera.GetComponent<BlurEffect>().enabled = true;
        _restartButton.GetComponent<EasyButton>().enable = true;

    }

    void HandleOnGameResume()
    {
        _player.GetComponent<PlayerMovement>().enabled = true;
        _telekinesisControl.SetActive(true);
        _mainCamera.GetComponent<BlurEffect>().enabled = false;
        _restartButton.GetComponent<EasyButton>().enable = false;
    }

    void HandleOnGameOver()
    {
        _player.GetComponent<PlayerMovement>().enabled = false;
        _telekinesisControl.SetActive(false);
    }

    private void HandleOnPowerPickUp(float powerToAdd)
    {
        Debug.Log("Add " + powerToAdd + " to power meter.");
        powerMeter += powerToAdd;
    }
}
