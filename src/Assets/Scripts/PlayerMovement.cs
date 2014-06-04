using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	private Transform _tower;

	public float MaxSpeed = 6.0f;
	public bool FacingRight = true;
	public float MoveDirection;

	public float JumpSpeed = 600.0f;
	public bool Grounded = false;
	public float rotationSpeed = 10.0f;
	public bool shouldRotate = true;
	
	void Awake()
	{
		//GroundCheck = GameObject.Find("GroundCheck").transform;
		_tower = GameObject.Find("Tower").transform;
	}
	
	// Use this for physics updates
	void FixedUpdate ()
	{
		this.shouldRotate = true;
		rigidbody.velocity = new Vector2(this.MoveDirection * this.MaxSpeed, rigidbody.velocity.y);

		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.right, out hit, 0.1f)
		    || Physics.Raycast(transform.position, Vector3.left, out hit, 0.1f)) {
			Debug.Log("Hit " + hit.transform.gameObject.name);
			if (hit.transform.tag == "Stoppable"){
				this.shouldRotate = false;
				rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
			}
		}

		Debug.DrawRay(transform.position, Vector3.right, Color.white);
		Debug.DrawRay(transform.position, Vector3.left, Color.white);
		
		if (this.MoveDirection > 0.0f && !this.FacingRight)
		{
			Flip();
		}
		else if (this.MoveDirection < 0.0f && this.FacingRight)
		{
			Flip();
		}
	}

	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.layer == 8)
		{
			this.Grounded = true;
		}
	}
	
	void OnCollisionStay(Collision collisionInfo) 
	{
		if (collisionInfo.gameObject.layer == 8){
			this.Grounded = true;
		}
	}
	
	void OnCollisionExit(Collision collisionInfo) 
	{
		if (collisionInfo.gameObject.layer == 8){
			this.Grounded = false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		this.MoveDirection = Input.GetAxis("Horizontal");
		
		if (this.Grounded && Input.GetButtonDown("Jump"))
		{
			rigidbody.AddForce(new Vector2(0, JumpSpeed));
		}

		if (Input.GetKey(KeyCode.RightArrow) && shouldRotate) {
			_tower.Rotate(Vector3.up * this.rotationSpeed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.LeftArrow) && shouldRotate) {
			_tower.Rotate(-Vector3.up * this.rotationSpeed * Time.deltaTime);
		}

		//Debug.DrawRay(transform.position, Vector3.right, Color.white);

		//Debug.Log(multiplier);
	}
	
	void Flip()
	{
		this.FacingRight = !FacingRight;
		transform.Rotate(Vector3.up, 180.0f, Space.World);
	}
}
