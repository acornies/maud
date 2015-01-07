using System.Diagnostics;
using System.Linq.Expressions;
using UnityEngine;
using System.Collections;

public class UpAndDown : PlatformBehaviour
{
    public float waitTimer;

    public float speed = 1.0f;
    public float waitTime = 1.0f;
    public float maxLocalY;
    public float minLocalY;
    public Vector3 moveDirection = Vector2.up;
    public float cameraSpeed = 1f;

    public delegate void UpdateCameraSpeed(float speed);
    public static event UpdateCameraSpeed OnUpdateCameraSpeed;

    public delegate void ReturnCameraSpeed();
    public static event ReturnCameraSpeed OnReturnCameraSpeed;

    public override void HandleOnPlatformReached(Transform platform, Transform player)
    {
        base.HandleOnPlatformReached(platform, player);

        //if (platform == null || child == null) return;
        if (platform.GetInstanceID() != child.GetInstanceID()) return;

        /*if (isOnPlatform && isBeingAffected)
        {
            player.parent = null;
        }
        
        if (isOnPlatform && !isBeingAffected)
        {
            player.parent = child;
        }*/
        
        if (OnUpdateCameraSpeed != null)
        {
            OnUpdateCameraSpeed(cameraSpeed);    
        }
    }

    public override void HandlePlayerAirborne(Transform player)
    {
        base.HandlePlayerAirborne(player);

        if (OnReturnCameraSpeed != null)
        {
            OnReturnCameraSpeed();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isStopped)
        {
            waitTimer -= Time.deltaTime;
            if (!(waitTimer <= 0)) return;
            isStopped = false;
        }

        if (child == null) return;

        if (Mathf.Approximately(child.localPosition.y, maxLocalY) && waitTimer >= 0)
        {
            moveDirection = Vector3.down;
            isStopped = true;
        }
        if (Mathf.Approximately(child.localPosition.y, minLocalY) && waitTimer >= 0)
        {
            moveDirection = Vector3.up;
            isStopped = true;
        }

        if (isStopped) return;

        if (moveDirection == Vector3.up && child.localPosition.y < maxLocalY)
        {
            waitTimer = waitTime;
            child.localPosition = Vector3.MoveTowards(new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z),
                new Vector3(child.localPosition.x, maxLocalY, child.localPosition.z),
                speed * Time.deltaTime);
        }
        if (moveDirection == Vector3.down && child.localPosition.y > minLocalY)
        {
            waitTimer = waitTime;
            child.localPosition = Vector3.MoveTowards(new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z),
                new Vector3(child.localPosition.x, minLocalY, child.localPosition.z),
                speed * Time.deltaTime);
        }
    }
}
