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
	//public float rotationSpeed = 10.0f;
	//public bool shouldRotate = true;
	public int boundaryForce = 100;

	public LayerMask whatIsGround;
	
	void Awake()
	{
		_groundCheck = GameObject.Find("GroundCheck").transform;
		//_tower = GameObject.Find("Tower").transform;
	}
	
	// Use this for physics updates
	void FixedUpdate ()
	{

		Move();

		// draw ray near the head of the player
		Vector3 headRay = new Vector3(transform.position.x, transform.position.y + 0.35f, transform.position.z);
		/*Debug.DrawRay(headRay, Vector3.right, Color.white);
		Debug.DrawRay(headRay, Vector3.left, Color.white);
		Debug.DrawRay(transform.position, Vector3.right, Color.white);
		Debug.DrawRay(transform.position, Vector3.left, Color.white);*/

		// stop player from sticking to colliders in mid-air
		RaycastHit hit;
		if (Physics.Raycast(headRay, Vector3.right, out hit, 0.3f)
		    || Physics.Raycast(transform.position, Vector3.right, out hit, 0.3f)
		    || Physics.Raycast(headRay, Vector3.left, out hit, 0.3f)
		    || Physics.Raycast(transform.position, Vector3.left, out hit, 0.3f)) 
		{
			// the "walkable" layer
			if (hit.transform.gameObject.layer == 8)
			{
				//this.shouldRotate = false;
				rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
			}
		}

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
		this.moveDirection = Input.GetAxis("Horizontal");
		
		if (Input.GetButtonDown("Jump"))
		{
			Jump();
		}
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
			boundaryForce, 
			0);
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
