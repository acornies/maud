using System.Linq;
using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
	protected Color _originalColor;

	protected bool shouldBurnOut;
    
	//protected Quaternion? rotationTarget;
    protected Transform child;
    protected bool isBeingAffected;
	protected Light innerLight;
    //protected InAndOut inAndOutScript;
	protected Color towerColor;

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

		if (renderer != null) 
		{
			_originalColor = renderer.material.color;;
			towerColor = renderer.material.color;
		}
    }

	protected virtual void Update()
	{
        if (shouldBurnOut && innerLight.intensity >= 0.1f)
		{
			innerLight.intensity = Mathf.Lerp(innerLight.intensity, 0, lightBurnoutSpeed * Time.deltaTime);
		}
        else if (shouldBurnOut && innerLight.intensity <= 0.1f)
        {
            innerLight.enabled = false;
            shouldBurnOut = false;
        }

		if (renderer == null) return;

		if (renderer.material.color != towerColor)
		{
			//Debug.Log ("Change tower color");
			renderer.material.color = Color.Lerp(renderer.material.color, towerColor, lightBurnoutSpeed * Time.deltaTime);
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

        if (innerLight == null || !Mathf.Approximately(innerLight.intensity, 0)) return;

        innerLight.color = GetPowerBarColor();
        innerLight.intensity = lightIntensity;
        shouldBurnOut = true;
    }

	private Color GetPowerBarColor()
	{
		Color newColor = GameController.Instance.powerBarRenderer.textureBarColor;
		var colorKeys = GameController.Instance.powerBarRenderer.textureBarGradient.colorKeys;
		var powerPercentage = GameController.Instance.powerMeter / GameController.Instance.maxPower;

		var barColor = colorKeys.FirstOrDefault (x => powerPercentage <= x.time).color;

		if (barColor.r != 0 || barColor.g != 0 || barColor.b != 0) 
		{
			return barColor;
		}
		else
		{
			return newColor;
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
		towerColor = this.GetPowerBarColor ();
    }

    public virtual void HandleOnAffectEnd(Transform platform)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
        isBeingAffected = false;
    }

}
