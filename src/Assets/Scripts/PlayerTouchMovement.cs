using UnityEngine;
using System.Collections;

public class PlayerTouchMovement : MonoBehaviour {

	private RotationControl _rotationControl;

	public GameObject playerObject;

	void Start()
	{
		_rotationControl = GameObject.Find("RotationControl").GetComponent<RotationControl>();
	}

	// Subscribe to events
	void OnEnable(){
		EasyJoystick.On_JoystickTap += On_JoystickTap;
		EasyJoystick.On_JoystickMoveStart += On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove += On_JoystickMoveStart;
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;
	}

	void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyJoystick.On_JoystickTap -= On_JoystickTap;
		EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
	}

	void On_JoystickTap(MovingJoystick movingStick){

		//gameObject.GetComponent<EasyJoystick>().isActivated = false;

		//Debug.Log(playerObject.name);
		if (playerObject != null) {
			Debug.Log ("Jump through joystick");
			playerObject.GetComponent<PlayerMovement>().Jump();
		}
	}

	void On_JoystickMoveStart (MovingJoystick move)
	{
		_rotationControl.shouldRotate = false;
	}

	void On_JoystickMoveEnd (MovingJoystick move)
	{
		_rotationControl.shouldRotate = true;
	}

}
