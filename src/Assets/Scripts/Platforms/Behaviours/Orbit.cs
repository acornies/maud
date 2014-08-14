using UnityEngine;
using System.Collections;

public class Orbit : PlatformBehaviour
{
    private float _stoppedTimer = 0.5f;

    public Transform center;
    public Vector3 axis = Vector3.up;
    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    public float orbitRotationSpeed = 10.0f;
    public float stopTime = 1.0f;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        center = transform;
        child.position = (child.position - center.position).normalized * radius + center.position;
    }

    void Update()
    {

    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isStopped && !isOnPlatform)
        {
            _stoppedTimer -= Time.deltaTime;
            if (!(_stoppedTimer <= 0)) return;
            isStopped = false;
        }

        if (isOnPlatform)
        {
            isStopped = true;
        }
        else
        {
            _stoppedTimer = stopTime;
            //if (rotationTarget != null) return;
            if (child == null) return;
            child.RotateAround(center.position, axis, orbitRotationSpeed);
            var desiredPosition = (child.position - center.position).normalized * radius + center.position;
            child.position = Vector3.MoveTowards(child.position, desiredPosition, radiusSpeed);
        }
    }
}
