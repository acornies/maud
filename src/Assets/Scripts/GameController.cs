using System.Linq;
using System.Collections;
using System.Collections.Generic;
using EnergyBarToolkit;
using UnityEngine;
using UnityEngine.UI;
using LegendPeak;
using LegendPeak.Native;
using UnityEngine.Advertisements;

public class GameController : MonoBehaviour
{
    private Transform _player;
    private GameObject _telekinesisControl;
    private float _deathTimer;
    private int _previousPlatformNumber;
    private float _resumeTimer;
    private bool _initiatingResume;
    private Image _menuButtonImage;
	private Button _menuButtonBehaviour;
    private Image _restartButtonImage;
	private Image _arrowImage;
	private Button _restartButtonBehaviour;
    private Image _playButtonImage;
	private Button _playButtonBehaviour;
	private Image _triangleImage;
    private Image _recordButtonImage;
	private Button _recordButtonBehaviour;
    private Image _musicButtonImage;
	private int _selectedSoundTrack;
	private Image _speakerImage;
	//private Image _controlModeImage; // TODO finish later
	//private Button _controlModeBehaviour;
	private Button _musicButtonBehaviour;
	private Image _shareButtonImage;
	private Image _graphImage;
	private Button _shareButtonBehaviour;
	private Image _leaderboardImage;
	private Button _leaderboardButton;
	private Image _storeButtonImage;
	private bool _isStoreOpen;
	private Button _storeButtonBehaviour;
	private Image _plusImage;
	private Image _restoreButtonImage;
	private Button _restoreButtonBehaviour;
	private Image _cloudDownloadImage;
	private Text _restoreButtonText;
    //private Image _cartButtonImage;
	//private Button _cartButtonBehaviour;
    private EnergyBar _powerBar;
    //private CameraMovement _cameraMovement;
    private bool _isSettingsOpen;
	private Text _highestPointText;
	private Text _totalHeightText;
	private bool _timeDestroyed;
	private Text _levelsInfoText;

	public int targetFrameRate = 60;
	public string advertisingAppIdIOS;
	public string advertisingAppIdAndroid;
	public bool advertisingTestMode;
	public Text heightCounter;
	public bool countHeight; 
    private CameraMovement _cameraMovement;
    public GameState gameState;
    //public GameMode gameMode;
    public bool inSafeZone = true;
    public float resumeTime = 1;
    public float playerZPosition = -2.8f;
    public int highestPoint;
    public float powerMeter = 0;
    //public float maxPower = 20;
    public float timeBetweenDeaths = 3.0f;
    public Vector3 playerSpawnPosition;
    public bool playerIsDead;
    public bool initiatingRestart;
    //public bool useAcceleration;
	//public ControlMode controlMode;
    //public float lifeCost = 5f;
    public float powerAccumulationRate = 0.25f;
    public float resurrectionSpeed = 15f;
    public float deathIconWidth = 20f;
    public float deathIconOffset = 10f;
    public FilledRenderer3D powerBarRenderer;
    //public Sprite soundOnImage;
    //public Sprite soundOffImage;
	public Sprite[] soundStateImages;
    public Sprite controlAccelerometerImage;
    public Sprite controlFingerSwipeImage;
	public Color heightCounterColor;
	public float continueAdPower = 50;
	public Color leaderboardActiveColor;
	
	//public int gameOverContinues;
	public int advertisingContinues = 2;
	public bool promptAdContinue
	{
		get {return (advertisingContinues > 0) ? true : false; }
	}

	public bool promptPurchaseContinue
	{
		get {return (PlayerState.Instance.Data.gameOverContinues == 0) ? true : false; }
	}

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

    public delegate void MaxHeightIncrease(int delta, float rotationMultiplier);
    public static event MaxHeightIncrease OnMaxHeightIncrease;

    public delegate void ToggleMusic(int musicSelection);
    public static event ToggleMusic OnToggleMusic;

	public delegate void PlayerReward();
	public static event PlayerReward OnPlayerReward;

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
		OnPlayerResurrection += HandleOnPlayerResurrection;
        PowerUpBehaviour.OnPowerPickUp += HandleOnPowerPickUp;
        IntroLedge.OnShowMenuButtons += HandleOnShowMenuButtons; 
		PlatformController.OnTimedDestroyGameOver += HandleOnTimedDestroyGameOver;
		PlatformController.On_ReachedCheckpoint += HandleOnReachedCheckpoint;
		MusicController.OnFastMusicStop += HandleOnFastMusicStop;
		CameraMovement.OnMovePlayerToGamePosition += HandleOnMovePlayerToGamePosition;
		//UnityAds.OnVideoCompleted += HandleOnVideoCompleted;
		//OnPlayerReward += HandleOnPlayerResurrectionOnGameOver;
		//StoreController.Instance.Native.OnTransactionComplete += HandleOnTransactionComplete;
		PlayerState.OnSuccessfullNonConsumable += HandleOnSuccessfullNonConsumable;
    }

    void HandleOnMovePlayerToGamePosition (Vector3 playerPosition)
    {
		heightCounter.enabled = true;
    }

    void HandleOnReachedCheckpoint (int platform)
    {
		_timeDestroyed = false;
    }

    void HandleOnPlayerResurrection ()
    {
		//_timeDestroyed = false;
    }

    void HandleOnFastMusicStop ()
    {
		//_timeDestroyed = false;
    }

    void HandleOnTimedDestroyGameOver ()
    {
		_timeDestroyed = true;
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
		OnPlayerResurrection -= HandleOnPlayerResurrection;
        PowerUpBehaviour.OnPowerPickUp -= HandleOnPowerPickUp;
        IntroLedge.OnShowMenuButtons -= HandleOnShowMenuButtons;
		PlatformController.OnTimedDestroyGameOver -= HandleOnTimedDestroyGameOver;
		PlatformController.On_ReachedCheckpoint += HandleOnReachedCheckpoint;
		MusicController.OnFastMusicStop -= HandleOnFastMusicStop;
		CameraMovement.OnMovePlayerToGamePosition -= HandleOnMovePlayerToGamePosition;
		//UnityAds.OnVideoCompleted -= HandleOnVideoCompleted;
		//OnPlayerReward -= HandleOnPlayerResurrectionOnGameOver;
		//StoreController.Instance.Native.OnTransactionComplete -= HandleOnTransactionComplete
		PlayerState.OnSuccessfullNonConsumable -= HandleOnSuccessfullNonConsumable;
    }

    void HandleOnSuccessfullNonConsumable (string productId)
    {
		var productButton = _storeButtonImage.transform.FindChild (productId);
		productButton.gameObject.SetActive (false);
		Debug.Log ("Removed store product button id: " + productId);

		if (productId == StoreController.Instance.Native.purchaseMusicIdentifier)
		{
			if (gameState == GameState.Started)
			{
				if (!_isSettingsOpen)
				{	
					ButtonMenu();
				}

				if (PlayerState.Instance.Data.soundTrackSelection == 0)
				{
					ButtonMusic();
					ButtonMusic();
				}
				else
				{
					ButtonMusic();
				}
			}
		}

		CheckDisableStoreButton ();
    }

	void CheckDisableStoreButton()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) return;

		if (StoreController.Instance.GetPurchasabletems().Count() == 0)
		{
			_isStoreOpen = false;
			_storeButtonBehaviour.interactable = false;
		}
	}

    // Use this for initialization
    void Awake()
    {
		Application.targetFrameRate = targetFrameRate;

		if (Advertisement.isSupported) 
		{
			Advertisement.allowPrecache = true;
			Advertisement.Initialize (Application.platform == RuntimePlatform.Android ? advertisingAppIdAndroid : advertisingAppIdIOS, advertisingTestMode);
		} 
		else 
		{
			Debug.Log("Platform not supported");
		}

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
		var powerBar = GameObject.Find ("PowerBar");
		_powerBar = powerBar.GetComponent<EnergyBar>();
		powerBarRenderer = powerBar.GetComponent<FilledRenderer3D>();
		heightCounter = GameObject.Find("HeightCounter").GetComponent<Text>();
		_cameraMovement = GameObject.Find ("Main Camera").GetComponent<CameraMovement>();

		var menuButton = GameObject.Find ("MenuButton");
		_menuButtonImage = menuButton.GetComponent<Image>();
		_menuButtonBehaviour = menuButton.GetComponent<Button>();

		var restartButton = GameObject.Find ("RestartButton");
        _restartButtonImage = restartButton.GetComponent<Image>();
		_restartButtonBehaviour = restartButton.GetComponent<Button>();
		_arrowImage = restartButton.transform.FindChild ("Arrow").GetComponent<Image>();

		var playButton = GameObject.Find ("PlayButton");
		_playButtonImage = playButton.GetComponent<Image>();
		_playButtonBehaviour = playButton.GetComponent<Button>();
		_triangleImage = playButton.transform.FindChild ("Triangle").GetComponent<Image> ();

		var musicButton = GameObject.Find ("MusicButton");
		_musicButtonImage = musicButton.GetComponent<Image>();
		_musicButtonBehaviour = musicButton.GetComponent<Button>();
		_speakerImage = musicButton.transform.FindChild ("Speaker").GetComponent<Image> ();

		var recordButton = GameObject.Find ("RecordButton");
		_recordButtonImage = recordButton.GetComponent<Image>();
		_recordButtonBehaviour = recordButton.GetComponent<Button>();

		var shareButton = GameObject.Find ("ShareButton");
		_shareButtonBehaviour = shareButton.GetComponent<Button> ();
		_shareButtonImage = shareButton.GetComponent<Image> ();
		_graphImage = shareButton.transform.FindChild ("Graph").GetComponent<Image>();

		//_totalHeightText = GameObject.Find ("TotalText").GetComponent<Text>();
		_highestPointText = GameObject.Find ("HighestText").GetComponent<Text>();

		var leaderboard = GameObject.Find ("LeaderboardButton");
		_leaderboardImage = leaderboard.GetComponent<Image> ();
		_leaderboardButton = leaderboard.GetComponent<Button> ();

		var storeButton = GameObject.Find ("StoreButton");
		_storeButtonImage = storeButton.GetComponent<Image> ();
		_storeButtonBehaviour = storeButton.GetComponent<Button> ();
		_plusImage = storeButton.transform.FindChild ("Plus").GetComponent<Image>();
		var restoreButton = storeButton.transform.FindChild ("RestoreButton");
		_restoreButtonImage = restoreButton.GetComponent<Image> ();
		_restoreButtonBehaviour = restoreButton.GetComponent<Button> ();
		_cloudDownloadImage = restoreButton.transform.FindChild ("Cloud").GetComponent<Image> ();
		_restoreButtonText = restoreButton.transform.FindChild("Text").GetComponent<Text> ();

		var controlMode = GameObject.Find ("ControlButton");
		//_controlModeImage = controlMode.GetComponent<Image>();
		//_controlModeBehaviour = controlMode.GetComponent<Button>();
		_levelsInfoText = GameObject.Find ("LevelInfo").GetComponent<Text>();

        gameState = GameState.Started;
        /*gameMode = GameMode.Story; // TODO: change from menu

        switch (gameMode)
        {
            case GameMode.Story:
                //_heightCounter.enabled = false;
                powerBarRenderer.texturesBackground[0].color.a = 0f;
                //powerBarRenderer.textureBar..a = 0f;
                break;
            default:
                break;
        }*/
    }

    void Start()
    {
		var isiOS = (Application.platform == RuntimePlatform.IPhonePlayer) ? true : false;
		_speakerImage.sprite = soundStateImages[PlayerState.Instance.Data.soundTrackSelection];
		powerMeter = PlayerState.Instance.playerLevel.lifeCost;

		var products = StoreController.Instance.GetPurchasabletems ();
		float yPositionOffset = 0;
		foreach(var product in products)
		{
			GameObject newButton = Instantiate(_restoreButtonImage.gameObject) as GameObject;
			newButton.transform.SetParent(_storeButtonImage.transform);
			newButton.name =  (!isiOS) ? product.identifier.ToLower() : product.identifier; //FUCKING HACKY HACK
			//var behaviour = newButton.GetComponent<Button>();
			var mainImage = newButton.GetComponent<Image>();
			var childSprite = newButton.transform.FindChild("Cloud").GetComponent<Image>();
			childSprite.name = product.description;
			var buttonText = newButton.transform.FindChild("Text").GetComponent<Text>();
			buttonText.text = product.description;
			childSprite.sprite = product.image;
			//behaviour.onClick.RemoveAllListeners();
			yPositionOffset += mainImage.rectTransform.sizeDelta.y;
			mainImage.rectTransform.anchoredPosition = new Vector2(0, -yPositionOffset);
			mainImage.rectTransform.localScale = new Vector2(1f, 1f);
			newButton.GetComponent<Button>().onClick.AddListener(() => { ButtonBuyProduct(product.identifier); });
		}
		
		_restoreButtonBehaviour.onClick.AddListener (ButtonRestorePurchases);

		_selectedSoundTrack = PlayerState.Instance.Data.soundTrackSelection;

		// remove restore purchaes for android
		_restoreButtonImage.enabled = isiOS;
		_restoreButtonBehaviour.interactable = isiOS;
		_cloudDownloadImage.enabled = isiOS;

		CheckDisableStoreButton ();
    }

    void OnGUI()
    {
        //ToggleControlModeGUI();
        
		_highestPointText.text = PlayerState.Instance.Data.highestPlatform.ToString();
		if (heightCounter.enabled)
        {
			heightCounter.text = highestPoint.ToString();
			//_totalHeightText.text = PlayerState.Instance.Data.totalPlatforms.ToString();
        }

		if (StoreController.Instance.Native.authenticated && !_leaderboardButton.interactable)
		{
			_leaderboardButton.interactable = true;
		}

        if (!inSafeZone && powerBarRenderer.textureBarColorType == EnergyBarBase.ColorType.Solid)
        {
            powerBarRenderer.textureBarColor.a = 1f;
			powerBarRenderer.textureBarColorType = EnergyBarBase.ColorType.Gradient;
            //powerBarRenderer.screenPosition = new Vector2(30f, 30f);
        }

        //if (!powerBarRenderer.enabled) return;

		if (powerMeter < PlayerState.Instance.playerLevel.lifeCost)
        {
            powerBarRenderer.texturesForeground[0].color.a = 1f;
        }
		else //if (powerMeter >= PlayerState.Instance.playerLevel.lifeCost)
        {
            powerBarRenderer.texturesForeground[0].color.a = 0f;
        }

		if (StoreController.Instance.Native.authenticated && _leaderboardImage.color != leaderboardActiveColor)
		{
			_leaderboardImage.color = Color.Lerp(_leaderboardImage.color, leaderboardActiveColor, 5f * Time.unscaledDeltaTime);
		}
    }

    /*private void ToggleControlModeGUI()
    {
        if (PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer && _controlModeImage.sprite != controlAccelerometerImage)
        {
            _controlModeImage.sprite = controlAccelerometerImage;
        }
        else if (PlayerState.Instance.Data.controlMode == ControlMode.FingerSwipe && _controlModeImage.sprite != controlFingerSwipeImage)
        {
            _controlModeImage.sprite = controlFingerSwipeImage;
        }
    }*/

    // Update is called once per frame
    void Update()
    {
		//Time.timeScale = (gameState == GameState.Paused && !_initiatingResume) ? 0 : 1;

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

		var currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber ();
		if (currentPlatform > highestPoint && countHeight)
        {
			//var roundedPosition = currentPlatform;
			var previous = highestPoint;
			highestPoint = currentPlatform;
            if (OnMaxHeightIncrease != null)
            {
				OnMaxHeightIncrease(highestPoint - previous, currentPlatform * SkyboxCameraMovement.speedMultiplier);
            }
        }

        if (PlatformController.Instance.GetCurrentPlatformNumber() > _previousPlatformNumber)
        {
            _previousPlatformNumber = PlatformController.Instance.GetCurrentPlatformNumber();
			powerMeter += PlayerState.Instance.playerLevel.energyAccumulationRate;
        }

        powerMeter = Mathf.Clamp(powerMeter, 0, PlayerState.Instance.playerLevel.maxEnergy);

        _powerBar.valueCurrent = Mathf.CeilToInt(powerMeter);
        _powerBar.SetValueMin(0);
		_powerBar.SetValueMax(Mathf.CeilToInt(PlayerState.Instance.playerLevel.maxEnergy));

        // time delay between player deaths
		if (playerIsDead && powerMeter >= PlayerState.Instance.playerLevel.lifeCost && gameState != GameState.Over)
        {
            _telekinesisControl.SetActive(false);

            if (!_cameraMovement.isTimedDestroyCutscene){
				_deathTimer -= Time.deltaTime;
			}

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
					powerMeter -= PlayerState.Instance.playerLevel.lifeCost;
                }
            }
        }

		else if (playerIsDead && powerMeter < PlayerState.Instance.playerLevel.lifeCost)
        {
			TriggerGameOver();
        }
        else
        {
            _deathTimer = timeBetweenDeaths;
            //movedFromSpawnPosition = false;
            initiatingRestart = false;
        }
    }

	void TriggerGameOver()
	{
		//_menuButtonBehaviour.interactable = false;
		_restartButtonBehaviour.interactable = true;
		if (OnGameOver != null && gameState != GameState.Over)
		{
			OnGameOver();
		}
	}

    void Restart()
    {
        //Debug.Log("Calling GameController.Restart");
        _menuButtonBehaviour.interactable = false;
        //_restartButton.GetComponent<Image>().enabled = false;
        _restartButtonBehaviour.interactable = false;
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

		if (_timeDestroyed)
		{
			TriggerGameOver();
		}
		else
		{
			var screenCenterToWorld =
				Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f));
			
			_player.GetComponent<PlayerMovement>().SetSpawnPosition(new Vector3(0, screenCenterToWorld.y, playerZPosition));
		}       
    }

    void HandleOnPlayerPowerDeplete(float amount)
    {
        powerMeter -= amount;
    }

    public void ButtonPlay()
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
				Time.timeScale = 1;
				_initiatingResume = true;
				Debug.Log ("Game resume through play button");
                break;
        }
    }

	public void ButtonShare()
	{
		StoreController.Instance.Native.socialShare ();
	}

	public void ButtonPhoto()
	{
		StoreController.Instance.Native.screenCapture ();
	}

    public void ButtonMenu()
    {
		if (!_isSettingsOpen)
		{
			_musicButtonImage.color = new Color(_musicButtonImage.color.r, _musicButtonImage.color.g, _musicButtonImage.color.b, 1f);
			_speakerImage.color = new Color(_speakerImage.color.r, _speakerImage.color.g, _speakerImage.color.b, 1f);
			_musicButtonBehaviour.interactable = true;

			_shareButtonImage.color = new Color(_shareButtonImage.color.r, _shareButtonImage.color.g, _shareButtonImage.color.b, 1f);
			_graphImage.color = new Color(_graphImage.color.r, _graphImage.color.g, _graphImage.color.b, 1f);
			_shareButtonBehaviour.interactable = true;
			//_controlModeImage.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);
			//_controlModeBehaviour.interactable = true;
			_menuButtonImage.color = new Color(_menuButtonImage.color.r, _menuButtonImage.color.g, _menuButtonImage.color.b, 1f);
		}
		else if (_isSettingsOpen)
		{
			_musicButtonImage.color = new Color(_musicButtonImage.color.r, _musicButtonImage.color.g, _musicButtonImage.color.b, 0);
			_speakerImage.color = new Color(_speakerImage.color.r, _speakerImage.color.g, _speakerImage.color.b, 0);
			_musicButtonBehaviour.interactable = false;

			_shareButtonImage.color = new Color(_shareButtonImage.color.r, _shareButtonImage.color.g, _shareButtonImage.color.b, 0);
			_graphImage.color = new Color(_graphImage.color.r, _graphImage.color.g, _graphImage.color.b, 0);
			_shareButtonBehaviour.interactable = false;
			//_controlModeImage.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0);
			//_controlModeBehaviour.interactable = false;
			_menuButtonImage.color = new Color(_menuButtonImage.color.r, _menuButtonImage.color.g, _menuButtonImage.color.b, 0);
		}
		
		_isSettingsOpen = !_isSettingsOpen;

		var menuAnimator = _menuButtonImage.GetComponentInChildren<Animator>();
		if (_isSettingsOpen && !menuAnimator.enabled)
		{
			menuAnimator.enabled = true;
		}
		else
		{
			menuAnimator.enabled = false;
		}
		
		if (gameState == GameState.Running)
		{
			if (OnGamePause != null)
			{
				OnGamePause();
			}
			
		}
    }

    public void ButtonControlMode()
    {
        switch (PlayerState.Instance.Data.controlMode)
        {
            case ControlMode.Accelerometer:
                PlayerState.Instance.Data.controlMode = ControlMode.FingerSwipe;
                break;
            case ControlMode.FingerSwipe:
                PlayerState.Instance.Data.controlMode = ControlMode.Accelerometer;
                break;
        }

        // HACK to trigger control mode change with EasyTouch events       
		_telekinesisControl.SetActive(true);
        _telekinesisControl.SetActive(false);
    }

    public void ButtonRestart()
    {
        Restart();
		initiatingRestart = true;
		Time.timeScale = 1;
    }

	public void ButtonAdContinue()
	{
		if(Advertisement.isReady()) {
			Advertisement.Show(null, new ShowOptions {
				pause = true,
				resultCallback = result => {
					switch (result)
					{
						case ShowResult.Finished:
							HandleOnPlayerResurrectionOnGameOver(false);
						break;
					}
				}
			});
		}
	}

	public void ButtonPurchaseContinue()
	{
		if (promptPurchaseContinue)
		{
			StoreController.Instance.Native.buyProduct (StoreController.Instance.Native.purchaseContinuationsIdentifier);
		}
		else
		{
			HandleOnPlayerResurrectionOnGameOver(true);
		}
	}
	
	public void ButtonMusic()
    {
        //PlayerState.Instance.Data.playMusic = !PlayerState.Instance.Data.playMusic;
		if (gameState == GameState.Started)
		{
			var indexLimit = (PlayerState.Instance.HasPurchasedAdditionalMusic) ? soundStateImages.Length - 1 : 1;
			if (PlayerState.Instance.Data.soundTrackSelection == indexLimit)
			{
				PlayerState.Instance.Data.soundTrackSelection = 0;
			}
			else
			{
				PlayerState.Instance.Data.soundTrackSelection = PlayerState.Instance.Data.soundTrackSelection+1;
			}
			Debug.Log ("selection: " + PlayerState.Instance.Data.soundTrackSelection + " limit: " + indexLimit);
			
			if (OnToggleMusic != null)
			{

				OnToggleMusic(PlayerState.Instance.Data.soundTrackSelection);
			}
			_selectedSoundTrack = PlayerState.Instance.Data.soundTrackSelection;
			_speakerImage.sprite = soundStateImages [PlayerState.Instance.Data.soundTrackSelection];
		}
		else
		{
			if (OnToggleMusic != null)
			{				
				OnToggleMusic((PlayerState.Instance.Data.soundTrackSelection > 0) ? 0 : _selectedSoundTrack);
			}
			_speakerImage.sprite = (PlayerState.Instance.Data.soundTrackSelection > 0) ? soundStateImages[_selectedSoundTrack] : soundStateImages[0];
		}
    }

	public void ButtonLeaderboardTop()
	{
		if (gameState == GameState.Running)
		{
			if (OnGamePause != null)
			{
				OnGamePause();
			}
			
		}
		StoreController.Instance.Native.showLeaderboards ();
	}

	public void ButtonStore()
	{
		if (!_isStoreOpen)
		{
			/*_storeButtonImage.color = new Color(_storeButtonImage.color.r, _storeButtonImage.color.g, _storeButtonImage.color.b, 1f);
			_restoreButtonImage.color = new Color(_restoreButtonImage.color.r, _restoreButtonImage.color.g, _restoreButtonImage.color.b, 1f);
			_restoreButtonBehaviour.interactable = true;
			_restoreButtonText.color = new Color(_restoreButtonText.color.r, _restoreButtonText.color.g, _restoreButtonText.color.b, 1f); 
			_cloudDownloadImage.color = new Color(_cloudDownloadImage.color.r, _cloudDownloadImage.color.g, _cloudDownloadImage.color.b, 1f);
			*/
			var storeButtonImages = _storeButtonImage.gameObject.GetComponentsInChildren<Image>();
			var storeButtonTexts = _storeButtonImage.gameObject.GetComponentsInChildren<Text>();
			var storeButtonBehaviours = _storeButtonImage.gameObject.GetComponentsInChildren<Button>();
			foreach (var image in storeButtonImages)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
			}

			foreach (var text in storeButtonTexts)
			{
				text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
			}

			foreach (var behaviour in storeButtonBehaviours)
			{
				behaviour.interactable = true;
			}

		}
		else if (_isStoreOpen)
		{
			/*_restoreButtonImage.color = new Color(_restoreButtonImage.color.r, _restoreButtonImage.color.g, _restoreButtonImage.color.b, 0);
			_restoreButtonBehaviour.interactable = false;
			_restoreButtonText.color = new Color(_restoreButtonText.color.r, _restoreButtonText.color.g, _restoreButtonText.color.b, 0);
			_cloudDownloadImage.color = new Color(_cloudDownloadImage.color.r, _cloudDownloadImage.color.g, _cloudDownloadImage.color.b, 0);*/
			var storeButtonImages = _storeButtonImage.gameObject.GetComponentsInChildren<Image>();
			var storeButtonTexts = _storeButtonImage.gameObject.GetComponentsInChildren<Text>();
			var storeButtonBehaviours = _storeButtonImage.gameObject.GetComponentsInChildren<Button>();
			foreach (var image in storeButtonImages)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
			}

			foreach (var text in storeButtonTexts)
			{
				text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
			}

			foreach (var behaviour in storeButtonBehaviours)
			{
				behaviour.interactable = false;
			}
			// override get components
			_storeButtonImage.color = new Color(_storeButtonImage.color.r, _storeButtonImage.color.g, _storeButtonImage.color.b, .6f);
			_plusImage.color = new Color(_plusImage.color.r, _plusImage.color.g, _plusImage.color.b, 1f);
			_storeButtonBehaviour.interactable = true;

		}
		
		_isStoreOpen = !_isStoreOpen;
		
		if (gameState == GameState.Running)
		{
			if (OnGamePause != null)
			{
				OnGamePause();
			}
			
		}
	}

	public void ButtonRestorePurchases()
	{
		var dialog = IOSDialog.Create("Restore Purchases", "Would you like to restore previously purchased items to this device?", "Yes", "Cancel");
		dialog.OnComplete += onRestoreDialogClose;
	}

	private void onRestoreDialogClose(IOSDialogResult result) {

		switch(result) {
			case IOSDialogResult.YES:
				Debug.Log ("Yes button pressed");
				StoreController.Instance.Native.restoreProducts ();
			break;
			case IOSDialogResult.NO:
				Debug.Log ("No button pressed");
			break;			
		}
	}

	public void ButtonBuyProduct(string productIdentifier)
	{
		StoreController.Instance.Native.buyProduct(productIdentifier);
	}

    void HandleOnShowMenuButtons()
    {
        _playButtonImage.enabled = true;
		_playButtonBehaviour.enabled = true;
		_triangleImage.enabled = true;
		_levelsInfoText.text = string.Format ("{0} platforms to level up", PlayerState.Instance.GetPlatformDeltaToNextLevel());
		//_highestPointText.text = PlayerState.Instance.Data.highestPlatform.ToString();
		//_totalHeightText.text = PlayerState.Instance.Data.totalPlatforms.ToString();
    }

	private void CloseSettingsAndStore()
	{
		if (_isSettingsOpen) 
		{
			_musicButtonImage.color = new Color(_musicButtonImage.color.r, _musicButtonImage.color.g, _musicButtonImage.color.b, 0);
			_speakerImage.color = new Color(_speakerImage.color.r, _speakerImage.color.g, _speakerImage.color.b, 0);
			_musicButtonBehaviour.interactable = false;

			_shareButtonImage.color = new Color(_shareButtonImage.color.r, _shareButtonImage.color.g, _shareButtonImage.color.b, 0);
			_graphImage.color = new Color(_graphImage.color.r, _graphImage.color.g, _graphImage.color.b, 0);
			_shareButtonBehaviour.interactable = false;
			//_controlModeImage.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);
			//_controlModeBehaviour.interactable = false;
			_isSettingsOpen = false;
			
			_menuButtonImage.GetComponentInChildren<Animator>().enabled = false;
			_menuButtonImage.color = new Color(_menuButtonImage.color.r, _menuButtonImage.color.g, _menuButtonImage.color.b, 0);
		}
		
		if (_isStoreOpen) 
		{
			var storeButtonImages = _storeButtonImage.gameObject.GetComponentsInChildren<Image>();
			var storeButtonTexts = _storeButtonImage.gameObject.GetComponentsInChildren<Text>();
			var storeButtonBehaviours = _storeButtonImage.gameObject.GetComponentsInChildren<Button>();
			foreach (var image in storeButtonImages)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
			}
			
			foreach (var text in storeButtonTexts)
			{
				text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
			}
			
			foreach (var behaviour in storeButtonBehaviours)
			{
				behaviour.interactable = false;
			}
			// override get components
			_storeButtonImage.color = new Color(_storeButtonImage.color.r, _storeButtonImage.color.g, _storeButtonImage.color.b, .6f);
			_plusImage.color = new Color(_plusImage.color.r, _plusImage.color.g, _plusImage.color.b, 1f);
			_storeButtonBehaviour.interactable = true;

			_isStoreOpen = false;
		}
	}

    void HandleOnGameStart()
    {
        gameState = GameState.Running;

		CloseSettingsAndStore ();
		
        _playButtonImage.enabled = false;
		_triangleImage.enabled = false;
        _playButtonImage.rectTransform.anchorMin = new Vector2(.5f, .5f);
        _playButtonImage.rectTransform.anchorMax = new Vector2(.5f, .5f);
        _playButtonImage.rectTransform.anchoredPosition = new Vector3(0, 0, 0);
		_playButtonImage.rectTransform.pivot = new Vector2 (.5f, .5f);
		_playButtonImage.rectTransform.localScale = new Vector2 (1.3f, 1.3f);
		//_highestPointText.text = string.Empty;
		//_totalHeightText.text = string.Empty;
		_levelsInfoText.enabled = false;
    }

    void HandleOnGamePause()
    {
		Time.timeScale = 0;
		gameState = GameState.Paused;
        _player.GetComponent<PlayerMovement>().disabled = true;
        _telekinesisControl.SetActive(false);
        _restartButtonImage.enabled = true;
		_arrowImage.enabled = true;
        _restartButtonBehaviour.interactable = true;
        _playButtonImage.enabled = true;
		_triangleImage.enabled = true;
        _playButtonBehaviour.interactable = true;
		_highestPointText.text = PlayerState.Instance.Data.highestPlatform.ToString ();
		//_totalHeightText.text = PlayerState.Instance.Data.totalPlatforms.ToString();
		heightCounter.color = Color.white;
    }

    void HandleOnGameResume()
    {
		gameState = GameState.Running;
        _player.GetComponent<PlayerMovement>().disabled = false;
        _telekinesisControl.SetActive(true);
        _restartButtonImage.enabled = false;
		_arrowImage.enabled = false;
        _restartButtonBehaviour.interactable = false;
        _playButtonImage.enabled = false;
		_triangleImage.enabled = false;
        _playButtonBehaviour.interactable = false;

		CloseSettingsAndStore ();

		//_highestPointText.text = string.Empty;
		//_totalHeightText.text = string.Empty;
		heightCounter.color = heightCounterColor;
        //_menuButtonImage.GetComponent<Animator>().enabled = false;
        //_musicButtonImage.enabled = false;
        //_cartButtonImage.enabled = false;
        //_isSettingsOpen = false;
    }

    void HandleOnGameOver()
    {
        gameState = GameState.Over;
        //_player.GetComponent<PlayerMovement>().disabled = true;
        _telekinesisControl.SetActive(false);
        _restartButtonImage.enabled = true;
		_arrowImage.enabled = true;
        _restartButtonBehaviour.interactable = true;
		_highestPointText.text = string.Format(PlayerState.Instance.Data.highestPlatform.ToString());
		//_totalHeightText.text = PlayerState.Instance.Data.totalPlatforms.ToString();
		heightCounter.color = Color.white;

		if (StoreController.Instance.Native.authenticated)
		{
			StoreController.Instance.Native.submitScore (highestPoint, StoreController.Instance.Native.leaderboardHighestName); //TOOD: consitent naming
			StoreController.Instance.Native.submitScore (PlayerState.Instance.Data.totalPlatforms, StoreController.Instance.Native.leaderboardTotalName);
		}

		// TODO display continue UI
		/*_continueButtonImage.enabled = promptAdContinue;
		_continueButtonText.enabled = promptAdContinue;
		_continueButtonBehaviour.enabled = promptAdContinue;*/
	}
	
	private void HandleOnPowerPickUp(float powerToAdd)
    {
        //Debug.Log("Add " + powerToAdd + " to power meter.");
		if (inSafeZone) 
		{
			UpdateSafeZone(false);		
		}
        powerMeter += powerToAdd;
    }

    private void HandleOnGameRestart(int sceneindex)
    {
        //gameState = GameState.Running;
		CloseSettingsAndStore ();
		_playButtonImage.enabled = false;
		_triangleImage.enabled = false;
        _playButtonBehaviour.interactable = false;
		//_shareButtonBehaviour.interactable = false;

        //_cartButtonImage.enabled = false;
		_menuButtonImage.GetComponentInChildren<Animator>().enabled = false;
    }

	void HandleOnPlayerResurrectionOnGameOver(bool fromIAP)
	{
		gameState = GameState.Running;
		initiatingRestart = false;
		/*_continueButtonImage.enabled = false;
		_continueButtonText.enabled = false;*/
		powerMeter += (fromIAP) ? PlayerState.Instance.playerLevel.maxEnergy : continueAdPower;

		//Debug.Log ("Player reward spawn: " + newSpawnPosition);
		if (fromIAP)
		{
			PlayerState.Instance.Data.gameOverContinues --;
		}
		else
		{
			advertisingContinues --;
		}

		_restartButtonImage.enabled = false;
		_restartButtonBehaviour.interactable = false;
		_arrowImage.enabled = false;
		_restartButtonBehaviour.interactable = false;

		_highestPointText.text = string.Empty;
		//_totalHeightText.text = string.Empty;

		CloseSettingsAndStore ();

		if (OnPlayerReward != null)
		{
			OnPlayerReward();
		}
	}
}
