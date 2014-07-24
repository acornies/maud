using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BoundaryController : MonoBehaviour 
{
	public Transform player;
	public float boundaryForce = 300f;

	void Awake()
	{
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
	}

	void OnCollisionEnter(Collision collision) 
	{
		// left and right boundary behaviour
		if (collision.transform == player)
		{
			BoundaryBounceBack();
		}
	}

	void BoundaryBounceBack()
	{
		player.rigidbody.velocity = new Vector2(0, player.rigidbody.velocity.y);
		var dir = (player.position.x - transform.position.x);
		//Debug.Log (dir);
		player.rigidbody.AddForce(dir * boundaryForce, 0, 0);
		player.GetComponent<PlayerMovement>().forcePushed = true;
	}
}
