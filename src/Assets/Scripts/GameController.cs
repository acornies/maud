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
    private GameObject _restartButton;
    private EnergyBar _powerBar;
    private EnergyBarRenderer _powerBarRenderer;
    private Texture _deathTexture;

    public bool isPaused;
    public float resumeTime = 1;
    public float playerZPosition = -2.8f;
    //public int lives = 3;
    public float highestPoint = 0.0f;
    public float powerMeter = 0;
    public float maxPower = 20;
    public float timeBetweenDeaths = 3.0f;
    public Vector3 playerSpawnPosition;
    public bool playerIsDead;
    public bool movedFromSpawnPosition;
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
        _deathTexture = Resources.Load<Texture>("Textures/GUI/Death");
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _restartButton = GameObject.Find("RestartButton");
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

        GUI.Label(new Rect(Screen.width - 110, 10, 100, 25), highestPoint + "m", guiStyleHeightMeter);
        //GUI.Label(new Rect(Screen.width - 110, Screen.height - 35, 100, 25), "Power: " + powerMeter, guiStyleHeightMeter);
        //GUI.Label(new Rect(10, 10, 150, 25), "Lives x " + lives, guiStyleLivesMeter);

        if (playerIsDead && powerMeter >= lifeCost)
        {
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "You died!", guiStyleDeathTitle);
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 15, 200, 50), "Next life in: " + Mathf.Round(_deathTimer) + "s", guiStyleDeathTimer);
        }

        if (powerMeter < lifeCost)
        {
            GUI.DrawTexture(
                new Rect(_powerBarRenderer.screenPosition.x, (_powerBarRenderer.screenPosition.y + _powerBarRenderer.SizePixels.y + deathIconOffset),
                deathIconWidth, deathIconWidth),
                _deathTexture, ScaleMode.ScaleToFit);

            if (playerIsDead)
            {
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "You died!", guiStyleDeathTitle);
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 15, 200, 50), "New game starts in: " + Mathf.Round(_deathTimer) + "s", guiStyleDeathTimer);
            }
        }

        if (isPaused)
        {
            //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Resources.Load<Texture>("Textures/PauseOverlay"), ScaleMode.StretchToFill);
            //GUI.ModalWindow(1, )
            /*var windowWidth = Screen.width/2;
            var windowHeight = Screen.height/2;
            GUI.ModalWindow(1, new Rect( (windowWidth - windowWidth/2), (windowHeight - windowHeight/2), windowWidth, windowHeight), PauseGUI, "Paused");
             * */
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
        if (playerIsDead)
        {
            _telekinesisControl.SetActive(false);
            if (powerMeter < lifeCost)
            {
                initiatingRestart = true;
            }

            if (!initiatingRestart)
            {
                _player.transform.position = (movedFromSpawnPosition) ? _player.transform.position : Vector3.Lerp(_player.transform.position, playerSpawnPosition, resurrectionSpeed * Time.deltaTime);
            }

            _deathTimer -= Time.deltaTime;
            if (!(_deathTimer <= 0)) return;
            if (powerMeter < lifeCost)
            {
                Restart();
            }
            else
            {
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
        }
        else
        {
            _deathTimer = timeBetweenDeaths;
            movedFromSpawnPosition = false;
            initiatingRestart = false;
        }
    }

    static void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void HandleOnPlayerDeath()
    {
        playerIsDead = true;

        var screenCenterToWorld =
            Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f));

        playerSpawnPosition = new Vector3(screenCenterToWorld.x, screenCenterToWorld.y, playerZPosition);
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
}
