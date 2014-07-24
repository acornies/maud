using UnityEngine;
using System;
using System.Linq;
using System.Collections;
//using TouchScript.Gestures;

public class PlayerMovement : MonoBehaviour 
{
	//private Transform _tower;
	private Transform _groundCheck;
	private int _additionalJumpCount;
	public bool isLongJumping;

	public bool canMove = false;
	public float maxSpeed = 6.0f;
	public bool facingRight = true;
	public float moveDirection;
	public float jumpForce = 600.0f;
	public float extraJumpForce = 200.0f;
	public bool isGrounded = false;
	public bool forcePushed = false;
	public int boundaryForce = 100;
	public float stickyBuffer = 0.4f;
	public LayerMask whatIsGround;
	public float groundedRadius = 0.15f;
	public int additionalJumps = 1;

	public delegate void ReachedPlatformAction(Transform platform);
	public static event ReachedPlatformAction On_PlatformReached;

	public delegate void PlayerAirborne();
	public static event PlayerAirborne On_PlayerAirborne;

	// Subscribe to events
	void OnEnable()
	{
		EasyJoystick.On_JoystickTap += On_JoystickTap;
		EasyJoystick.On_JoystickMove += On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMove;
		EasyTouch.On_LongTapEnd += HandleLongTap;
	}

	void HandleLongTap (Gesture gesture)
	{
		Jump(extraJumpForce);
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
		EasyJoystick.On_JoystickTap -= On_JoystickTap;
		EasyJoystick.On_JoystickMove -= On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMove;
		EasyTouch.On_LongTapEnd -= HandleLongTap;
	}
	
	void Awake()
	{
		_groundCheck = GameObject.Find("GroundCheck").transform;
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
			var collider = groundColliders.FirstOrDefault();
			if (collider != null && isGrounded)
			{
				_additionalJumpCount = 0;
				forcePushed = false;
				On_PlatformReached(collider.transform); // trigger event for finding current platform
			}
		}

		if (!isGrounded)
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
	
	// Update is called once per frame
	void Update ()
	{
	}

	// TODO: move to specific platform script and get player through collision
	void OnTriggerEnter(Collider collider)
	{
		// handle orbit object force
		if (collider.gameObject.name == "Sphere") {
			var dir = (transform.position.x - collider.transform.position.x);
			rigidbody.AddForce(dir * collider.gameObject.GetComponent<Orbit>().artificialForce, 0, 0);
			forcePushed = true;
		}
	}

	void On_JoystickMove(MovingJoystick movingStick)
	{
		moveDirection = movingStick.joystickAxis.x;
	}

	void On_JoystickTap(MovingJoystick movingStick)
	{
		Jump();
	}

	void HandleStickyPhysics()
	{
		// draw ray near the head of the player
		Vector3 headRay = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
		Vector3 midRay = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		Vector3 footRay = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
		/*Debug.DrawRay(midRay, Vector3.right, Color.white);
		Debug.DrawRay(midRay, Vector3.left, Color.white);
		Debug.DrawRay(midRay, new Vector3(1, 0, 1), Color.green);
		Debug.DrawRay(midRay, new Vector3(-1, 0, 1), Color.green);
		Debug.DrawRay(footRay, Vector3.right, Color.white);
		Debug.DrawRay(footRay, Vector3.left, Color.white);
		Debug.DrawRay(headRay, Vector3.right, Color.white);
		Debug.DrawRay(headRay, Vector3.left, Color.white);*/
		
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
			//Debug.Log("Stop movement 1!!!");
			// the "walkable" layer
			if (hit.transform.gameObject.layer == 8)
			{
				canMove = false;
			}
			else 
			{
				canMove = true;
			}
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
			rigidbody.velocity = (canMove) 
				? new Vector2(this.moveDirection * this.maxSpeed, rigidbody.velocity.y) 
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
		if (this.isGrounded) 
		{
			rigidbody.AddForceAtPosition(new Vector3(0, jumpForce + extraForce, 0), transform.position);
			if (extraForce > 0)
			{
				isLongJumping = true;
			}
			else
			{
				isLongJumping = false;
			}
		}

		// conditions for mid-air jump
		if (!isGrounded 
		    && _additionalJumpCount < additionalJumps 
		    && !isLongJumping
		    && !forcePushed)
		{
			rigidbody.AddForceAtPosition(new Vector3(0, extraJumpForce, 0), transform.position);
			_additionalJumpCount++;
		}

	}
}
