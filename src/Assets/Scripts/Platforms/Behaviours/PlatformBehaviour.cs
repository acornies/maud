using System.Linq;
using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
    private GameObject _objectToDestroy;
    private float _postDestroyTimeout = 3f;
    
    private Material _originalMaterial;

    //protected bool shouldBurnOut;
    protected bool shouldDestroy;

    //protected Quaternion? rotationTarget;
    protected Transform child;
    protected bool isBeingAffected;
    //protected Light innerLight;
    //protected InAndOut inAndOutScript;
    protected Color towerColor;

    //public float rotationSpeed = 1;
    public bool isOnPlatform;
    public bool isStopped;
	public float colorChangeSpeed = 10f;
    public float destroyTransitionSpeed = 10f;
    //public float lightIntensity = 2.75f;
    //public float lightBurnoutSpeed = 1f;

    // Subscribe to events
    public virtual void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;
        TelekinesisHandler.OnAffectStart += HandleOnAffectStart;
        TelekinesisHandler.OnAffectEnd += HandleOnAffectEnd;
        PlatformController.OnTimedDestroy += HandleOnTimedDestroy;
    }

    void HandleOnTimedDestroy(GameObject objectToDestroy)
    {
        if (objectToDestroy.GetInstanceID() != gameObject.GetInstanceID()) return;

        _objectToDestroy = objectToDestroy;

        //Debug.Log ("Time destroy " + name);
        /*var stabilizeEffect = child.FindChild("StabilizeEffect");
        var powerEffect = child.FindChild("PowerEffect");
        Destroy(powerEffect.gameObject);
        Destroy(stabilizeEffect.gameObject);
        */
        shouldDestroy = true;
        transform.tag = "Untagged";
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
        PlatformController.OnTimedDestroy -= HandleOnTimedDestroy;
    }

    protected virtual void Start()
    {
        child = transform.Find("Cube");

        if (renderer != null)
        {
            _originalMaterial = renderer.material;
            towerColor = renderer.material.color;
        }
    }

    protected virtual void Update()
    {

		if (renderer != null && renderer.material.color != towerColor && !shouldDestroy)
		{
			//Debug.Log ("Change tower color");
			renderer.material.color = Color.Lerp(renderer.material.color, towerColor, colorChangeSpeed * Time.deltaTime);
		}

        if (shouldDestroy && _objectToDestroy.GetInstanceID() == gameObject.GetInstanceID())
        {
            renderer.material.color = Color.Lerp(renderer.material.color, Color.white, destroyTransitionSpeed * Time.deltaTime);
            if (renderer.material.color != Color.white) return;

			if (child == null) return;

			var childRigidbody = child.GetComponent<Rigidbody>();
            if (childRigidbody != null && !childRigidbody.useGravity)
            {
				var stabilizeEffect = child.FindChild("StabilizeEffect");
				var powerEffect = child.FindChild("PowerEffect");
				Destroy(powerEffect.gameObject);
				Destroy(stabilizeEffect.gameObject);
				transform.DetachChildren(); // don't delete player if it's a child of the platform
				childRigidbody.constraints = RigidbodyConstraints.None;
                childRigidbody.useGravity = true;
                childRigidbody.isKinematic = false;
            }

			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.01f, 0.01f, 0.01f), destroyTransitionSpeed * Time.deltaTime);

            _postDestroyTimeout -= Time.deltaTime;
            if (!(_postDestroyTimeout <= 0)) return;

            // destroy all the things
            Destroy (child.gameObject);
			//shouldDestroy = false;
            Destroy (gameObject);
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

    private Color GetPowerBarColor()
    {
        Color newColor = GameController.Instance.powerBarRenderer.textureBarColor;
        var colorKeys = GameController.Instance.powerBarRenderer.textureBarGradient.colorKeys;
        var powerPercentage = GameController.Instance.powerMeter / GameController.Instance.maxPower;

        /*var barColor = colorKeys.FirstOrDefault (x => powerPercentage <= x.time).color;

        if (barColor.r != 0 || barColor.g != 0 || barColor.b != 0) 
        {
            return barColor;
        }
        else
        {
            return newColor;
        }*/

        foreach (GradientColorKey key in colorKeys)
        {
            if (powerPercentage >= key.time)
            {
                newColor = key.color;
            }
        }

        return newColor;
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
        towerColor = this.GetPowerBarColor();
    }

    public virtual void HandleOnAffectEnd(Transform platform)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
        isBeingAffected = false;
    }

}
