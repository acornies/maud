﻿using UnityEngine;
using System.Collections;

public class Drop : PlatformBehaviour
{
    private bool _isDropping;
    private bool _atOriginalPosition = true;
    private float _originalYPosition;

    public float maxY = 0.0f;
    public float minY = 0.0f;
    public float returnSpeed = 0.1f;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        _originalYPosition = child.position.y;
        maxY = transform.position.y + (transform.localScale.y / 2);
        minY = transform.position.y - (transform.localScale.y / 2);
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (child == null) return;
        _atOriginalPosition = child.position.y == _originalYPosition;

        if (_isDropping && child.position.y >= minY)
        {
            child.rigidbody.isKinematic = false;
            child.rigidbody.useGravity = true;
        }
        else
        {
            child.rigidbody.isKinematic = true;
            child.rigidbody.useGravity = false;
        }

        if (!_atOriginalPosition && !_isDropping)
        {
            child.position = Vector3.Lerp(child.position, new Vector3(child.position.x, _originalYPosition, child.position.z), returnSpeed * Time.deltaTime);
        }
    }

    public override void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform.GetInstanceID() != child.GetInstanceID()) return;
        _isDropping = true;
        isOnPlatform = true;
        player.parent = child;
    }

    public override void HandlePlayerAirborne(Transform player)
    {
        _isDropping = false;
        isOnPlatform = false;
        player.parent = null;
    }
}
