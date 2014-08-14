using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections;

public class UpAndDown : PlatformBehaviour
{
    public float _waitTimer;
    
    public float speed = 1.0f;
	public float waitTime = 1.0f;
	public bool isMoving = true;
	public Vector3 maxY;
	public Vector3 minY;
    public Vector3 moveDirection = Vector2.up;

    protected override void Start()
    {
        base.Start();
        maxY = new Vector3(transform.position.x, transform.position.y + (transform.localScale.y / 2), child.position.z);
        minY = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2), child.position.z);
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

        if (Mathf.Approximately(child.position.y, maxY.y) && _waitTimer >= 0)
        {
            moveDirection = Vector3.down;
            isStopped = true;
        }
        if (Mathf.Approximately(child.position.y, minY.y) && _waitTimer >= 0)
        {
            moveDirection = Vector3.up;
            isStopped = true;
        }

        if (isStopped) return;

        if (moveDirection == Vector3.up && child.position.y < maxY.y)
        {
            _waitTimer = waitTime;
            child.position = Vector3.MoveTowards(new Vector3(child.position.x, child.position.y, child.position.z),
                new Vector3(child.position.x, maxY.y, child.position.z),
                speed*Time.deltaTime);
        }
        if (moveDirection == Vector3.down && child.position.y > minY.y)
        {
            _waitTimer = waitTime;
            child.position = Vector3.MoveTowards(new Vector3(child.position.x, child.position.y, child.position.z),
                new Vector3(child.position.x, minY.y, child.position.z),
                speed*Time.deltaTime);
        }
    }
}
