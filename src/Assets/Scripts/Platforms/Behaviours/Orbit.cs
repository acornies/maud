using UnityEngine;
using System.Collections;

public class Orbit : PlatformBehaviour 
{
    private float _stoppedTimer = 0.5f;
    private bool _isStopped;
    
    public Transform center;
	public Vector3 axis = Vector3.up;
	public float radius = 2.0f;
	public float radiusSpeed = 0.5f;
	public float rotationSpeed = 10.0f;
    public float artificalForce = 1000f;
    public float stopTime = 1.0f;

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
	    //_isStopped = isOnPlatform;
 
        if (_isStopped && !isOnPlatform)
	    {
            _stoppedTimer -= Time.deltaTime;
            if (!(_stoppedTimer <= 0)) return;
	        _isStopped = false;
	    }

        if (isOnPlatform)
        {
            _isStopped = true;
        }
	    else
	    {
            _stoppedTimer = stopTime;
            transform.RotateAround(center.position, axis, rotationSpeed);
            var desiredPosition = (transform.position - center.position).normalized * radius + center.position;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed);
	    }
	}

    void OnCollisionEnter(Collision collision)
    {
        HandlePlayerCollisions(collision);
    }
    /*void OnCollisionStay(Collision collision)
    {
        HandlePlayerCollisions(collision);
    }*/

    void HandlePlayerCollisions(Collision collision)
    {
        if (collision.gameObject.name != "Player" || isOnPlatform || _playerCollidingWithHead) return;

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
        _isStopped = true;
    }
}
