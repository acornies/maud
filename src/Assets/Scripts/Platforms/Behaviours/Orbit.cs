using UnityEngine;
using System.Collections;

public class Orbit : PlatformBehaviour 
{
	public Transform center;
	public Vector3 axis = Vector3.up;
	public float radius = 2.0f;
	public float radiusSpeed = 0.5f;
	public float rotationSpeed = 10.0f;
    public float artificalForce = 1000f;
    public float pushedTimer = 0.5f;

    // Use this for initialization
	void Start () 
	{
		transform.position = (transform.position - center.position).normalized * radius + center.position;
		center = transform.parent;
	}

    void Update()
    {
        
    }
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.RotateAround (center.position, axis, rotationSpeed);
		var desiredPosition = (transform.position - center.position).normalized * radius + center.position;
		transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed);
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Player" || isOnPlatform) return;
        
        if (axis.y > 0)
        {
            collision.rigidbody.AddForce(-1 * (artificalForce * rotationSpeed), collision.transform.position.y, collision.transform.position.z);
        }
        else
        {
            collision.rigidbody.AddForce(1 * (artificalForce * rotationSpeed), collision.transform.position.y, collision.transform.position.z);
        }

        //var timer = pushedTimer;
        collision.transform.GetComponent<PlayerMovement>().forcePushed = true;
    }
}
