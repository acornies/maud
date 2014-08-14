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
    private Transform _platformClone;

    public float sensitivity = 0.5f;
    public Transform platform;
    public bool shouldRotate;
    public bool isRotating = false;
    public float powerTimeout = 1.0f;
    public float cloneScaleMultiplier = 1.5f;

    public delegate void TelekinesisPowersStart();
    public static event TelekinesisPowersStart On_PlayerPowersStart;

    public delegate void TelekinesisPowersEnd();
    public static event TelekinesisPowersEnd On_PlayerPowersEnd;

    public delegate void TelekinesisNewRotation(Transform platformToRotate, Quaternion rotation);
    public static event TelekinesisNewRotation On_NewPlatformRotation;

    // Subscribe to events
    void OnEnable()
    {
        EasyTouch.On_Swipe2Fingers += On_Swipe;
        EasyTouch.On_SwipeStart2Fingers += On_SwipeStart;
        EasyTouch.On_SwipeEnd2Fingers += On_SwipeEnd;
        EasyTouch.On_Swipe += On_Swipe;
        EasyTouch.On_SwipeStart += On_SwipeStart;
        EasyTouch.On_SwipeEnd += On_SwipeEnd;
        EasyTouch.On_LongTapStart += HandleLongTap;
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
        EasyTouch.On_LongTapStart -= HandleLongTap;
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

    void HandleLongTap(Gesture gesture)
    {
        ActivatePlatform(gesture);
        Stabilize(platform);
    }

    private static void Stabilize(Transform platformWithScripts)
    {
        if (platformWithScripts == null) return;
        var scriptsToDisable = platformWithScripts.GetComponentsInChildren<PlatformBehaviour>();
        scriptsToDisable.ToList().ForEach(x =>
        {
            x.enabled = false;
        });
    }

    void HandleLongTapEnd(Gesture gesture)
    {
        if (platform != null)
        {
            _isActivePowerTimingOut = true;
        }
    }

    void On_SwipeStart(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;
        ActivatePlatform(gesture);
        if (platform == null) return;
        _platformClone = Instantiate(platform, platform.position, platform.rotation) as Transform;
        if (_platformClone == null) return;
        _platformClone.renderer.material.color = new Color(1, 1, 1, .5f);
        _platformClone.FindChild("Cube").renderer.material.color = new Color(1, 1, 1, .5f);
        _platformClone.localScale = new Vector3(platform.localScale.x * cloneScaleMultiplier, platform.localScale.y * cloneScaleMultiplier, platform.localScale.z * cloneScaleMultiplier);
        _platformClone.GetComponentsInChildren<Collider>().ToList().ForEach(x => x.enabled = false);
        Stabilize(_platformClone);
    }

    void On_Swipe(Gesture gesture)
    {
        if (gesture.touchCount == 1 && !GameController.Instance.useAcceleration) return;
        Rotate(gesture);
    }

    private void Rotate(Gesture gesture)
    {
        _isActivePowerTimingOut = false;

        if (!shouldRotate || platform == null) return; 

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
        //shouldRotate = false;
        isRotating = false;
        if (platform != null)
        {
            _isActivePowerTimingOut = true;
            On_NewPlatformRotation(platform, _platformClone.localRotation);
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
        if (platform != null && platform.particleSystem != null)
        {
            platform.particleSystem.Stop();
        }
        platform = null;
        if (On_PlayerPowersEnd != null)
        {
            On_PlayerPowersEnd();
        }
    }

    private void ActivatePlatform(Gesture gesture)
    {
        _pointerReference = gesture.position;
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(gesture.position), out hitInfo);
        if (hit)
        {
            //Debug.Log("Hit " + hitInfo.transform.gameObject.name);
            if (hitInfo.transform.gameObject.tag == "Rotatable")
            {
                //Debug.Log ("It's active");
                this.platform = hitInfo.transform;

                if (On_PlayerPowersStart != null)
                {
                    On_PlayerPowersStart();
                }
                shouldRotate = true;
                if (platform != null && platform.particleSystem != null)
                {
                    platform.particleSystem.Play();
                }
            }
            if (hitInfo.transform.gameObject.tag == "Stoppable")
            {
                //Debug.Log("Hit child, rotate parent: " + hitInfo.transform.parent.name);
                this.platform = hitInfo.transform.parent;

                if (On_PlayerPowersStart != null)
                {
                    On_PlayerPowersStart();
                }
                shouldRotate = true;
                if (platform != null && platform.particleSystem != null)
                {
                    platform.particleSystem.Play();
                }
            }
        }
    }
}
