using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LegendPeak.Native;

public class StoreController : MonoBehaviour {
	
	public static StoreController Instance { get; private set; }
	public IStoreManager Native { get; private set; }

	public const string SKIN_PACK = "com.AndrewCornies.LegendPeak.SkinPack1";
	public const string REVIVAL_PACK = "com.AndrewCornies.LegendPeak.RevivalPack1";
	public const string MUSIC_PACK = "com.AndrewCornies.LegendPeak.AltMusicPack1";

	public NonConsumableProduct[] availableProducts;

	void Awake()
	{
		switch(Application.platform)
		{
			//case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.IPhonePlayer:
				Native = new AppleStoreManager();
				//Debug.Log ("Loaded AppleStoreManager");
				break;
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.Android:
				//instance = 
				//TODO: implement
				Native = new GoogleStoreManager();
				//Debug.Log ("Loaded GoogleStoreManager");
				break;
		}

		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
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

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

[Serializable]
public class MaudProduct
{
	public string identifier;
	public string description;
}

[Serializable]
public class NonConsumableProduct : MaudProduct
{
	public bool purchased;
}

[Serializable]
public class ConsumableProduct : MaudProduct
{

}
