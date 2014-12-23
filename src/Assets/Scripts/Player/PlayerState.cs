using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;
using LegendPeak.Player;

public class PlayerState : MonoBehaviour
{
	private float lastTime;

	public string dataPath;
    public PlayerState Instance { get; private set; }
    public PlayerData Data { get; private set; }

	void OnEnable()
	{
		//GameController.OnGameStart += HandleOnGameStart;
		GameController.OnGamePause += HandleOnGamePause;
		//OnGameResume += HandleOnGameResume;
		GameController.OnGameOver += HandleOnGameOver;
		GameController.OnGameRestart += HandleOnGameRestart;     
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
		//GameController.OnGameStart -= HandleOnGameStart;
		GameController.OnGamePause -= HandleOnGamePause;
		//OnGameResume -= HandleOnGameResume;
		GameController.OnGameOver -= HandleOnGameOver;
		GameController.OnGameRestart -= HandleOnGameRestart;
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
		FileStream playerFile;
		var binaryFormatter = new BinaryFormatter();

		if (GameController.Instance.highestPoint > Data.highestPoint) 
		{
			Data.highestPoint = GameController.Instance.highestPoint;
			//Data.highestPoint = 0;
		}

		Debug.Log("Before: " + lastTime);
		var toAdd = GameController.Instance.highestPoint - lastTime;
		lastTime = GameController.Instance.highestPoint;
		Debug.Log ("After: " + lastTime);

		Debug.Log("Added: " + toAdd);
		Data.totalHeight += toAdd;
		//Data.totalHeight = 0;

		if (!File.Exists(string.Format(dataPath, Application.persistentDataPath)))
		{
			playerFile = File.Create(string.Format(dataPath, Application.persistentDataPath));
		}
		else
		{
			playerFile = File.OpenWrite(string.Format(dataPath, Application.persistentDataPath));
		}		      
        
        binaryFormatter.Serialize(playerFile, Data);
        playerFile.Close();
    }

    public void Load()
    {
        if (!File.Exists(string.Format(dataPath, Application.persistentDataPath)))
		{
			Data = new PlayerData();
		}
		else
		{
			var binaryFormatter = new BinaryFormatter();
			var playerFile = File.Open(string.Format(dataPath, Application.persistentDataPath), FileMode.Open);
			
			Data = binaryFormatter.Deserialize(playerFile) as PlayerData;
			playerFile.Close ();
		}
       
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

	void HandleOnGameRestart(int levelToLoad)
	{
		lastTime = 0;
	}
}
