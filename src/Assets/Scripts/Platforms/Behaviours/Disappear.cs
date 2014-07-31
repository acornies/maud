using UnityEngine;
using System.Collections;

public class Disappear : PlatformBehaviour 
{
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;

	public float timer = 0.0f;
	public float interval = 3.0f;
	public bool isInvisible;

	// Use this for initialization
	void Start () 
	{
		var child = transform.Find ("Cube");
		_initialPosition = transform.Find("Cube").localPosition;
		_initialRotation = child.localRotation;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		timer += Time.deltaTime;

		if (isInvisible)
		{
		    if (!(timer >= interval)) return;
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
		else
		{
		    if (!(timer >= interval)) return;
		    Transform platformToDestroy = transform.Find("Cube");
		    if (platformToDestroy == null) return;
		    Destroy(platformToDestroy.gameObject);
		    timer = 0;
		    isInvisible = true;
		}
	}

    public override void HandleOnPlatformReached(Transform platform)
    {
        // since this script is on the parent gameobject, compare platform id with child id
        var child = transform.Find("Cube");
        if (platform != null && child != null)
        {
            isOnPlatform = platform.GetInstanceID() == child.GetInstanceID();
        }
    }
}
