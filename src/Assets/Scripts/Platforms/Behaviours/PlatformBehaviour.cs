using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{

    //protected Quaternion? rotationTarget;
    protected Transform child;

    //public float rotationSpeed = 1;
    public bool isOnPlatform;
    public bool isStopped;

    // Subscribe to events
    public virtual void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;
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
    }

    protected virtual void Start()
    {
        child = transform.Find("Cube");
    }

    protected virtual void FixedUpdate()
    {
        //RotateToTarget();
    }

    public virtual void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform != null && child != null)
        {
            isOnPlatform = platform.GetInstanceID() == child.GetInstanceID();
        }
    }

    public virtual void HandlePlayerAirborne()
    {
        isOnPlatform = false;
    }
}
