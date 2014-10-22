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
    private Transform _hazzard;
    private Transform _hazzardClone;
    private float _rotationTimer;
    private PlayerMovement _player;
    private AudioSource[] _teleSources;
    private AudioSource _teleAttack;
    private AudioSource _teleLoop;
    private AudioSource _teleTail;

    public float rotationSensitivity = 0.5f;
    public float cloneScaleMultiplier = 1.5f;
    public float powerTimeout = 1.0f;
    //public float maxFlickGestureTime = 0.2f;
    public float rotationCostPerSecond = 1f;
    public float moveCostPerSecond = 1.5f;
    public float stabilizeCost = 3;

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
        EasyTouch.On_Swipe2Fingers += On_Swipe;
        EasyTouch.On_SwipeStart2Fingers += On_SwipeStart;
        EasyTouch.On_SwipeEnd2Fingers += On_SwipeEnd;
        EasyTouch.On_Swipe += On_Swipe;
        EasyTouch.On_SwipeStart += On_SwipeStart;
        EasyTouch.On_SwipeEnd += On_SwipeEnd;
        EasyTouch.On_LongTapStart += HandleLongTapStart;
        EasyTouch.On_LongTapEnd += HandleLongTapEnd;
        PlayerMovement.On_PlayerAirborne += HandlePlayerAirborne;
        On_PlayerPowersStart += HandleOnPowersStart;
        On_PlayerPowersEnd += HandleOnPlayerPowersEnd;
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
        EasyTouch.On_Swipe2Fingers -= On_Swipe;
        EasyTouch.On_SwipeStart2Fingers -= On_SwipeStart;
        EasyTouch.On_SwipeEnd2Fingers -= On_SwipeEnd;
        EasyTouch.On_Swipe -= On_Swipe;
        EasyTouch.On_SwipeStart -= On_SwipeStart;
        EasyTouch.On_SwipeEnd -= On_SwipeEnd;
        EasyTouch.On_LongTapStart -= HandleLongTapStart;
        EasyTouch.On_LongTapEnd -= HandleLongTapEnd;
        PlayerMovement.On_PlayerAirborne -= HandlePlayerAirborne;
        On_PlayerPowersStart -= HandleOnPowersStart;
        On_PlayerPowersEnd -= HandleOnPlayerPowersEnd;
    }

    void Awake()
    {
        _player = GameObject.Find("Player").transform.GetComponent<PlayerMovement>();
        _teleSources = GetComponents<AudioSource>();
        if (_teleSources == null || _teleSources.Length <= 0) return;
        _teleAttack = _teleSources[0];
        _teleLoop = _teleSources[1];
        _teleTail = _teleSources[2];
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

    void HandlePlayerAirborne(Transform player)
    {
        TelekinesisEnd();
    }

    void HandleLongTapStart(Gesture gesture)
    {
        if (GameController.Instance.powerMeter < stabilizeCost) return;

        ActivateObject(gesture);
        if (_platform != null)
        {
            On_TelekinesisStabilize(_platform);
            On_PlayerPowerDeplete(stabilizeCost);
        }

        if (_hazzard != null)
        {
            On_TelekinesisStabilize(_hazzard);
            On_PlayerPowerDeplete(stabilizeCost);
        }

        if (_teleTail.clip != null && !_teleTail.isPlaying)
        {
            _teleTail.PlayDelayed(_teleAttack.clip.length);
            if (_teleLoop.isPlaying)
            {
                _teleLoop.Stop();
            }
        }
    }

    private static void Stabilize(Transform objectWithScripts)
    {
        if (objectWithScripts == null) return;
        var scriptsToDisable = objectWithScripts.GetComponentsInChildren<TelekinesisHandler>();
        scriptsToDisable.ToList().ForEach(x =>
        {
            x.isStable = true;
            x.isClone = true;
        });
    }

    void HandleLongTapEnd(Gesture gesture)
    {
        _isActivePowerTimingOut = true;
    }

    void On_SwipeStart(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;

        ActivateObject(gesture);
        
        if (_platform != null)
        {
            ClonePlatform();
        }

        if (_hazzard != null)
        {
            CloneHazzard();
        }
    }

    private void ClonePlatform()
    {

        if (_platformClone != null) return; // prevent multi-finger object cloning
        _platformClone = Instantiate(_platform, _platform.position, _platform.rotation) as Transform;
        if (_platformClone == null) return;

        _platformClone.renderer.material.color = new Color(1, 1, 1, .5f);
        var cloneChild = _platformClone.FindChild("Cube");
        if (cloneChild != null)
        {
            // if player is parented to the platform, DESTROY lest we spawn her N times
            var playerClone = cloneChild.FindChild("Player");
            if (playerClone != null)
            {
                //Debug.Log("Found player clone");
                Destroy(playerClone.gameObject);
            }
            
            cloneChild.renderer.material.color = new Color(1, 1, 1, .5f);
        }
        _platformClone.localScale = new Vector3(_platform.localScale.x * cloneScaleMultiplier, _platform.localScale.y * cloneScaleMultiplier, _platform.localScale.z * cloneScaleMultiplier);
        _platformClone.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.enabled = false);
        Stabilize(_platformClone);
    }

    private void CloneHazzard()
    {
        //if (_platform == null) return;
        if (_hazzardClone != null) return; // prevent multi-finger object cloning
        _hazzardClone = Instantiate(_hazzard, _hazzard.position, _hazzard.rotation) as Transform;
        if (_hazzardClone == null) return;
        //_hazzardClone.renderer.material.color = new Color(1, 1, 1, .5f);
        var cloneChild = _hazzardClone.FindChild("ProtoModel");
        if (cloneChild != null)
        {
            cloneChild.renderer.material.color = new Color(1, 1, 1, .5f);
        }
        _hazzardClone.localScale = new Vector3(_hazzard.localScale.x * cloneScaleMultiplier, _hazzard.localScale.y * cloneScaleMultiplier, _hazzard.localScale.z * cloneScaleMultiplier);
        _hazzardClone.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.enabled = false);
        if (!_hazzardClone.rigidbody.isKinematic)
        {
            _hazzardClone.rigidbody.isKinematic = true;
        }
        Stabilize(_hazzardClone);
    }

    void On_Swipe(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;

        if (GameController.Instance.powerMeter <= 0) return;

        if (_platformClone != null && _platform != null)
        {
            Rotate(gesture);
        }
        if (_hazzardClone != null && _hazzard != null)
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

        //GameController.Instance.powerMeter -= rotationCostPerSecond * Time.deltaTime;
        On_PlayerPowerDeplete(rotationCostPerSecond * Time.deltaTime);
    }

    private void TeleportMove(Gesture gesture)
    {
        _isTeleporting = true;
        _hazzardClone.position = gesture.GetTouchToWordlPoint(GameController.Instance.playerZPosition, true);
        On_PlayerPowerDeplete(moveCostPerSecond * Time.deltaTime);
    }

    void On_SwipeEnd(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;

        if (_platform != null)
        {
            _isActivePowerTimingOut = true;
            On_NewTelekinesisRotation(_platform, _platformClone.localRotation);
            _isRotating = false;
        }
        else if (_hazzard != null)
        {
            _isActivePowerTimingOut = true;
            On_NewTelekinesisMovePosition(_hazzard, _hazzardClone.position);
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
        _hazzard = null;
        if (_hazzardClone != null)
        {
            Destroy(_hazzardClone.gameObject);
            _hazzardClone = null;
        }
    }

    private void ActivateObject(Gesture gesture)
    {
        if (!_player.isGrounded) return;
        
        _pointerReference = gesture.position;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(gesture.position), out hitInfo);
        if (hit)
        {
            if (hitInfo.transform.tag == "Rotatable")
            {
                this._platform = hitInfo.transform;

                if (On_PlayerPowersStart != null)
                {
                    On_PlayerPowersStart();
                }
                _shouldRotate = true;

            }
            if (hitInfo.transform.tag == "Stoppable")
            {
                this._platform = hitInfo.transform.parent;

                if (On_PlayerPowersStart != null)
                {
                    On_PlayerPowersStart();
                }
                _shouldRotate = true;
            }

            if (hitInfo.transform.tag == "Hazzard")
            {
                this._hazzard = hitInfo.transform;
                if (On_PlayerPowersStart != null)
                {
                    On_PlayerPowersStart();
                }
            }
        }
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
        if (!_teleTail.isPlaying)
        {
            _teleTail.Play();
        }     
        _teleLoop.Stop();
    }
}
