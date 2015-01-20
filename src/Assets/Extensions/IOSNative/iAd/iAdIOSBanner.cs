////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//Attach the script to the empty gameobject on your sceneS
public class iAdIOSBanner : MonoBehaviour {
	
	public TextAnchor anchor = TextAnchor.LowerCenter;


	private static Dictionary<string, iAdBanner> _registerdBanners = null;


	// --------------------------------------
	// Unity Events
	// --------------------------------------


	void Start() {
		ShowBanner();
	}

	void OnDestroy() {
		HideBanner();
	}


	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------

	public void ShowBanner() {
		iAdBanner banner;

		if(registerdBanners.ContainsKey(sceneBannerId)) {
			banner = registerdBanners[sceneBannerId];
		}  else {
			banner = iAdBannerController.instance.CreateAdBanner(anchor);
			registerdBanners.Add(sceneBannerId, banner);
		}

		if(banner.IsLoaded && !banner.IsOnScreen) {
			banner.Show();
		}
	}

	public void HideBanner() {
		if(registerdBanners.ContainsKey(sceneBannerId)) {
			iAdBanner banner = registerdBanners[sceneBannerId];
			if(banner.IsLoaded) {
				if(banner.IsOnScreen) {
					banner.Hide();
				}
			} else {
				banner.ShowOnLoad = false;
			}
		}
	}

	// --------------------------------------
	// GET / SET
	// --------------------------------------


	public static Dictionary<string, iAdBanner> registerdBanners {
		get {
			if(_registerdBanners == null) {
				_registerdBanners = new Dictionary<string, iAdBanner>();
			}

			return _registerdBanners;
		}
	}

	public string sceneBannerId {
		get {
			return Application.loadedLevelName + "_" + this.gameObject.name;
		}
	}

	
}
