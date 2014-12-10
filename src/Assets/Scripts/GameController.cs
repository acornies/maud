using EnergyBarToolkit;
using UnityEngine;
using UnityEngine.UI;
using LegendPeak;

public class GameController : MonoBehaviour
{
    private Transform _player;
    private GameObject _telekinesisControl;
    private float _deathTimer;
    private int _previousPlatformNumber;
    private float _resumeTimer;
    //private bool _initiatingResume;
    private GameObject _menuButton;
    private GameObject _restartButton;
    private GameObject _playButton;
    private GameObject _recordButton;
    private GameObject _musicButton;
    private GameObject _cartButton;
    private EnergyBar _powerBar;
    private Text _heightCounter;
    private CameraMovement _cameraMovement;
    private bool _isMenuOpen;

    public GameObject mainCamera;
    public bool playMusic = true;
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
    public Sprite soundOnImage;
    public Sprite soundOffImage;

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

    public delegate void ToggleMusic(bool playMusic);
    public static event ToggleMusic OnToggleMusic;

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
        OnGameRestart += HandleOnGameRestart;
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
        OnGameRestart -= HandleOnGameRestart;
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
        _musicButton = GameObject.Find("MusicButton");
        _cartButton = GameObject.Find("CartButton");

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

    void Start()
    {

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
        Time.timeScale = (gameState == GameState.Paused) ? 0 : 1;

        /*if (gameState == GameState.Paused && _initiatingResume)
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
        }*/

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
        //_restartButton.GetComponent<Image>().enabled = false;
        _restartButton.GetComponent<Button>().interactable = false;
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
        switch (gameState)
        {
            case GameState.Started:
                if (OnGameStart != null)
                {
                    OnGameStart();
                }
                break;
            case GameState.Paused:
                if (OnGameResume != null)
                {
                    OnGameResume();
                }
                gameState = GameState.Running;
                break;
        }

    }

    public void ButtonMenu()
    {
        if (gameState == GameState.Started)
        {
            _isMenuOpen = !_isMenuOpen;

        }
        else if (gameState != GameState.Paused)
        {
            if (OnGamePause != null)
            {
                OnGamePause();
            }
        }
        else
        {
            if (OnGameResume != null)
            {
                OnGameResume();
            }
            gameState = GameState.Running;
        }

        var menuAnimator = _menuButton.GetComponent<Animator>();
        if (_isMenuOpen && !menuAnimator.enabled)
        {
            menuAnimator.enabled = true;
        }
        else
        {
            menuAnimator.enabled = false;
        }

        if (_isMenuOpen && gameState == GameState.Started)
        {
            _musicButton.GetComponent<Image>().enabled = true;
            _cartButton.GetComponent<Image>().enabled = true;
            // TODO: FUCKING DISABLE HAND BUTTON FOR NOW
            _cartButton.GetComponent<Button>().interactable = true; // TODO enable when ready
            _cartButton.GetComponent<Button>().interactable = false; // TODO enable when ready
        }
        else if (!_isMenuOpen && gameState == GameState.Started)
        {
            _musicButton.GetComponent<Image>().enabled = false;
            _cartButton.GetComponent<Image>().enabled = false;
        }
    }

    public void ButtonRestart()
    {
        Restart();
        //gameState = GameState.Running;
        _player.GetComponent<PlayerMovement>().disabled = false; // TODO move to playerMovement

    }

    public void ButtonMusic()
    {
        playMusic = !playMusic;

        if (OnToggleMusic != null)
        {
            OnToggleMusic(playMusic);
        }

        _musicButton.GetComponent<Image>().sprite = !playMusic ? soundOffImage : soundOnImage;
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
        _musicButton.GetComponent<Image>().enabled = false;
        _cartButton.GetComponent<Image>().enabled = false;
        _isMenuOpen = false;
        _menuButton.GetComponent<Animator>().enabled = false;

        _playButton.GetComponent<Image>().enabled = false;
        ((RectTransform)_playButton.transform).anchorMin = new Vector2(.5f, .5f);
        ((RectTransform)_playButton.transform).anchorMax = new Vector2(.5f, .5f);
        ((RectTransform)_playButton.transform).anchoredPosition = new Vector3(0, 0, 0);


        //ButtonMenu();
    }

    void HandleOnGamePause()
    {
        gameState = GameState.Paused;
        _player.GetComponent<PlayerMovement>().disabled = true;
        _telekinesisControl.SetActive(false);
        //_mainCamera.GetComponent<BlurEffect>().enabled = true;
        _restartButton.GetComponent<Image>().enabled = true;
        _restartButton.GetComponent<Button>().interactable = true;
        _playButton.GetComponent<Image>().enabled = true;
        _playButton.GetComponent<Button>().interactable = true;
        _musicButton.GetComponent<Image>().enabled = true;
        _cartButton.GetComponent<Image>().enabled = true;
        // TODO: FUCKING DISABLE HAND BUTTON FOR NOW
        _cartButton.GetComponent<Button>().interactable = true; // TODO enable when ready
        _cartButton.GetComponent<Button>().interactable = false; // TODO enable when ready
        _isMenuOpen = true;
    }

    void HandleOnGameResume()
    {
        gameState = GameState.Running;
        _player.GetComponent<PlayerMovement>().disabled = false;
        _telekinesisControl.SetActive(true);
        //_mainCamera.GetComponent<BlurEffect>().enabled = false;
        _restartButton.GetComponent<Image>().enabled = false;
        _restartButton.GetComponent<Button>().interactable = false;
        _playButton.GetComponent<Image>().enabled = false;
        _playButton.GetComponent<Button>().interactable = false;
        _menuButton.GetComponent<Animator>().enabled = false;
        _musicButton.GetComponent<Image>().enabled = false;
        _cartButton.GetComponent<Image>().enabled = false;
        _isMenuOpen = false;
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

    private void HandleOnGameRestart(int sceneindex)
    {
        gameState = GameState.Running;
        _playButton.GetComponent<Image>().enabled = false;
        _playButton.GetComponent<Button>().interactable = false;
        _musicButton.GetComponent<Image>().enabled = false;
        _cartButton.GetComponent<Image>().enabled = false;
        _menuButton.GetComponent<Animator>().enabled = false;
    }
}
