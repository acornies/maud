using System.Runtime.Remoting.Messaging;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PushDestroyHandler : TelekinesisHandler
{
    private float _destroyTimer;
    private bool _isPushed;

    public float artificalForce = 2;
    public float destroyTimeout = 3;

    void OnEnable()
    {
        //TelekinesisController.On_TelekinesisPushDestroy += HandlePushDestroy;
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
       //TelekinesisController.On_TelekinesisPushDestroy -= HandlePushDestroy;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_isPushed)
        {
            _destroyTimer -= Time.deltaTime;
        }
        else
        {
            _destroyTimer = destroyTimeout;
        }

        if (_isPushed && _destroyTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    void HandlePushDestroy(Transform objectToDestroy, Gesture gestureInfo)
    {
        if (objectToDestroy == null || objectToDestroy.GetInstanceID() != transform.GetInstanceID()) return;

        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
        rigidbody.AddForceAtPosition(gestureInfo.swipeVector * artificalForce, transform.position);
        _isPushed = true;
    }
}
