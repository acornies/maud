using UnityEngine;
using System.Collections;

public class Disappear : MonoBehaviour 
{

	public float timer = 0.0f;
	public float interval = 3.0f;
	public bool isInvisible;
	public bool isOnPlatform;
	public Vector3 position;

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
		position = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;

		if (isInvisible)
		{
			if (timer >= interval)
			{
				//this.enabled = true;
				renderer.enabled = true;
				//rigidbody.useGravity = false;
				//rigidbody.isKinematic = true;
				timer = 0;
			}
		}
		else
		{
			//var timer = visibleTimer;
			//timer += Time.deltaTime;
			
			if (timer >= interval)
			{
				renderer.enabled = false;
				//rigidbody. = true;
				//rigidbody.isKinematic = false;
				//transform.position = position;
				timer = 0;
			}
		}

		isInvisible = !renderer.enabled;
	}

	void HandleOnPlatformReached (Transform platform)
	{
		if (platform.GetInstanceID() == this.transform.GetInstanceID())
		{
			isOnPlatform = true;
		}
	}

	void HandlePlayerAirborne ()
	{
		isOnPlatform = false;
	}
	
}
