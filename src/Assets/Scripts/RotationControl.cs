using UnityEngine;
using System.Collections;

public class RotationControl : MonoBehaviour 
{
	private Vector3 _pointerReference;
	private Vector3 _pointerOffset;
	private Vector3 _rotation = Vector3.zero;
    private float _longTapEndTimer;
    private bool _isLongTapTimingOut;

    public float sensitivity = 0.005f;
    public Transform platform;
    public bool shouldRotate;
	public bool isRotating = false;
	public float longTapTimeout = 1.0f;

	public delegate void RotationPowersStart();
	public static event RotationPowersStart On_PlayerRotationPowersStart;

	public delegate void RotationPowersEnd();
	public static event RotationPowersEnd On_PlayerRotationPowersEnd;

	// Subscribe to events
	void OnEnable()
	{
		EasyTouch.On_Swipe += On_Swipe;
		EasyTouch.On_SwipeStart += On_SwipeStart;
		EasyTouch.On_SwipeEnd += On_SwipeEnd;
		EasyTouch.On_LongTapStart += HandleLongTap;
		EasyTouch.On_LongTapEnd += HandleLongTapEnd;
		PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;

		/*EasyJoystick.On_JoystickMoveStart += On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove += On_JoystickMoveStart;
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;*/
	}

	void OnDisable()
	{
		UnsubscribeEvent();
	}
	
	void OnDestroy()
	{
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent()
	{
		EasyTouch.On_Swipe -= On_Swipe;
		EasyTouch.On_SwipeStart -= On_SwipeStart;
		EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		EasyTouch.On_LongTapStart -= HandleLongTap;
		EasyTouch.On_LongTapEnd -= HandleLongTapEnd;
		PlayerMovement.On_PlayerAirborne -= HandlePlayerAirborne;
		/*EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMove -= On_JoystickMoveStart;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;*/
	}

    void Update()
    {        
        if (_isLongTapTimingOut)
        {
            if (shouldRotate && !isRotating)
            {
                _longTapEndTimer -= Time.deltaTime;
                if (!(_longTapEndTimer <= 0)) return;
                shouldRotate = false;
                platform.particleSystem.Stop();
                platform = null;
                if (On_PlayerRotationPowersEnd != null)
                {
                    On_PlayerRotationPowersEnd();
                }
                _isLongTapTimingOut = false;
            }
        }
        else
        {
            _longTapEndTimer = longTapTimeout;
        }
    }

	void HandlePlayerAirborne ()
	{
		shouldRotate = false;
	}
	
	void HandleLongTap (Gesture gesture)
	{
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
				
				if (On_PlayerRotationPowersStart != null)
				{
					On_PlayerRotationPowersStart();
				}
				shouldRotate = true;
                if (platform != null && platform.particleSystem != null)
			    {
                    platform.particleSystem.Play();
			    }        
			}
			if (hitInfo.transform.gameObject.tag == "Stoppable")
			{
				//Debug.Log("Hit child, rotate parent: " + hitInfo.transform.parent.name);
				this.platform = hitInfo.transform.parent;
				
				if (On_PlayerRotationPowersStart != null)
				{
					On_PlayerRotationPowersStart();
				}
				shouldRotate = true;
                if (platform != null && platform.particleSystem != null)
                {
                    platform.particleSystem.Play();
                }   
			}
		}
	}
    void HandleLongTapEnd(Gesture gesture)
    {
        if (platform != null)
        {
            _isLongTapTimingOut = true;
        }
    }

	void On_SwipeStart(Gesture gesture)
	{

	}

	void On_Swipe(Gesture gesture)
	{
	    _isLongTapTimingOut = false;
        
        //Debug.Log("swiping... isRotating is: " + shouldRotate);
	    if (!shouldRotate || platform == null) return;
	    // offset
	    _pointerOffset = (new Vector3(gesture.position.x, gesture.position.y, 0) - _pointerReference);
			
	    // apply rotation
	    _rotation.y = -(_pointerOffset.x + _pointerOffset.y) * sensitivity;

	    isRotating = true;

	    // rotate
	    platform.Rotate(_rotation);
			
	    // store mouse
	    _pointerReference = gesture.position;
	    //Debug.Log(platform.gameObject.name + " is rotating.");
	}

	void On_SwipeEnd(Gesture gesture)
	{
		shouldRotate = false;
		isRotating = false;
        if (platform != null && platform.particleSystem != null)
	    {
            platform.particleSystem.Stop();   
	    }
		platform = null;
		if (On_PlayerRotationPowersEnd != null)
		{
			On_PlayerRotationPowersEnd();
		}
	}

	/*void On_JoystickMoveStart (MovingJoystick move)
	{
		shouldRotate = false;
	}
	
	void On_JoystickMoveEnd (MovingJoystick move)
	{
		shouldRotate = true;
	}*/
}
