using UnityEngine;
using System.Collections;

public class Disappear : MonoBehaviour 
{
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;

	public float timer = 0.0f;
	public float interval = 3.0f;
	public bool isInvisible;
	public bool isOnPlatform;

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
		var child = transform.Find ("Cube");
		_initialPosition = transform.Find("Cube").localPosition;
		_initialRotation = child.localRotation;
		//_childCopy =  (GameObject)Object.Instantiate(transform.GetChild (0).gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;

		if (isInvisible)
		{
			if (timer >= interval)
			{
				GameObject newPlatform = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/PrototypeCube"), 
				                                       _initialPosition, Quaternion.identity);
				newPlatform.name = "Cube";
				newPlatform.transform.parent = transform;
				newPlatform.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				newPlatform.transform.localPosition = _initialPosition;
				newPlatform.transform.localRotation = _initialRotation;
				timer = 0;
				isInvisible = false;
			}
		}
		else
		{
			if (timer >= interval)
			{
				GameObject platformToDestroy = transform.Find("Cube").gameObject;
				Destroy(platformToDestroy);
				timer = 0;
				isInvisible = true;
			}
		}
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
