////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementTemplate  {



	public string id;
	public float _progress;



	public float progress {
		get {
			if(IOSNativeSettings.Instance.UsePPForAchievements) {
				return GameCenterManager.getAchievementProgress(id);
			} else {
				return _progress;
			}

		}

		set {
			_progress = value;
		}
	}
}
