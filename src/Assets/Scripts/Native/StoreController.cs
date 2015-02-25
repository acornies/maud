using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LegendPeak.Native;

public class StoreController : MonoBehaviour {
	
	private Image _storeButtonImage;
	private Image _restoreButtonImage;
	private Button _restoreButtonBehaviour;

	public static StoreController Instance { get; private set; }
	public IStoreManager Native { get; private set; }

	public const string SKIN_PACK = "com.AndrewCornies.LegendPeak.SkinPack1";
	//public const string REVIVAL_PACK = "com.AndrewCornies.LegendPeak.RevivalPack1";
	//public const string MUSIC_PACK = "com.AndrewCornies.LegendPeak.AltMusicPack1";

	public NonConsumableProduct[] availableProducts;

	void Awake()
	{
		switch(Application.platform)
		{
			//case RuntimePlatform.OSXEditor:
			case RuntimePlatform.IPhonePlayer:
				Native = new AppleStoreManager();
				//Debug.Log ("Loaded AppleStoreManager");
				break;
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.OSXEditor:
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
	void Update () 
	{
	
	}

	void OnGUI()
	{

	}

	public NonConsumableProduct[] GetPurchasabletems()
	{
		var playerPurchased = PlayerState.Instance.Data.purchasedProductIds;
		if (playerPurchased != null && playerPurchased.Length > 0)
		{
			foreach(var item in playerPurchased)
			{
				availableProducts.FirstOrDefault( x => x.identifier.Equals(item, StringComparison.InvariantCultureIgnoreCase)).purchased = true;
			}
			
			return availableProducts.Where (x => !x.purchased).ToArray();
		}
		else
		{
			return availableProducts;
		}

	}
}

[Serializable]
public class MaudProduct
{
	public string identifier;
	public string title;
	public string description;
	public Sprite image;
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