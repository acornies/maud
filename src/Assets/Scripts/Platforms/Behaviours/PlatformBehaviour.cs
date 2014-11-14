using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
	protected bool shouldBurnOut;
    
	//protected Quaternion? rotationTarget;
    protected Transform child;
    protected bool isBeingAffected;
	protected Light innerLight;
    //protected InAndOut inAndOutScript;

    //public float rotationSpeed = 1;
    public bool isOnPlatform;
    public bool isStopped;
	public float lightIntensity = 2.75f;
	public float lightBurnoutSpeed = 1f;

    // Subscribe to events
    public virtual void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;
        TelekinesisHandler.OnAffectStart += HandleOnAffectStart;
        TelekinesisHandler.OnAffectEnd += HandleOnAffectEnd;
    }

    public virtual void OnDisable()
    {
        UnsubscribeEvent();
    }

    public virtual void OnDestroy()
    {
        UnsubscribeEvent();
    }

    public virtual void UnsubscribeEvent()
    {
        PlayerMovement.On_PlatformReached -= HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne -= HandlePlayerAirborne;
        TelekinesisHandler.OnAffectStart -= HandleOnAffectStart;
        TelekinesisHandler.OnAffectEnd -= HandleOnAffectEnd;
    }

    protected virtual void Start()
    {
        child = transform.Find("Cube");
		innerLight = GetComponentInChildren<Light> ();
        //inAndOutScript = transform.GetComponent<InAndOut>();
    }

	protected virtual void Update()
	{
		if (shouldBurnOut)
		{
			innerLight.intensity = Mathf.Lerp(innerLight.intensity, 0, lightBurnoutSpeed * Time.deltaTime);

			if (innerLight.intensity == 0)
			{
				innerLight.enabled = false;
			}
		}
	}

    protected virtual void FixedUpdate()
    {
        //RotateToTarget();
    }

    public virtual void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform == null || child == null) return;
        //isOnPlatform = platform.GetInstanceID() == child.GetInstanceID();

		if (platform.GetInstanceID() != child.GetInstanceID()) return;
		isOnPlatform = true;
        //if (inAndOutScript != null) return;

        if (isOnPlatform && isBeingAffected)
        {
            player.parent = null;
        }
        else if (isOnPlatform && !isBeingAffected)
        {
            player.parent = child;
        }

		if (innerLight != null && innerLight.intensity == 0)
		{
			innerLight.intensity = lightIntensity;
			shouldBurnOut = true;
			//Debug.Log("turn on");
		}
    }

    public virtual void HandlePlayerAirborne(Transform player)
    {
        isOnPlatform = false;
        //player.parent = null;
    }

    public virtual void HandleOnAffectStart(Transform platform)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
        isBeingAffected = true;
    }

    public virtual void HandleOnAffectEnd(Transform platform)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
        isBeingAffected = false;
    }

}
