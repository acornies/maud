using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	//private Transform _tower;
	private Transform _groundCheck;
	private int _additionalJumpCount;
	private bool _isLongJumping;

	public bool canMove;
	public bool isRotating;
	public float maxSpeed = 6.0f;
	public bool facingRight = true;
	public float moveDirection;
	public float jumpForce = 900.0f;
	//public float longJumpForce = 300.0f;
	public bool isGrounded;
	public bool forcePushed;
	public float stickyBuffer = 0.4f;
	public LayerMask whatIsGround;
	public float groundedRadius = 0.15f;
	public int additionalJumps = 1;
	public float additionalJumpForce = 500.0f;

	public delegate void ReachedPlatformAction(Transform platform);
	public static event ReachedPlatformAction On_PlatformReached;

	public delegate void PlayerAirborne();
	public static event PlayerAirborne On_PlayerAirborne;

	// Subscribe to events
	void OnEnable()
	{
		/*EasyJoystick.On_JoystickTap += On_JoystickTap;
		EasyJoystick.On_JoystickMove += On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMove;*/
		EasyTouch.On_DoubleTap += HandleDoubleTap;
		EasyTouch.On_Swipe += HandleSwipe;
		EasyTouch.On_SwipeEnd += HandleSwipeEnd;
		EasyTouch.On_SimpleTap += HandleSimpleTap;
		RotationControl.On_PlayerRotationPowersStart += HandlePlayerRotationPowersStart;
		RotationControl.On_PlayerRotationPowersEnd += HandlePlayerRotationPowersEnd;
	}

	void HandlePlayerRotationPowersEnd ()
	{
		isRotating = false;
	}

	void HandlePlayerRotationPowersStart ()
	{
		isRotating = true;
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
		/*EasyJoystick.On_JoystickTap -= On_JoystickTap;
		EasyJoystick.On_JoystickMove -= On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMove;*/
		EasyTouch.On_DoubleTap -= HandleDoubleTap;
		EasyTouch.On_Swipe -= HandleSwipe;
		EasyTouch.On_SwipeEnd -= HandleSwipeEnd;
		EasyTouch.On_SimpleTap -= HandleSimpleTap;
		RotationControl.On_PlayerRotationPowersStart -= HandlePlayerRotationPowersStart;
		RotationControl.On_PlayerRotationPowersEnd -= HandlePlayerRotationPowersEnd;
	}
	
	void Awake()
	{
		_groundCheck = GameObject.Find("GroundCheck").transform;
	}

	void Start()
	{
	}
	
	// Use this for physics updates
	void FixedUpdate ()
	{
		HandleStickyPhysics();
		Move();

		// handle jumping
		var groundColliders = Physics.OverlapSphere(_groundCheck.position, groundedRadius, whatIsGround);
		if (groundColliders != null)
		{
			isGrounded = groundColliders.Length > 0 ? true : false;
			var groundCollider = groundColliders.FirstOrDefault();
			if (groundCollider != null && isGrounded)
			{
				_additionalJumpCount = 0;
				forcePushed = false;
				On_PlatformReached(groundCollider.transform); // trigger event for finding current platform
			}
		}

		if (!isGrounded && On_PlayerAirborne != null)
		{
			On_PlayerAirborne();
		}
		
		// flip player on the y axis
		if (this.moveDirection > 0.0f && !this.facingRight)
		{
			Flip();
		}
		else if (this.moveDirection < 0.0f && this.facingRight)
		{
			Flip();
		}
	}

	void HandleSimpleTap (Gesture gesture)
	{
		Jump();
	}
	
	void HandleSwipeEnd (Gesture gesture)
	{
		moveDirection = 0;
	}
	
	void HandleSwipe (Gesture gesture)
	{
	    switch (gesture.swipe)
	    {
	        case EasyTouch.SwipeType.Left:
	            facingRight = false;
	            moveDirection = -1;
	            break;
            case EasyTouch.SwipeType.Right:
	            facingRight = true;
	            moveDirection = 1;
	            break;
	    }
	}

    void HandleDoubleTap (Gesture gesture)
	{
		Jump();
	}

    /*
	void On_JoystickMove(MovingJoystick movingStick)
	{
		moveDirection = movingStick.joystickAxis.x;
	}

	void On_JoystickTap(MovingJoystick movingStick)
	{
		Jump();
	}*/

	void HandleStickyPhysics()
	{
		// draw ray near the head of the player
		Vector3 headRay = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
		Vector3 midRay = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		Vector3 footRay = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
		Debug.DrawRay(midRay, Vector3.right, Color.white);
		Debug.DrawRay(midRay, Vector3.left, Color.white);
		Debug.DrawRay(midRay, new Vector3(1, 0, 1), Color.green);
		Debug.DrawRay(midRay, new Vector3(-1, 0, 1), Color.green);
		Debug.DrawRay(footRay, Vector3.right, Color.white);
		Debug.DrawRay(footRay, Vector3.left, Color.white);
		Debug.DrawRay(headRay, Vector3.right, Color.white);
		Debug.DrawRay(headRay, Vector3.left, Color.white);
		
		// stop player from sticking to colliders in mid-air
		RaycastHit hit;
		if (
		    Physics.Raycast(footRay, Vector3.right, out hit, stickyBuffer)
			|| Physics.Raycast(footRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
			|| Physics.Raycast(footRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
			|| Physics.Raycast(footRay, Vector3.left, out hit, stickyBuffer)
		    || Physics.Raycast(midRay, Vector3.right, out hit, stickyBuffer)
		    || Physics.Raycast(midRay, Vector3.left, out hit, stickyBuffer)
			|| Physics.Raycast(midRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
			|| Physics.Raycast(midRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
		    || Physics.Raycast(headRay, Vector3.right, out hit, stickyBuffer)
		    || Physics.Raycast(headRay, Vector3.left, out hit, stickyBuffer)
			|| Physics.Raycast(headRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
			|| Physics.Raycast(headRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
			) 
		{
			// the "walkable" layer
			canMove = hit.transform.gameObject.layer != 8;
		}
		else
		{
			canMove = true;
		}
	}

	private void Move()
	{
		// move player
		if (!forcePushed) 
		{
			//this.shouldRotate = true;
			rigidbody.velocity = (canMove && !isRotating) 
				? new Vector2(this.moveDirection * maxSpeed, rigidbody.velocity.y) 
					: new Vector2(0, rigidbody.velocity.y);
		}
	}
	
	void Flip()
	{
		this.facingRight = !facingRight;
		transform.Rotate(Vector3.up, 180.0f, Space.World);
	}

	public void Jump(float extraForce = 0)
	{
	    if (!canMove) return;
	    if (this.isGrounded)
	    {
	        rigidbody.AddForceAtPosition(new Vector3(0, jumpForce + extraForce, 0), transform.position);
	        _isLongJumping = extraForce > 0;
	    }

	    // conditions for mid-air jump
	    if (isGrounded || _additionalJumpCount >= additionalJumps || _isLongJumping || forcePushed) return;
	    rigidbody.AddForceAtPosition(new Vector3(0, additionalJumpForce, 0), transform.position);
	    _additionalJumpCount++;
	}
}
