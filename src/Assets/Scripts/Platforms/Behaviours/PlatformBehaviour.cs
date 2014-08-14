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
        //TelekinesisController.On_NewPlatformRotation += HandleNewPlatformRotation;
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
        //TelekinesisController.On_NewPlatformRotation -= HandleNewPlatformRotation;
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

    /*protected void HandleNewPlatformRotation(Transform platform, Quaternion rotation)
    {
        if (platform.GetInstanceID() == this.transform.GetInstanceID())
        {
            Debug.Log("new rotation: "+ rotation +" for " + platform.name);
            rotationTarget = rotation;
        }
    }

    protected virtual void RotateToTarget()
    {
        if (rotationTarget == null) return;
        if (transform.root.localRotation != rotationTarget)
        {
            transform.root.rotation = Quaternion.Lerp(transform.root.localRotation, rotationTarget.Value, rotationSpeed * Time.deltaTime);
        }
        if (transform.root.localRotation == rotationTarget)
        {
            rotationTarget = null;
        }
    }*/
}
