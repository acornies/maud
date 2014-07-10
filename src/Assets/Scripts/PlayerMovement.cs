using UnityEngine;
using System;
using System.Collections;
//using TouchScript.Gestures;

public class PlayerMovement : MonoBehaviour {

	//private Transform _tower;
	private Transform _groundCheck;

	public bool canMove = false;
	public float maxSpeed = 6.0f;
	public bool facingRight = true;
	public float moveDirection;
	public float jumpSpeed = 600.0f;
	public bool grounded = false;
	public bool forcePushed = false;
	public int boundaryForce = 100;
	public float stickyBuffer = 0.4f;
	public LayerMask whatIsGround;

	// Subscribe to events
	void OnEnable(){
		EasyJoystick.On_JoystickTap += On_JoystickTap;
		EasyJoystick.On_JoystickMove += On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd += On_JoystickMove;
		//EasyJoystick.On_JoystickTouchUp += On_JoystickTap;
	}
	
	void OnDisable(){
		UnsubscribeEvent();
	}
	
	void OnDestroy(){
		UnsubscribeEvent();
	}
	
	void UnsubscribeEvent(){
		EasyJoystick.On_JoystickTap -= On_JoystickTap;
		EasyJoystick.On_JoystickMove -= On_JoystickMove;
		EasyJoystick.On_JoystickMoveEnd -= On_JoystickMove;
		//EasyJoystick.On_JoystickTouchUp -= On_JoystickTap;
	}
	
	void Awake()
	{
		_groundCheck = GameObject.Find("GroundCheck").transform;
		//_tower = GameObject.Find("Tower").transform;
	}
	
	// Use this for physics updates
	void FixedUpdate ()
	{
		HandleStickPhysics();
		Move();

		// handle jumping
		var groundColliders = Physics.OverlapSphere(_groundCheck.position, 0.20f, whatIsGround);
		if (groundColliders != null){
			this.grounded = groundColliders.Length > 0 ? true : false;
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
		//this.moveDirection = Input.GetAxis("Horizontal");
		
		/*if (Input.GetButtonDown("Jump"))
		{
			Jump();
		}*/
	}

	void OnCollisionEnter(Collision collision) 
	{
		// left and right boundary behaviour
		if (collision.transform.name == "LeftBoundary" && !grounded)
		{
			BounceBack("left");
		}
		if (collision.transform.name == "RightBoundary" && !grounded)
		{
			BounceBack("right");
		}

		// restore control when bounce back is finished
		if (collision.transform.gameObject.layer == 8)
		{
			forcePushed = false;
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		// handle orbit object force
		if (collider.gameObject.name == "Sphere") {
			var dir = (transform.position.x - collider.transform.position.x);
			//if (!forcePushed) {
				rigidbody.AddForce(dir * collider.gameObject.GetComponent<Orbit>().artificialForce, 0, 0);
				forcePushed = true;
			//}
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

	void HandleStickPhysics()
	{
		// draw ray near the head of the player
		Vector3 headRay = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
		Vector3 midRay = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		Vector3 footRay = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
		Debug.DrawRay(midRay, Vector3.right, Color.white);
		Debug.DrawRay(midRay, Vector3.left, Color.white);
		Debug.DrawRay(footRay, Vector3.right, Color.white);
		Debug.DrawRay(footRay, Vector3.left, Color.white);
		Debug.DrawRay(headRay, Vector3.right, Color.white);
		Debug.DrawRay(headRay, Vector3.left, Color.white);
		
		// stop player from sticking to colliders in mid-air
		RaycastHit hit;
		if (Physics.Raycast(midRay, Vector3.right, out hit, stickyBuffer)
		    || Physics.Raycast(footRay, Vector3.right, out hit, stickyBuffer)
		    || Physics.Raycast(midRay, Vector3.left, out hit, stickyBuffer)
		    || Physics.Raycast(footRay, Vector3.left, out hit, stickyBuffer)
		    || Physics.Raycast(headRay, Vector3.right, out hit, stickyBuffer)
		    || Physics.Raycast(headRay, Vector3.left, out hit, stickyBuffer)) 
		{
			//Debug.Log("Stop movement 1!!!");
			// the "walkable" layer
			if (hit.transform.gameObject.layer == 8)
			{
				//Debug.Log("Stop movement 2!!!");
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

	void BounceBack(string direction)
	{
		rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
		rigidbody.AddForce(
			(direction == "right") ? -boundaryForce : boundaryForce, 
			boundaryForce, 0);
		forcePushed = true;
	}
	
	void Flip()
	{
		this.facingRight = !facingRight;
		transform.Rotate(Vector3.up, 180.0f, Space.World);
	}

	public void Jump()
	{
		if (this.grounded) 
		{
			rigidbody.AddForce(new Vector2(0, jumpSpeed));
		}

	}
}
