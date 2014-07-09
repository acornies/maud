using UnityEngine;
using System.Collections;

public class RotationControl : MonoBehaviour {

	public float sensitivity = 0.005f;
	public Transform platform;

	private Vector3 _pointerReference;
	private Vector3 _pointerOffset;
	private Vector3 _rotation = Vector3.zero;
	public bool shouldRotate;

	// Subscribe to events
	void OnEnable(){
		EasyTouch.On_Swipe += On_Swipe;
		EasyTouch.On_SwipeStart += On_SwipeStart;
		EasyTouch.On_SwipeEnd += On_SwipeEnd;
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
		EasyTouch.On_Swipe -= On_Swipe;
		EasyTouch.On_SwipeStart -= On_SwipeStart;
		EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
	}

	void On_SwipeStart(Gesture gesture)
	{
		shouldRotate = true;
		_pointerReference = gesture.position;
		RaycastHit hitInfo = new RaycastHit();
		bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(gesture.position), out hitInfo);
		if (hit) 
		{
			//Debug.Log("Hit " + hitInfo.transform.gameObject.name);
			if (hitInfo.transform.gameObject.tag == "Rotatable")
			{
				//Debug.Log ("It's active");
				this.platform = hitInfo.transform;
			}
			if (hitInfo.transform.gameObject.tag == "Stoppable")
			{
				//Debug.Log("Hit child, rotate parent: " + hitInfo.transform.parent.name);
				this.platform = hitInfo.transform.parent;
			}
		}
	}

	void On_Swipe(Gesture gesture)
	{
		Debug.Log("swiping... isRotating is: " + shouldRotate);
		if(shouldRotate && platform != null)
		{
			// offset
			_pointerOffset = (new Vector3(gesture.position.x, gesture.position.y, 0) - _pointerReference);
			
			// apply rotation
			_rotation.y = -(_pointerOffset.x + _pointerOffset.y) * sensitivity;
			
			// rotate
			platform.Rotate(_rotation);
			
			// store mouse
			_pointerReference = gesture.position;
			//Debug.Log(platform.gameObject.name + " is rotating.");
		}
	}

	void On_SwipeEnd(Gesture gesture)
	{
		shouldRotate = false;
		platform = null;
	}

	void On_JoystickMoveStart (MovingJoystick move)
	{
		shouldRotate = false;
	}
	
	void On_JoystickMoveEnd (MovingJoystick move)
	{
		shouldRotate = true;
	}
}
