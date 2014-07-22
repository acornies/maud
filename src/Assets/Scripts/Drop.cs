using UnityEngine;
using System.Collections;

public class Drop : MonoBehaviour 
{

	public float dropSpeed = 10.0f;
	public bool isDropping;
	public bool atOriginalPosition = true;
	public float maxY = 0.0f;
	public float minY = 0.0f;

	private float _originalYPosition;

	// Subscribe to events
	void OnEnable()
	{
		PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
		PlayerMovement.On_PlatformExit += HandleOnPlatformExit;
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
		PlayerMovement.On_PlatformExit -= HandleOnPlatformExit;
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
			atOriginalPosition = true;
		}
		else
		{
			atOriginalPosition = false;
		}

		if (isDropping && transform.position.y >= minY) 
		{
			transform.Translate(Vector3.down * dropSpeed * Time.deltaTime);
		}

		if (!atOriginalPosition && !isDropping) 
		{
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, _originalYPosition, transform.position.z), 1.0f);
		}
	}

	void HandleOnPlatformReached (Transform platform)
	{
		if (platform.GetInstanceID() == this.transform.GetInstanceID())
		{
			//Debug.Log("drop platform");
			//transform.position -= dropSpeed * Time.deltaTime;
			//if (transform.position.y >= minY)
			//{
			isDropping = true;
			//}
			//atOriginalPosition = false;
		}
	}

	void HandleOnPlatformExit(Transform platform)
	{
		//transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, _originalYPosition, transform.position.z), 0.3f);
		if (platform.GetInstanceID () == this.transform.GetInstanceID ()) 
		{
			//Debug.Log("return platform");
			isDropping = false;
			//atOriginalPosition = false;
		}
	}
}
