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
	protected override void Start () 
	{
        base.Start();
		_initialPosition = child.localPosition;
		_initialRotation = child.localRotation;
	}
	
	// Update is called once per frame
    protected override void FixedUpdate() 
	{
        base.FixedUpdate();
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
		    Transform platformToDestroy = child;
		    if (platformToDestroy == null) return;
		    Destroy(platformToDestroy.gameObject);
		    timer = 0;
		    isInvisible = true;
		}
	}

    /*public override void HandleOnPlatformReached(Transform platform, Transform player)
    {
        // since this script is on the parent gameobject, compare platform id with child id
        if (platform != null && child != null)
        {
            isOnPlatform = platform.GetInstanceID() == child.GetInstanceID();
        }
    }*/
}
