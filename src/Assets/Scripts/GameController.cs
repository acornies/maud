using System;
using Assets.Scripts.GameState;
using EnergyBarToolkit;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LegendPeak;

public class GameController : MonoBehaviour
{
    private Transform _player;
    private GameObject _telekinesisControl;
    private float _deathTimer;
    private int _previousPlatformNumber;
    private float _resumeTimer;
    private bool _initiatingResume;
    private GameObject _menuButton;
    private GameObject _restartButton;
    private GameObject _playButton;
    private GameObject _recordButton;
    private EnergyBar _powerBar;
    private Text _heightCounter;
    private CameraMovement _cameraMovement;

    public GameObject mainCamera;
    public GameState gameState;
    public GameMode gameMode;
    public bool inSafeZone = true;
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
    public EnergyBarRenderer powerBarRenderer;

    public static GameController Instance { get; private set; }

    public delegate void GameStart();
    public static event GameStart OnGameStart;

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

    public delegate void MaxHeightIncrease(float amount);
    public static event MaxHeightIncrease OnMaxHeightIncrease;

    // Subscribe to events
    void OnEnable()
    {
        KillBox.On_PlayerDeath += HandleOnPlayerDeath;
        BoundaryController.On_PlayerDeath += HandleOnPlayerDeath;
        TelekinesisController.On_PlayerPowerDeplete += HandleOnPlayerPowerDeplete;
        OnGameStart += HandleOnGameStart;
        OnGamePause += HandleOnGamePause;
        OnGameResume += HandleOnGameResume;
        OnGameOver += HandleOnGameOver;
        PowerUpBehaviour.OnPowerPickUp += HandleOnPowerPickUp;
        IntroLedge.OnShowMenuButtons += HandleOnShowMenuButtons;
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
        OnGameStart -= HandleOnGameStart;
        OnGamePause -= HandleOnGamePause;
        OnGameResume -= HandleOnGameResume;
        OnGameOver -= HandleOnGameOver;
        PowerUpBehaviour.OnPowerPickUp -= HandleOnPowerPickUp;
        IntroLedge.OnShowMenuButtons -= HandleOnShowMenuButtons;
    }

    // Use this for initialization
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

	void Start()
	{
        _player = GameObject.Find("Player").transform;
        _telekinesisControl = GameObject.Find("TelekinesisControl");
        _powerBar = GetComponentInChildren<EnergyBar>();
        powerBarRenderer = GetComponentInChildren<EnergyBarRenderer>();

        //_mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _menuButton = GameObject.Find("MenuButton");
        _restartButton = GameObject.Find("RestartButton");
        _heightCounter = GameObject.Find("HeightCounter").GetComponent<Text>();
        _playButton = GameObject.Find("PlayButton");
        _recordButton = GameObject.Find("RecordButton");

        if (mainCamera == null)
        {
            Debug.LogError("Please set a main camera GameObject to GameController.cs");
        }
        else
        {
            _cameraMovement = mainCamera.GetComponent<CameraMovement>();
        }

        gameState = GameState.Started;
        gameMode = GameMode.Story; // TODO: change from menu

        switch (gameMode)
        {
            case GameMode.Story:
                //_heightCounter.enabled = false;
                powerBarRenderer.texturesBackground[0].color.a = 0f;
                //powerBarRenderer.textureBar..a = 0f;
                break;
            default:
                break;
        }
    }

    void OnGUI()
    {
        if (_heightCounter.enabled)
        {
            _heightCounter.text = highestPoint + "m"; // TODO: localize
        }

        if (!inSafeZone && powerBarRenderer.texturesBackground[0].color.a == 0f)
        {
            powerBarRenderer.texturesBackground[0].color.a = 1f;
            //powerBarRenderer.textureBarColor.a = 0f;
            powerBarRenderer.screenPosition = new Vector2(20f, 20f);
        }

        //if (!powerBarRenderer.enabled) return;

        if (powerMeter < lifeCost)
        {
            powerBarRenderer.texturesForeground[0].color.a = 1f;
        }
        else if (powerMeter >= lifeCost)
        {
            powerBarRenderer.texturesForeground[0].color.a = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = (gameState == GameState.Paused && !_initiatingResume) ? 0 : 1;

        if (gameState == GameState.Paused && _initiatingResume)
        {
            _resumeTimer -= Time.deltaTime;
            if (_resumeTimer <= 0)
            {
                gameState = GameState.Running;
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
            var roundedPosition = Mathf.Round(_player.position.y);
            highestPoint = roundedPosition;
            if (OnMaxHeightIncrease != null)
            {
                OnMaxHeightIncrease(roundedPosition * SkyboxCameraMovement.speedMultiplier);
            }
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
                if (!inSafeZone)
                {
                    powerMeter -= lifeCost;
                }
            }
        }

        else if (playerIsDead && powerMeter < lifeCost)
        {
            initiatingRestart = true;
            _menuButton.GetComponent<Button>().interactable = false;
            _restartButton.GetComponent<Button>().interactable = true;
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
        _menuButton.GetComponent<Button>().interactable = false;
        //button.enable = false; 
        if (OnGameRestart != null)
        {
            OnGameRestart(Application.loadedLevel);
        }
    }

    public void UpdateSafeZone(bool boolValue)
    {
        inSafeZone = boolValue;
    }

    void HandleOnPlayerDeath()
    {
        playerIsDead = true;

        var screenCenterToWorld =
            Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f));

        _player.GetComponent<PlayerMovement>().SetSpawnPosition(new Vector3(screenCenterToWorld.x, screenCenterToWorld.y, playerZPosition));
    }

    void HandleOnPlayerPowerDeplete(float amount)
    {
        powerMeter -= amount;
    }

    public void ButtonStart()
    {
        //Debug.Log("Pressed play button");
        if (OnGameStart != null)
        {
            OnGameStart();
        }
        _playButton.GetComponent<Image>().enabled = false;
    }

    public void ButtonMenu()
    {
        if (gameState == GameState.Started)
        {
            // TODO: show settings
            Debug.Log("Toggle settings.");
        }
        else if (gameState != GameState.Paused)
        {
            if (OnGamePause != null)
            {
                OnGamePause();
            }
            Debug.Log("Toggle settings.");
        }
        else
        {
            _initiatingResume = true;
        }
    }

    public void ButtonRestart()
    {
        Restart();
        _initiatingResume = true;
    }

    void HandleOnShowMenuButtons()
    {
        Debug.Log("Show play button");
        _playButton.GetComponent<Image>().enabled = true;
        _menuButton.GetComponent<Image>().enabled = true;
        _recordButton.GetComponent<Image>().enabled = true;
    }

    void HandleOnGameStart()
    {
        gameState = GameState.Running;
        _cameraMovement.CameraTarget = _player.FindChild("CharacterTarget");
    }

    void HandleOnGamePause()
    {
        gameState = GameState.Paused;
        _player.GetComponent<PlayerMovement>().disabled = true;
        _telekinesisControl.SetActive(false);
        //_mainCamera.GetComponent<BlurEffect>().enabled = true;
        _restartButton.GetComponent<Image>().enabled = true;
        _restartButton.GetComponent<Button>().interactable = true;

    }

    void HandleOnGameResume()
    {
        _player.GetComponent<PlayerMovement>().disabled = false;
        _telekinesisControl.SetActive(true);
        //_mainCamera.GetComponent<BlurEffect>().enabled = false;
        _restartButton.GetComponent<Image>().enabled = false;
        _restartButton.GetComponent<Button>().interactable = false;
    }

    void HandleOnGameOver()
    {
        gameState = GameState.Over;
        _player.GetComponent<PlayerMovement>().disabled = true;
        _telekinesisControl.SetActive(false);
        _restartButton.GetComponent<Image>().enabled = true;
        _restartButton.GetComponent<Button>().interactable = true;
    }

    private void HandleOnPowerPickUp(float powerToAdd)
    {
        //Debug.Log("Add " + powerToAdd + " to power meter.");
        powerMeter += powerToAdd;
    }
}
