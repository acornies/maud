using System;
using System.Security.Cryptography;
using System.Security.Policy;
using UnityEngine;
using System.Collections;
using System.Linq;

public class TelekinesisController : MonoBehaviour
{
    private Vector3 _pointerReference;
    private Vector3 _pointerOffset;
    private Vector3 _rotation = Vector3.zero;
    private float _powerEndTimer;
    private bool _isActivePowerTimingOut;
    private Transform _platform;
    private Transform _platformClone;
    public Transform _hazzard;

    public float sensitivity = 0.5f;
    public bool shouldRotate;
    public bool isRotating = false;
    public float powerTimeout = 1.0f;
    public float cloneScaleMultiplier = 1.5f;
    public float maxFlickGestureTime = 0.2f;

    public delegate void TelekinesisPowersStart();
    public static event TelekinesisPowersStart On_PlayerPowersStart;

    public delegate void TelekinesisPowersEnd();
    public static event TelekinesisPowersEnd On_PlayerPowersEnd;

    public delegate void TelekinesisNewRotation(Transform platformToRotate, Quaternion rotation);
    public static event TelekinesisNewRotation On_NewTelekinesisRotation;

    public delegate void TelekinesisStabilize(Transform objectToStabilize);
    public static event TelekinesisStabilize On_TelekinesisStabilize;

    public delegate void TelekinesisPushDestroy(Transform platformToDestroy, Gesture gesture);
    public static event TelekinesisPushDestroy On_TelekinesisPushDestroy;

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
    }

    void Update()
    {
        if (_isActivePowerTimingOut)
        {
            if (shouldRotate && !isRotating)
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

    void HandlePlayerAirborne()
    {
        TelekinesisEnd();
    }

    void HandleLongTapStart(Gesture gesture)
    {
        ActivatePlatform(gesture);
        if (_platform != null)
        {
            //DisableBehaviour(_platform);
            //_platform.GetComponent<TelekinesisHandler>().isStable = true;
            On_TelekinesisStabilize(_platform);
        }

        if (_hazzard != null)
        {
            //DisableBehaviour(_hazzard);
            //_hazzard.GetComponent<TelekinesisHandler>().isStable = true;
            On_TelekinesisStabilize(_hazzard);
        }
    }

    private static void DisableBehaviour(Transform platformWithScripts)
    {
        if (platformWithScripts == null) return;
        var scriptsToDisable = platformWithScripts.GetComponentsInChildren<TelekinesisHandler>();
        scriptsToDisable.ToList().ForEach(x =>
        {
            x.isStable = true;
            x.isClone = true;
        });
    }

    void HandleLongTapEnd(Gesture gesture)
    {
        if (_platform != null)
        {
            _isActivePowerTimingOut = true;
        }
    }

    void On_SwipeStart(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;
        ActivatePlatform(gesture);
        if (_platform == null) return;
        if (_platformClone != null) return; // prevent multi-finger object cloning
        _platformClone = Instantiate(_platform, _platform.position, _platform.rotation) as Transform;
        if (_platformClone == null) return;
        _platformClone.renderer.material.color = new Color(1, 1, 1, .5f);
        var cloneChild = _platformClone.FindChild("Cube");
        if (cloneChild != null)
        {
            cloneChild.renderer.material.color = new Color(1, 1, 1, .5f);
        }
        _platformClone.localScale = new Vector3(_platform.localScale.x * cloneScaleMultiplier, _platform.localScale.y * cloneScaleMultiplier, _platform.localScale.z * cloneScaleMultiplier);
        _platformClone.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.enabled = false);
        DisableBehaviour(_platformClone);
    }

    void On_Swipe(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;
        Rotate(gesture);
    }

    private void Rotate(Gesture gesture)
    {
        _isActivePowerTimingOut = false;

        if (!shouldRotate || _platform == null) return;

        // offset
        _pointerOffset = (new Vector3(gesture.position.x, gesture.position.y, 0) - _pointerReference);

        // apply rotation
        _rotation.y = -(_pointerOffset.x + _pointerOffset.y) * sensitivity;

        isRotating = true;

        // rotate
        _platformClone.Rotate(_rotation);

        // store mouse
        _pointerReference = gesture.position;
    }

    void On_SwipeEnd(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;

        isRotating = false;
        //if (_platform != null && gesture.actionTime > maxFlickGestureTime)
        if (_platform != null)
        {
            _isActivePowerTimingOut = true;
            On_NewTelekinesisRotation(_platform, _platformClone.localRotation);
        }
        /*if (_hazzard != null && _hazzard.GetComponent<TelekinesisHandler>().isStable)
        {
            _isActivePowerTimingOut = true;
            //Debug.Log("hold & destroy");
            On_TelekinesisPushDestroy(_hazzard, gesture);
        }*/
        else
        {
            _isActivePowerTimingOut = true;
        }
    }

    private void TelekinesisEnd()
    {
        shouldRotate = false;
        if (_platformClone != null)
        {
            Destroy(_platformClone.gameObject);
            _platformClone = null;
        }
        if (_platform != null && _platform.particleSystem != null)
        {
            _platform.particleSystem.Stop();
        }
        _platform = null;
        if (On_PlayerPowersEnd != null)
        {
            On_PlayerPowersEnd();
        }
        _hazzard = null;
    }

    private void ActivatePlatform(Gesture gesture)
    {
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
                shouldRotate = true;
                if (_platform != null && _platform.particleSystem != null)
                {
                    _platform.particleSystem.Play();
                }
            }
            if (hitInfo.transform.tag == "Stoppable")
            {
                this._platform = hitInfo.transform.parent;

                if (On_PlayerPowersStart != null)
                {
                    On_PlayerPowersStart();
                }
                shouldRotate = true;
                if (_platform != null && _platform.particleSystem != null)
                {
                    _platform.particleSystem.Play();
                }
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
}
