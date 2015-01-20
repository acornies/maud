//#define PUSH_ENABLED
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
using UnityEngine;
#else
using UnityEngine.iOS;
#endif

using System.Collections;

public class DeviceTokenListner : UnityEngine.MonoBehaviour {
	
	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public static void Create() {
		new UnityEngine.GameObject ("DeviceTokenListner").AddComponent<DeviceTokenListner> ();
	}


	void Awake() {
		DontDestroyOnLoad (gameObject);

	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	


	  
	 
	#if (UNITY_IPHONE && !UNITY_EDITOR && PUSH_ENABLED) || SA_DEBUG_MODE
	
	private bool tokenSent = false;

	void  FixedUpdate () {


		
		if (!tokenSent) {

			byte[] token   = NotificationServices.deviceToken;
			//Debug.Log(NotificationServices.deviceToken);
			if(token != null) {

				IOSNotificationDeviceToken t = new IOSNotificationDeviceToken(token);
				IOSNotificationController.instance.OnDeviceTockeReceivedAction (t);
				Destroy (gameObject);
			}
		}

	}

	#endif

	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
