using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LegendPeak;
using LegendPeak.Player;

public class PlayerState : MonoBehaviour
{
	private int lastTime;
    private Image _controlModeImage;

	public string dataPath;
	public PlayerLevel[] playerLevels;

    public static PlayerState Instance { get; private set; }
    public PlayerData Data { get; private set; }

	void OnEnable()
	{
		GameController.OnGamePause += HandleOnGamePause;
		GameController.OnGameResume += HandleOnGameResume;
		GameController.OnGameOver += HandleOnGameOver;
		GameController.OnGameRestart += HandleOnGameRestart;
        GameController.OnGameStart += HandleOnGameStart;
	    GameController.OnToggleMusic += HandleToggleMusic;
	}

    private void HandleToggleMusic(bool playmusic)
    {
        Data.playMusic = playmusic;
        Debug.Log("Save music preference");
        this.Save();
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
		GameController.OnGamePause -= HandleOnGamePause;
		GameController.OnGameResume -= HandleOnGameResume;
		GameController.OnGameOver -= HandleOnGameOver;
		GameController.OnGameRestart -= HandleOnGameRestart;
        GameController.OnGameStart -= HandleOnGameStart;
        GameController.OnToggleMusic -= HandleToggleMusic;
	}

    void Awake()
    {
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		
		if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        this.Load();
    }
    
    // Use this for initialization
    void Start()
    {
    }

    public void Save()
    {
        var binaryFormatter = new BinaryFormatter();

		if (GameController.Instance.highestPoint > Data.highestPlatform) 
		{
			Data.highestPlatform = GameController.Instance.highestPoint;
			//Data.highestPoint = 0;
		}

		//Debug.Log("Before: " + lastTime);
		var toAdd = GameController.Instance.highestPoint - lastTime;
		lastTime = GameController.Instance.highestPoint;
		//Debug.Log ("After: " + lastTime);

		//Debug.Log("Added: " + toAdd);
		Data.totalPlatforms += toAdd;
		//Data.totalHeight = 0;

		FileStream playerFile = !File.Exists(string.Format(dataPath, Application.persistentDataPath)) 
		    ? File.Create(string.Format(dataPath, Application.persistentDataPath)) 
		    : File.OpenWrite(string.Format(dataPath, Application.persistentDataPath));		      
        
        binaryFormatter.Serialize(playerFile, Data);
        playerFile.Close();
    }

    public void Load()
    {
        if (!File.Exists(string.Format(dataPath, Application.persistentDataPath)))
		{
			Data = new PlayerData(){
				// TODO enable once supported
				//controlMode = (MobileHelper.isTablet) ? ControlMode.FingerSwipe : ControlMode.Accelerometer,
				controlMode = ControlMode.Accelerometer,
				playMusic = true,
				highestPlatform = 0,
				totalPlatforms = 0,
				monetizedState = MonetizedState.Free,
				playerLevel = 1,
				gameOverContinues = 0
			};
		}
		else
		{
			var binaryFormatter = new BinaryFormatter();
			var playerFile = File.Open(string.Format(dataPath, Application.persistentDataPath), FileMode.Open);
			
			Data = binaryFormatter.Deserialize(playerFile) as PlayerData;
			playerFile.Close ();
		}
       
    }

	public PlayerLevel playerLevel
	{
		get { return playerLevels[Data.playerLevel - 1]; }
	}

    // Update is called once per frame
    void Update()
    {

    }

	void HandleOnGamePause()
	{
		this.Save ();
	}

	void HandleOnGameOver()
	{
		this.Save ();
	}

    private void HandleOnGameStart()
    {
        this.Save();
    }

    private void HandleOnGameResume()
    {
        this.Save();
    }

	void HandleOnGameRestart(int levelToLoad)
	{
		lastTime = 0;
	}
}
