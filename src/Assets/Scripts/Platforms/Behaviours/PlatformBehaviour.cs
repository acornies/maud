using System.Linq;
using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
   	/*private GameObject _objectToDestroy;
	private float _destroyTimer;
    private float _postDestroyTimeout = 3f;*/

    //protected bool shouldBurnOut;
    //protected bool shouldDestroy;

    //protected Quaternion? rotationTarget;
    protected Transform child;
	//public GameObject flashEffect;
    protected bool isBeingAffected;
    //protected Light innerLight;
    //protected InAndOut inAndOutScript;
    protected Color towerColor;

    //public float rotationSpeed = 1;
    public bool isOnPlatform;
    public bool isStopped;
	//public float colorChangeSpeed = 10f;
    //public float destroyTransitionSpeed = 10f;
	//public float destroyTime = 2f;
    //public float lightIntensity = 2.75f;
    //public float lightBurnoutSpeed = 1f;

    // Subscribe to events
    public virtual void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;
        TelekinesisHandler.OnAffectStart += HandleOnAffectStart;
        TelekinesisHandler.OnAffectEnd += HandleOnAffectEnd;
        //PlatformController.OnTimedDestroy += HandleOnTimedDestroy;
    }

    /*
	void HandleOnTimedDestroy(GameObject objectToDestroy)
    {
        if (objectToDestroy.GetInstanceID() != gameObject.GetInstanceID()) return;

        _objectToDestroy = objectToDestroy;
        shouldDestroy = true;
        transform.tag = "Untagged";
		_destroyTimer = destroyTime;
    }*/

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
        //PlatformController.OnTimedDestroy -= HandleOnTimedDestroy;
    }

    protected virtual void Start()
    {
        child = transform.FindChild("Cube");

        if (renderer != null)
        {
            towerColor = renderer.material.color;
        }
    }

    protected virtual void Update()
    {

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

        /*if (isOnPlatform && isBeingAffected)
        {
            player.parent = null;
        }
        else if (isOnPlatform && !isBeingAffected)
        {
            player.parent = child;
        }*/

        if (GameController.Instance.inSafeZone && PlatformController.Instance.GetCurrentPlatformNumber() >= 11)
        {
            //Debug.Log("Exit safe zone");
            GameController.Instance.UpdateSafeZone(false);
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
        //towerColor = this.GetPowerBarColor();
    }

    public virtual void HandleOnAffectEnd(Transform platform)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
        isBeingAffected = false;
    }

}
