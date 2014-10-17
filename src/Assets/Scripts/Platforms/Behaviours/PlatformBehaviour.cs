using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{

    //protected Quaternion? rotationTarget;
    protected Transform child;
    protected bool isBeingAffected;

    //public float rotationSpeed = 1;
    public bool isOnPlatform;
    public bool isStopped;

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
    }

    protected virtual void FixedUpdate()
    {
        //RotateToTarget();
    }

    public virtual void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform == null || child == null) return;
        isOnPlatform = platform.GetInstanceID() == child.GetInstanceID();

        if (isOnPlatform && isBeingAffected)
        {
            player.parent = null;
        }
        else if (isOnPlatform && !isBeingAffected)
        {
            player.parent = child;
        }
    }

    public virtual void HandlePlayerAirborne(Transform player)
    {
        isOnPlatform = false;
        player.parent = null;
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
