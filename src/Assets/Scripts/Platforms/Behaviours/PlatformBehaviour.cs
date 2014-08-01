using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour 
{
    protected bool _playerCollidingWithHead;
    
    public bool isOnPlatform;

    // Subscribe to events
    public virtual void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;
        PlayerMovement.On_HitHead += HandleHitHead;
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
        PlayerMovement.On_HitHead -= HandleHitHead;
    }

    public virtual void HandleOnPlatformReached(Transform platform)
    {
        if (platform.GetInstanceID() == this.transform.GetInstanceID())
        {
            isOnPlatform = true;
        }
    }

    public virtual void HandlePlayerAirborne()
    {
        isOnPlatform = false;
    }

    public virtual void HandleHitHead(Transform platform)
    {
        if (platform.GetInstanceID() == this.transform.GetInstanceID())
        {
            //Debug.Log("Player hit head");
            _playerCollidingWithHead = true;
        }
    }
}
