using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections;

public class UpAndDown : PlatformBehaviour
{
    private float _waitTimer;

    public float speed = 1.0f;
    public float waitTime = 1.0f;
    public float maxLocalY;
    public float minLocalY;
    public Vector3 moveDirection = Vector2.up;

    protected override void Start()
    {
        base.Start();
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

        if (Mathf.Approximately(child.localPosition.y, maxLocalY) && _waitTimer >= 0)
        {
            moveDirection = Vector3.down;
            isStopped = true;
        }
        if (Mathf.Approximately(child.localPosition.y, minLocalY) && _waitTimer >= 0)
        {
            moveDirection = Vector3.up;
            isStopped = true;
        }

        if (isStopped) return;

        if (moveDirection == Vector3.up && child.localPosition.y < maxLocalY)
        {
            _waitTimer = waitTime;
            child.localPosition = Vector3.MoveTowards(new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z),
                new Vector3(child.localPosition.x, maxLocalY, child.localPosition.z),
                speed * Time.deltaTime);
        }
        if (moveDirection == Vector3.down && child.localPosition.y > minLocalY)
        {
            _waitTimer = waitTime;
            child.localPosition = Vector3.MoveTowards(new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z),
                new Vector3(child.localPosition.x, minLocalY, child.localPosition.z),
                speed * Time.deltaTime);
        }
    }
}
