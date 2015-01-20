////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class AppEventHandlerExample : MonoBehaviour {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {

		//Event use example
		IOSNativeAppEvents.instance.addEventListener (IOSNativeAppEvents.APPLICATION_DID_BECOME_ACTIVE,                OnApplicationDidBecomeActive);

		//Action use example
		IOSNativeAppEvents.instance.OnApplicationDidReceiveMemoryWarning += OnApplicationDidReceiveMemoryWarning;
	}

	//--------------------------------------
	// EVENTS
	//--------------------------------------
	

	private void OnApplicationDidBecomeActive() {
		// Called when application become active again. Optionally refresh the user interface, check for some data than probably was chenged wile application was paused

		Debug.Log ("Catched  OnApplicationDidBecomeActive event");
	}

	private void OnApplicationDidReceiveMemoryWarning() {
		//Called application receives a memory warning from the system.

		Debug.Log ("Catched  OnApplicationDidReceiveMemoryWarning event");
	}


	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	// DESTROY
	//--------------------------------------
}
