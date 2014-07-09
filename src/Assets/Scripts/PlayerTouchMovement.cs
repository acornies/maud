using UnityEngine;
using System.Collections;

public class PlayerTouchMovement : MonoBehaviour {

	public GameObject playerObject;

	// Subscribe to events
	void OnEnable(){
		EasyJoystick.On_JoystickTap += On_JoystickTap;
	}
	
	void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyJoystick.On_JoystickTap -= On_JoystickTap;	
	}
	
	// Simple tap
	private void On_JoystickTap(MovingJoystick movingStick){

		//gameObject.GetComponent<EasyJoystick>().isActivated = false;

		//Debug.Log(playerObject.name);
		if (playerObject != null) {
			Debug.Log ("Jump through joystick");
			playerObject.GetComponent<PlayerMovement>().Jump();
		}
	}

}
