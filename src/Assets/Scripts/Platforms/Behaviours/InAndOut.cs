using UnityEngine;
using System.Collections;

public class InAndOut : PlatformBehaviour
{
    public float _waitTimer;
    public float speed = 1.0f;
    public float waitTime = 1.0f;
    public bool isMoving = true;
    public float maxLocalZ = 0.2f;
    public float minLocalZ;
    public Vector3 moveDirection = Vector3.back;
    
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //maxLocalZPosition = child.localPosition.z;
        //minLocalZ = child.localPosition.z;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isStopped)
        {
            _waitTimer -= Time.deltaTime;
            if (!(_waitTimer <= 0)) return;
            isStopped = false;
        }

        if (child == null) return;

        if (Mathf.Approximately(child.localPosition.z, maxLocalZ) && _waitTimer >= 0)
        {
            moveDirection = Vector3.back;
            isStopped = true;
        }
        if (Mathf.Approximately(child.localPosition.z, minLocalZ) && _waitTimer >= 0)
        {
            moveDirection = Vector3.forward;
            isStopped = true;
        }

        if (isStopped) return;

        if (moveDirection == Vector3.forward && child.localPosition.z < maxLocalZ)
        {
            _waitTimer = waitTime;
            child.localPosition = Vector3.MoveTowards(new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z),
                new Vector3(child.localPosition.x, child.localPosition.y, maxLocalZ),
                speed * Time.deltaTime);
        }
        if (moveDirection == Vector3.back && child.localPosition.z > minLocalZ)
        {
            _waitTimer = waitTime;
            child.localPosition = Vector3.MoveTowards(new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z),
                new Vector3(child.localPosition.x, child.localPosition.y, minLocalZ),
                speed * Time.deltaTime);
        }
    }
}
