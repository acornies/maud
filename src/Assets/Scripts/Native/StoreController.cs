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

		var storeButton = GameObject.Find ("StoreButton");
		_storeButtonImage = storeButton.GetComponent<Image> ();
		var restoreButton = storeButton.transform.FindChild ("RestoreButton");
		_restoreButtonImage = restoreButton.GetComponent<Image> ();
	}

	// Use this for initialization
	void Start () 
	{
		var products = GetPurchasabletems ();
		float yPositionOffset = 0;
		foreach(var product in products)
		{
			GameObject newButton = Instantiate(_restoreButtonImage.gameObject) as GameObject;
			newButton.transform.SetParent(_storeButtonImage.transform);
			newButton.name = product.identifier;
			var behaviour = newButton.GetComponent<Button>();
			var mainImage = newButton.GetComponent<Image>();
			var childSprite = newButton.transform.FindChild("Cloud").GetComponent<Image>();
			childSprite.name = product.description;
			var buttonText = newButton.transform.FindChild("Text").GetComponent<Text>();
			buttonText.text = product.description;
			childSprite.sprite = product.image;
			behaviour.onClick.RemoveAllListeners();
			behaviour.onClick.AddListener(() => { ButtonBuyProduct(product.identifier); });
			yPositionOffset += mainImage.rectTransform.sizeDelta.y;
			mainImage.rectTransform.anchoredPosition = new Vector2(0, -yPositionOffset);
			mainImage.rectTransform.localScale = new Vector2(1f, 1f);
		}
	}

	public void ButtonBuyProduct(string productIdentifier)
	{
		Native.buyProduct(productIdentifier);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnGUI()
	{

	}

	private NonConsumableProduct[] GetPurchasabletems()
	{
		var playerPurchased = PlayerState.Instance.Data.purchasedProductIds;
		if (playerPurchased != null && playerPurchased.Length > 0)
		{
			foreach(var item in playerPurchased)
			{
				availableProducts.FirstOrDefault( x => x.identifier == item).purchased = true;
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