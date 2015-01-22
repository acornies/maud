using UnityEngine;
using System.Collections;
using LegendPeak.Native;

public class StoreController : MonoBehaviour {

	public static IStoreManager instance { get; private set;}

	void Awake()
	{
		switch(Application.platform)
		{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.IPhonePlayer:
				instance = new AppleStoreManager();
				Debug.Log ("Loaded AppleStoreManager");
				break;
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.Android:
				//instance = 
				//TODO: implement
				Debug.Log ("Loaded GoogleStoreManager");
				break;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
