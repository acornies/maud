using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour 
{
	private bool _isDropping;
	private bool _atOriginalPosition = true;
	private float _originalYPosition;

	public float dropSpeed = 10.0f;
	public float maxY = 0.0f;
	public float minY = 0.0f;
	public float returnSpeed = 0.1f;

	// Subscribe to events
	void OnEnable()
	{
		PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
		PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;
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
		PlayerMovement.On_PlatformReached -= HandleOnPlatformReached;
		PlayerMovement.On_PlayerAirborne -= HandlePlayerAirborne;
	}

	// Use this for initialization
	void Start () 
	{
		_originalYPosition = transform.position.y;
		maxY = transform.parent.position.y + (transform.parent.localScale.y / 2);
		minY = transform.parent.position.y - (transform.parent.localScale.y / 2);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (transform.position.y == _originalYPosition)
		{
			_atOriginalPosition = true;
		}
		else
		{
			_atOriginalPosition = false;
		}
		
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

	void HandleOnPlatformReached (Transform platform)
	{
		if (platform.GetInstanceID() == this.transform.GetInstanceID())
		{
			_isDropping = true;
		}
	}

	void HandlePlayerAirborne()
	{
		_isDropping = false;
	}
}
