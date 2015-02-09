using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TelekinesisHandler : MonoBehaviour
{
	private Transform child;
	private PlatformBehaviour[] _platformScripts;
    //private HazzardMovement[] _hazzardScripts;
    private float _stabilizeTimer;
    private Transform _hazzardModel;
    private float _teleportTimer;
    private ParticleSystem _rotationEffect;
    private ParticleSystem _implosionEffect;
    private ParticleSystem _smokeEffect;
    private ParticleSystem _stabilizeEffect;
	private GameObject _flash;
	private Color _towerColor;
	
	private GameObject _objectToDestroy;
	private float _destroyTimer;
	private float _postDestroyTimeout = 3f;

	public bool shouldDestroy;
	public GameObject flashEffect;
	public float colorChangeSpeed = 10f;

    protected Quaternion? rotationTarget;
    protected Vector3? teleportLocation;

    public bool isClone;
    public bool isStable;
    public float rotationSpeed = 1;
    //public float stabilizationTime = 3;
    public float teleportTime = 1;

	public float destroyTransitionSpeed = 10f;
	public float destroyTime = 2f;

    public delegate void AffectStart(Transform transformObj);
    public static event AffectStart OnAffectStart;

    public delegate void AffectEnd(Transform transformObj);
    public static event AffectEnd OnAffectEnd;

    public virtual void OnEnable()
    {
        TelekinesisController.On_NewTelekinesisRotation += HandleNewTelekinesisRotation;
        TelekinesisController.On_TelekinesisStabilize += HandleTelekinesisStabilize;
        TelekinesisController.On_NewTelekinesisMovePosition += HandleNewTelekinesisMovePosition;
		MusicController.OnFastMusicStart += HandleOnFastMusicStart;
		CameraMovement.OnRestorePlayerState += HandleOnRestorePlayerState;
        //Disappear.OnPlatformReappear += HandlePlatformReappear;
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
		_destroyTimer = destroyTime;
		audio.Play ();
	}

    void HandleOnRestorePlayerState ()
    {
		if (_platformScripts.Any(x => !x.enabled ))
		{
			_platformScripts.ToList().ForEach(x =>
			                                  {
				x.enabled = true;
			});
		}
    }

    void HandleOnFastMusicStart (float timedSpeed)
    {
		if (PlatformController.Instance.GetCurrentPlatformNumber() != int.Parse(name.Split('_')[1] ) ) return;
		//Debug.Log ("Freeze current platform for cutscene");

		// fix the case where drop platform falls while kinematics are turned off
		if (!child.rigidbody.isKinematic)
		{
			child.rigidbody.isKinematic = true;
		}
		_platformScripts.ToList().ForEach(x =>
		                                  {
			x.enabled = false;
		});
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
        TelekinesisController.On_NewTelekinesisRotation -= HandleNewTelekinesisRotation;
        TelekinesisController.On_TelekinesisStabilize -= HandleTelekinesisStabilize;
        TelekinesisController.On_NewTelekinesisMovePosition -= HandleNewTelekinesisMovePosition;
		MusicController.OnFastMusicStart -= HandleOnFastMusicStart;
		CameraMovement.OnRestorePlayerState -= HandleOnRestorePlayerState;
        //Disappear.OnPlatformReappear -= HandlePlatformReappear;
		PlatformController.OnTimedDestroy -= HandleOnTimedDestroy;
    }

    // Use this for initialization
    private void Awake()
    {
		child = transform.FindChild ("Cube");
		_platformScripts = GetComponentsInChildren<PlatformBehaviour>();
        //_hazzardScripts = GetComponentsInChildren<HazzardMovement>();
        _hazzardModel = transform.FindChild("ProtoModel");

        var effects = transform.GetComponentsInChildren<ParticleSystem>();

        if (effects == null || effects.Length <= 0) return;

        _rotationEffect = effects.FirstOrDefault(x => x.transform.name == "PowerEffect");
        _implosionEffect = effects.FirstOrDefault(x => x.transform.name == "ImplosionEffect");
        _smokeEffect = effects.FirstOrDefault(x => x.transform.name == "SmokeEffect");
        _stabilizeEffect = effects.FirstOrDefault(x => x.transform.name == "StabilizeEffect");

		if (renderer != null)
		{
			_towerColor = renderer.material.color;
		}
		
	}
	
	void Update()
    {
        if (isStable)
        {
            _stabilizeTimer -= Time.deltaTime;
        }
        else
        {
            _stabilizeTimer = PlayerState.Instance.playerLevel.stabilizeTime;
        }

        HandleStabilizePlatforms();

		if (renderer != null && renderer.material.color != _towerColor && !shouldDestroy)
		{
			//Debug.Log ("Change tower color");
			renderer.material.color = Color.Lerp(renderer.material.color, _towerColor, colorChangeSpeed * Time.deltaTime);
		}

		if (shouldDestroy && _objectToDestroy.GetInstanceID() == gameObject.GetInstanceID())
		{
			renderer.material.color = Color.Lerp(renderer.material.color, Color.white, destroyTransitionSpeed * Time.deltaTime);
			//if (renderer.material.color != Color.white) return;
			
			_destroyTimer -= Time.deltaTime;
			if (_destroyTimer > 0) return;
			
			if (child == null) return;
			
			var childRigidbody = child.GetComponent<Rigidbody>();
			if (childRigidbody != null && !childRigidbody.useGravity)
			{
				var stabilizeEffect = child.FindChild("StabilizeEffect");
				var powerEffect = child.FindChild("PowerEffect");
				
				if (stabilizeEffect != null)
				{
					Destroy(powerEffect.gameObject);
				}
				if (powerEffect != null)
				{
					Destroy(stabilizeEffect.gameObject);
				}

				_platformScripts.ToList().ForEach(x =>
				                                  {
					x.enabled = false;
				});

				if (flashEffect != null && _flash == null)
				{
					GameObject flash = Instantiate(flashEffect, transform.position, Quaternion.identity) as GameObject;
					flash.GetComponent<ParticleSystem>().Play();
					_flash = flash;
				}
				child.gameObject.layer = 0;
				child.tag = "Untagged";
				transform.DetachChildren(); // don't delete player if it's a child of the platform
				childRigidbody.constraints = RigidbodyConstraints.None;
				childRigidbody.useGravity = true;
				childRigidbody.isKinematic = false;
			}
			
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.001f, 0.001f, 0.001f), destroyTransitionSpeed * Time.deltaTime);
			
			_postDestroyTimeout -= Time.deltaTime;
			if (!(_postDestroyTimeout <= 0)) return;
			
			// destroy all the things
			Destroy (child.gameObject);
			//shouldDestroy = false;
			Destroy (gameObject);
		}
        //HandleStabilizeHazzards();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateToTarget();
        Teleport();
    }

    protected void HandleNewTelekinesisRotation(Transform platform, Quaternion rotation)
    {
        if (platform.GetInstanceID() != this.transform.GetInstanceID()) return;
        rotationTarget = rotation;
        if (_rotationEffect != null)
        {
            _rotationEffect.Play();
        }
        if (OnAffectStart != null)
        {
            OnAffectStart(transform);
        }
		_towerColor = this.GetPowerBarColor();
    }

	private Color GetPowerBarColor()
	{
		Color newColor = GameController.Instance.powerBarRenderer.textureBarColor;
		var colorKeys = GameController.Instance.powerBarRenderer.textureBarGradient.colorKeys;

		var randomKey = Random.Range (0, colorKeys.Length);
		return colorKeys [randomKey].color;
	}
	
	protected virtual void RotateToTarget()
    {
        if (!rotationTarget.HasValue) return;
        if (transform.root.localRotation != rotationTarget)
        {
            transform.root.rotation = Quaternion.Lerp(transform.root.rotation, rotationTarget.Value, rotationSpeed * Time.deltaTime);
        }
        if (transform.root.rotation == rotationTarget)
        {
            rotationTarget = null;
            if (OnAffectEnd != null)
            {
                OnAffectEnd(transform);
            }
        }
    }

    void Teleport()
    {
        if (teleportLocation.HasValue)
        {
            if (transform.position != teleportLocation.Value)
            {
                _teleportTimer -= Time.deltaTime;
                if (_hazzardModel != null)
                {
                    _hazzardModel.renderer.enabled = false;
                }
                if (_teleportTimer <= 0)
                {
                    rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z); // reset velocity after teleport
                    transform.position = teleportLocation.Value;
                    if (_hazzardModel != null)
                    {
                        if (_smokeEffect != null)
                        {
                            _smokeEffect.Play();
                        }
                        _hazzardModel.renderer.enabled = true;
                    }
                }
            }

            if (transform.position == teleportLocation.Value)
            {
                _teleportTimer = teleportTime;
                teleportLocation = null;
            }
        }
        else
        {
            _teleportTimer = teleportTime;
        }


    }

    void HandleTelekinesisStabilize(Transform stabilizedObject)
    {
        if (stabilizedObject.GetInstanceID() != this.transform.GetInstanceID()) return;

        isStable = true;

        if (_stabilizeEffect != null)
        {
            _stabilizeEffect.Play();
        }
    }

    void HandleStabilizePlatforms()
    {
        if (isStable && _platformScripts != null && _platformScripts.Any(x => x.enabled))
        {
            //Debug.Log("Stabilize platform " + transform.name);
            _platformScripts.ToList().ForEach(x =>
            {
                x.enabled = false;
            });
        }

        if (!isClone && isStable && _stabilizeTimer <= 0 && _platformScripts != null && _platformScripts.Any(x => !x.enabled))
        {
            _platformScripts.ToList().ForEach(x =>
            {
                x.enabled = true;
            });
            isStable = false;
        }
    }

    /*void HandleStabilizeHazzards()
    {
        if (isStable && _hazzardScripts != null && _hazzardScripts.Any(x => x.enabled))
        {
            //Debug.Log("Stabilize hazzard " + transform.name);
            _hazzardScripts.ToList().ForEach(x =>
            {
                x.enabled = false;
            });
            //rigidbody.useGravity = false;
        }

        if (!isClone && isStable && _stabilizeTimer <= 0 && _hazzardScripts != null && _hazzardScripts.Any(x => !x.enabled))
        {
            _hazzardScripts.ToList().ForEach(x =>
            {
                x.enabled = true;
            });
            isStable = false;
        }
    }*/

    void HandleNewTelekinesisMovePosition(Transform objectToMove, Vector3 newPosition)
    {
        if (objectToMove.GetInstanceID() != this.transform.GetInstanceID()) return;

        teleportLocation = newPosition;
        if (_implosionEffect != null)
        {
            _implosionEffect.Play();
        }
    }
}
