using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour 
{
    public bool isOnPlatform;

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
}
