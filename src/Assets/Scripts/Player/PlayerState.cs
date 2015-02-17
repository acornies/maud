using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LegendPeak;
using LegendPeak.Player;
using LegendPeak.Native;

public class PlayerState : MonoBehaviour
{
	private int lastTime;
    private Image _controlModeImage;

	public string dataPath;
	public PlayerLevel[] playerLevels;

    public static PlayerState Instance { get; private set; }
    public PlayerData Data { get; private set; }

	public delegate void SuccessfullContinuationPurchase();
	public static event SuccessfullContinuationPurchase OnSuccessfullContinuationPurchase;

	public delegate void SuccessfullNonConsumable(string productId);
	public static event SuccessfullNonConsumable OnSuccessfullNonConsumable;
	
	void OnEnable()
	{
		GameController.OnGamePause += HandleOnGamePause;
		GameController.OnGameResume += HandleOnGameResume;
		GameController.OnGameOver += HandleOnGameOver;
		GameController.OnGameRestart += HandleOnGameRestart;
        GameController.OnGameStart += HandleOnGameStart;
	    GameController.OnToggleMusic += HandleToggleMusic;
		GameController.OnMaxHeightIncrease += HandleOnMaxHeightIncrease;
	}

	void HandleOnMaxHeightIncrease (int delta, float rotationMultiplier)
	{
		if (GameController.Instance.highestPoint > Data.highestPlatform) 
		{
			Data.highestPlatform = GameController.Instance.highestPoint;
			//Data.highestPoint = 0;
		}
		Data.totalPlatforms += delta;
	}

	void HandleTransactionComplete (object sender, EventArgs e)
	{
		//Debug.Log ("purchase made it to PlayerState.HandleTransactionComplete");
		//if (e.GetType () == typeof(IStoreResponse) )
		var isStoreResponse = typeof(IStoreResponse).IsAssignableFrom (e.GetType ());
		if (isStoreResponse)
		{
			var storeResponse = e as IStoreResponse;
			switch (storeResponse.productId)
			{
				case StoreController.REVIVAL_PACK:
					
					switch (storeResponse.status)
					{
						case StoreResponseStatus.Success:
							Debug.Log("Player successfully purchased continuations.");
							// TODO: some UI for successfull game conitnue purchases
							Data.gameOverContinues += 10; // TODO: move to editor
							Save();
							if (OnSuccessfullContinuationPurchase != null)
							{
								OnSuccessfullContinuationPurchase();
							}
							break;
						default:
							Debug.Log("Player tried to purchase continuations but it failed.");
							break;
					}
				break;

				case StoreController.MUSIC_PACK:
					
					switch (storeResponse.status)
					{
						case StoreResponseStatus.Success:
						case StoreResponseStatus.Restored:
							Debug.Log("Player successfully purchased music pack.");
							HandleNonConsumableRestoreOrPurchase(storeResponse.productId);
						break;
						default:
							Debug.Log("Player tried to purchase music but it failed.");
						break;
					}

				break;
			}
		}
	}

	private void HandleNonConsumableRestoreOrPurchase(string productId)
	{
		if (Data.purchasedProductIds == null || Data.purchasedProductIds.Length == 0)
		{
			Data.purchasedProductIds = new string[]{ productId };
			this.Save();
		}
		else
		{
			var list = new List<string>();
			foreach(var product in Data.purchasedProductIds)
			{
				list.Add(product);
			}

			if (list.Contains(productId)) return; // don't add duplicate products

			// now add new item
			list.Add(productId);
			Data.purchasedProductIds = list.ToArray();
			this.Save();
		}

		if (OnSuccessfullNonConsumable != null)
		{
			OnSuccessfullNonConsumable(productId);
		}
	}

    private void HandleToggleMusic(int selection)
    {
        Data.soundTrackSelection = selection;
        Debug.Log("Save music preference: " + selection);
        Save();
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
		GameController.OnMaxHeightIncrease -= HandleOnMaxHeightIncrease;
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
		StoreController.Instance.Native.OnTransactionComplete += HandleTransactionComplete;
    }

	public bool HasPurchasedAdditionalMusic
	{
		get
		{
			if (Data.purchasedProductIds == null || Data.purchasedProductIds.Length == 0) return false;
			var list = new List<string>();
			foreach(var product in Data.purchasedProductIds)
			{
				list.Add(product);
			}

			return list.Contains(StoreController.MUSIC_PACK);
		}
	}

    public void Save()
    {
        var binaryFormatter = new BinaryFormatter();

		/*if (GameController.Instance.highestPoint > Data.highestPlatform) 
		{
			Data.highestPlatform = GameController.Instance.highestPoint;
			//Data.highestPoint = 0;
		}*/

		//Debug.Log("Before: " + lastTime);
		/*var toAdd = GameController.Instance.highestPoint - lastTime;
		lastTime = GameController.Instance.highestPoint;
		//Debug.Log ("After: " + lastTime);

		//Debug.Log("Added: " + toAdd);
		Data.totalPlatforms += toAdd;
		*/
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
				gameOverContinues = 0,
				viewedTutorialGeneral = false
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
