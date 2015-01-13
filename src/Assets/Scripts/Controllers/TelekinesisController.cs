using System;
using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class TelekinesisController : MonoBehaviour
{
    private Vector3 _pointerReference;
    private Vector3 _pointerOffset;
    private Vector3 _rotation = Vector3.zero;
    private bool _shouldRotate;
    private bool _isRotating;
    private bool _isTeleporting;
    private float _powerEndTimer;
    private bool _isActivePowerTimingOut;
    private Transform _platform;
    private Transform _platformClone;
    private Transform _hazard;
    private Transform _hazzardClone;
    private AudioSource[] _teleSources;
    private AudioSource _teleAttack;
    private AudioSource _teleLoop;
    private AudioSource _teleTail;
    private AudioSource _noTele;
    private PlayerMovement _player;

    public float rotationSensitivity = 0.5f;
    public float cloneScaleMultiplier = 1.5f;
    public float powerTimeout = 1.0f;
    //public float maxFlickGestureTime = 0.2f;
    public float rotationCostPerSecond = 1f;
    public float moveCostPerSecond = 1.5f;
    public float stabilizeCost = 3;
    //public float minimumSwipeTime = 0.15f;
    public Material telekinesisMaterial;

    public delegate void TelekinesisPowersStart();
    public static event TelekinesisPowersStart On_PlayerPowersStart;

    public delegate void TelekinesisPowersEnd();
    public static event TelekinesisPowersEnd On_PlayerPowersEnd;

    public delegate void TelekinesisPowerDeplete(float amount);
    public static event TelekinesisPowerDeplete On_PlayerPowerDeplete;

    public delegate void TelekinesisRotation(Transform platformToRotate, Quaternion rotation);
    public static event TelekinesisRotation On_NewTelekinesisRotation;

    public delegate void TelekinesisStabilize(Transform objectToStabilize);
    public static event TelekinesisStabilize On_TelekinesisStabilize;

    public delegate void TelekinesisMove(Transform objectToMove, Vector3 newPosition);
    public static event TelekinesisMove On_NewTelekinesisMovePosition;

    // Subscribe to events
    void OnEnable()
    {
        if (PlayerState.Instance.Data.controlMode == ControlMode.FingerSwipe)
		{
			EasyTouch.On_Swipe2Fingers += On_Swipe;
			EasyTouch.On_SwipeStart2Fingers += On_SwipeStart;
			EasyTouch.On_SwipeEnd2Fingers += On_SwipeEnd;
			EasyTouch.On_SwipeEnd += On_SwipeEnd;
			EasyTouch.On_LongTapStart2Fingers += HandleLongTapStart;
			EasyTouch.On_LongTapEnd2Fingers += HandleLongTapEnd;
		}

		if (PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer)
		{
			EasyTouch.On_Swipe += On_Swipe;
			EasyTouch.On_SwipeStart += On_SwipeStart;
			EasyTouch.On_SwipeEnd += On_SwipeEnd;
			EasyTouch.On_LongTapStart += HandleLongTapStart;
			EasyTouch.On_LongTapEnd += HandleLongTapEnd;
		}
       

        On_PlayerPowersStart += HandleOnPowersStart;
        On_PlayerPowersEnd += HandleOnPlayerPowersEnd;
        On_TelekinesisStabilize += HandleOnTelekinesisStabilize;
		IntroTrigger.OnNewIntroLedgePosition += OnNewIntroLedegePosition;
		KillBox.On_PlayerDeath += HandleOnPlayerDeath;
		MusicController.OnFastMusicStart += HandleOnFastMusicStart;
    }

    void HandleOnFastMusicStart (float timedSpeed)
    {
		TelekinesisEnd ();
    }

    void HandleOnPlayerDeath ()
    {
		TelekinesisEnd ();
    }

    void OnDisable()
    {
        UnsubscribeEvent();
    }

    void OnDestroy()
    {
        UnsubscribeEvent();
    }

    void UnsubscribeEvent()
    {
		if (PlayerState.Instance.Data.controlMode == ControlMode.FingerSwipe)
		{
			EasyTouch.On_Swipe2Fingers -= On_Swipe;
			EasyTouch.On_SwipeStart2Fingers -= On_SwipeStart;
			EasyTouch.On_SwipeEnd2Fingers -= On_SwipeEnd;
			EasyTouch.On_SwipeEnd -= On_SwipeEnd;
			EasyTouch.On_LongTapStart2Fingers -= HandleLongTapStart;
			EasyTouch.On_LongTapEnd2Fingers -= HandleLongTapEnd;
		}

		if (PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer)
		{
			EasyTouch.On_Swipe -= On_Swipe;
			EasyTouch.On_SwipeStart -= On_SwipeStart;
			EasyTouch.On_SwipeEnd -= On_SwipeEnd;
			EasyTouch.On_LongTapStart -= HandleLongTapStart;			
			EasyTouch.On_LongTapEnd -= HandleLongTapEnd;

		}

        
        On_PlayerPowersStart -= HandleOnPowersStart;
        On_PlayerPowersEnd -= HandleOnPlayerPowersEnd;
        On_TelekinesisStabilize -= HandleOnTelekinesisStabilize;
		IntroTrigger.OnNewIntroLedgePosition -= OnNewIntroLedegePosition;
		KillBox.On_PlayerDeath -= HandleOnPlayerDeath;
		MusicController.OnFastMusicStart -= HandleOnFastMusicStart;
    }

    void Awake()
    {
        _player = GameObject.Find("Player").transform.GetComponent<PlayerMovement>();
        _teleSources = GetComponents<AudioSource>();
        if (_teleSources == null || _teleSources.Length <= 0) return;
        _teleAttack = _teleSources[0];
        _teleLoop = _teleSources[1];
        _teleTail = _teleSources[2];
        _noTele = _teleSources[3];
    }

    void Update()
    {
        if (_isActivePowerTimingOut)
        {
            if ((_shouldRotate && !_isRotating) || !_isTeleporting)
            {
                _powerEndTimer -= Time.deltaTime;
                if (!(_powerEndTimer <= 0)) return;

                TelekinesisEnd();
                _isActivePowerTimingOut = false;
            }
            else
            {
                TelekinesisEnd();
            }
        }
        else
        {
            _powerEndTimer = powerTimeout;
        }
    }

    void HandleLongTapStart(Gesture gesture)
    {
        if (_player.disabled) return;
        
        if (GameController.Instance.powerMeter < stabilizeCost)
        {
            _noTele.Play();
            return;   
        }

        Transform teleObject;
        ActivateObject(gesture, out teleObject);

        // figure out the type of object
        if (teleObject == null) return;

        var platformBehaviour = teleObject.GetComponent<PlatformBehaviour>();
        var hazardBehaviour = teleObject.GetComponent<HazardBehaviour>();
        if (platformBehaviour != null)
        {
            _platform = teleObject;
            if (On_PlayerPowersStart != null)
            {
                On_PlayerPowersStart();
            }
            On_TelekinesisStabilize(_platform);
            if (!GameController.Instance.inSafeZone)
            {
                On_PlayerPowerDeplete(stabilizeCost);
            }
        }

        if (hazardBehaviour != null)
        {
            _hazard = teleObject;
            if (On_PlayerPowersStart != null)
            {
                On_PlayerPowersStart();
            }
            On_TelekinesisStabilize(_hazard);
            if (!GameController.Instance.inSafeZone)
            {
                On_PlayerPowerDeplete(stabilizeCost);
            }
        }
    }

    private void Stabilize(Transform objectWithScripts)
    {
        if (objectWithScripts == null) return;
        var baseScripts = objectWithScripts.GetComponentsInChildren<TelekinesisHandler>();
        baseScripts.ToList().ForEach(x =>
        {
            //x.isStable = true;
            x.isClone = true;
        });

        if (_platform != null) 
		{
			var upAndDownClone = objectWithScripts.GetComponentInChildren<UpAndDown>();
			var upAndDownReal = _platform.GetComponentInChildren<UpAndDown>();
			if (upAndDownClone != null && upAndDownReal != null)
			{
				upAndDownClone.waitTimer = upAndDownReal.waitTimer;
				upAndDownClone.speed = upAndDownReal.speed;
				upAndDownClone.waitTime = upAndDownReal.waitTime;
				upAndDownClone.maxLocalY = upAndDownReal.maxLocalY;
				upAndDownClone.minLocalY = upAndDownReal.minLocalY;
				upAndDownClone.moveDirection = upAndDownReal.moveDirection;
			}
		}

    }

    void HandleLongTapEnd(Gesture gesture)
    {
        _isActivePowerTimingOut = true;
    }

    void On_SwipeStart(Gesture gesture)
    {
        if (_player.disabled) return;

		if (gesture.touchCount > 1 && PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer) return;

		//if (!_player.isGrounded) return; disable powers mid-jump

        if (GameController.Instance.powerMeter <= 0)
        {
            _noTele.Play();
            return;
        }

        Transform teleObject;
        ActivateObject(gesture, out teleObject);

        // figure out the type of object
        if (teleObject == null) return;

        var platformBehaviour = teleObject.GetComponent<PlatformBehaviour>();
        var hazardBehaviour = teleObject.GetComponent<HazardBehaviour>();
        if (platformBehaviour != null)
        {
            if (platformBehaviour.isOnPlatform) return; // disable powers if on platform
            _platform = teleObject; // TODO: change to params
            ClonePlatform();
            if (On_PlayerPowersStart != null)
            {
                On_PlayerPowersStart();
            }
            _shouldRotate = true;
        }

        if (hazardBehaviour != null)
        {
            _hazard = teleObject; // TODO: change to params
            CloneHazzard();
            if (On_PlayerPowersStart != null)
            {
                On_PlayerPowersStart();
            }
        }
    }

    private void ClonePlatform()
    {

        if (_platformClone != null) return; // prevent multi-finger object cloning
        _platformClone = Instantiate(_platform, _platform.position, _platform.rotation) as Transform;
        if (_platformClone == null) return;

        _platformClone.renderer.enabled = false;
        //TelekinesisMaterial(_platformClone);
        var cloneChild = _platformClone.FindChild("Cube");
		cloneChild.gameObject.layer = 0;
        if (cloneChild != null)
        {
            // if player is parented to the platform, DESTROY lest we spawn her N times
            var playerClone = cloneChild.FindChild("Player");
            if (playerClone != null)
            {
                //Debug.Log("Found player clone");
                Destroy(playerClone.gameObject);
            }

            TelekinesisMaterial(cloneChild);
        }
        _platformClone.localScale = new Vector3(_platform.localScale.x * cloneScaleMultiplier, _platform.localScale.y * cloneScaleMultiplier, _platform.localScale.z * cloneScaleMultiplier);
        _platformClone.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.enabled = false);
        Stabilize(_platformClone);
    }

    private void CloneHazzard()
    {
        //if (_platform == null) return;
        if (_hazzardClone != null) return; // prevent multi-finger object cloning
        _hazzardClone = Instantiate(_hazard, _hazard.position, _hazard.rotation) as Transform;
        if (_hazzardClone == null) return;

        var cloneChild = _hazzardClone.FindChild("ProtoModel");
        if (cloneChild != null)
        {
            TelekinesisMaterial(cloneChild);
        }
        _hazzardClone.localScale = new Vector3(_hazard.localScale.x * cloneScaleMultiplier, _hazard.localScale.y * cloneScaleMultiplier, _hazard.localScale.z * cloneScaleMultiplier);
        _hazzardClone.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.enabled = false);
        if (!_hazzardClone.rigidbody.isKinematic)
        {
            _hazzardClone.rigidbody.isKinematic = true;
        }
        Stabilize(_hazzardClone);
    }

    private void TelekinesisMaterial(Transform clone)
    {
        if (telekinesisMaterial == null) return;
        clone.renderer.material = telekinesisMaterial;
    }

    void On_Swipe(Gesture gesture)
    {
        if (gesture.touchCount > 1 && PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer) return;

        if (GameController.Instance.powerMeter <= 0) return;

        if (_platformClone != null && _platform != null)
        {
            Rotate(gesture);
        }
        if (_hazzardClone != null && _hazard != null)
        {
            TeleportMove(gesture);
        }
    }

    private void Rotate(Gesture gesture)
    {
        _isActivePowerTimingOut = false;

        if (!_shouldRotate) return;

        // offset
        _pointerOffset = (new Vector3(gesture.position.x, gesture.position.y, 0) - _pointerReference);

        // apply rotation
        _rotation.y = -(_pointerOffset.x + _pointerOffset.y) * rotationSensitivity;

        _isRotating = true;

        // rotate
        _platformClone.Rotate(_rotation);

        // store mouse
        _pointerReference = gesture.position;

        if (!GameController.Instance.inSafeZone)
        {
            On_PlayerPowerDeplete(rotationCostPerSecond * Time.deltaTime);
        }
    }

    private void TeleportMove(Gesture gesture)
    {
        _isTeleporting = true;
        _hazzardClone.position = gesture.GetTouchToWordlPoint(GameController.Instance.playerZPosition, true);

        if (!GameController.Instance.inSafeZone)
        {
            On_PlayerPowerDeplete(moveCostPerSecond * Time.deltaTime);
        }
    }

    void On_SwipeEnd(Gesture gesture)
    {
		//if (gesture.touchCount == 1 && PlayerState.Instance.Data.controlMode == ControlMode.FingerSwipe) return;

        if (_platform != null)
        {
            _isActivePowerTimingOut = true;
            On_NewTelekinesisRotation(_platform, _platformClone.localRotation);
            _isRotating = false;
        }
        else if (_hazard != null)
        {
            _isActivePowerTimingOut = true;
            On_NewTelekinesisMovePosition(_hazard, _hazzardClone.position);
            _isTeleporting = false;
        }
        else
        {
            _isActivePowerTimingOut = true;
        }
    }

    private void TelekinesisEnd()
    {
        _shouldRotate = false;
        if (_platformClone != null)
        {
            Destroy(_platformClone.gameObject);
            _platformClone = null;
        }

        _platform = null;
        if (On_PlayerPowersEnd != null)
        {
            On_PlayerPowersEnd();
        }
        _hazard = null;
        if (_hazzardClone != null)
        {
            Destroy(_hazzardClone.gameObject);
            _hazzardClone = null;
        }
    }

    private void ActivateObject(Gesture gesture, out Transform teleObject)
    {
        Transform teleTransform = null;
        _pointerReference = gesture.position;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(gesture.position), out hitInfo);
        if (hit)
        {
            if (hitInfo.transform.tag == "Rotatable" && hitInfo.transform.gameObject.layer == 10)
            {
                teleTransform = hitInfo.transform;

            }
            if (hitInfo.transform.tag == "Stoppable" && hitInfo.transform.parent.gameObject.layer == 10)
            {
                teleTransform = hitInfo.transform.parent;
            }

            if (hitInfo.transform.tag == "Hazzard")
            {
                teleTransform = hitInfo.transform;
            }
        }

        teleObject = teleTransform;
    }

    void HandleOnPowersStart()
    {
        if (_teleAttack.clip == null || _teleLoop.clip == null) return;

        _teleAttack.Play();


        if (!_teleLoop.isPlaying)
        {
            _teleLoop.PlayDelayed(_teleAttack.clip.length);
        }
    }

    void HandleOnPlayerPowersEnd()
    {
        if (_teleTail.clip == null || !_teleLoop.isPlaying) return;

        _teleTail.PlayOneShot(_teleTail.clip);
        _teleLoop.Stop();
    }

    void HandleOnTelekinesisStabilize(Transform transformtoStabilize)
    {
        if (_teleTail.clip != null && !_teleTail.isPlaying)
        {
            _teleTail.Play();
            if (_teleLoop.isPlaying)
            {
                _teleLoop.Stop();
            }
        }
    }

	void OnNewIntroLedegePosition(Vector3 newPosition)
	{
		var initialDelay = .2f;
		_teleAttack.PlayDelayed (initialDelay);
		_teleTail.PlayDelayed (initialDelay + _teleAttack.clip.length);
	}
}
