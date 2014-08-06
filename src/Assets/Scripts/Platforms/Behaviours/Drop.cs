using UnityEngine;
using System.Collections;

public class Drop : PlatformBehaviour 
{
	private bool _isDropping;
	private bool _atOriginalPosition = true;
	private float _originalYPosition;

	public float maxY = 0.0f;
	public float minY = 0.0f;
	public float returnSpeed = 0.1f;

	// Use this for initialization
	void Start () 
	{
		_originalYPosition = transform.position.y;
		maxY = transform.parent.position.y + (transform.parent.localScale.y / 2);
		minY = transform.parent.position.y - (transform.parent.localScale.y / 2);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		_atOriginalPosition = transform.position.y == _originalYPosition;
		
		if (_isDropping && transform.position.y >= minY) 
		{
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
		}
		else
		{
			rigidbody.isKinematic = true;
			rigidbody.useGravity = false;
		}

		if (!_atOriginalPosition && !_isDropping) 
		{
			transform.position = Vector3.Lerp(transform.position, new Vector3 (transform.position.x, _originalYPosition, transform.position.z), returnSpeed);
		}
	}

	public override void HandleOnPlatformReached (Transform platform, Transform player)
	{
		if (platform.GetInstanceID() == this.transform.GetInstanceID())
		{
			_isDropping = true;
		    isOnPlatform = true;
		}
	}

	public override void HandlePlayerAirborne()
	{
		_isDropping = false;
	    isOnPlatform = false;
	}
}
