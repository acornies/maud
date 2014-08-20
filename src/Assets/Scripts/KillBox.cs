﻿using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour
{

    private GameObject _checkpointPlatform;

    public float cameraPositionBuffer = 8.0f;

    public delegate void PlayerDeath(float spawnPosition);
    public static event PlayerDeath On_PlayerDeath;

    // Subscribe to events
    void OnEnable()
    {
        CameraMovement.On_CameraUpdatedMinY += UpdateKillBoxAndCheckpointPosition;
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
        CameraMovement.On_CameraUpdatedMinY -= UpdateKillBoxAndCheckpointPosition;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "Player")
        {
            //Destroy(collider.gameObject);
            if (_checkpointPlatform != null)
            {
                On_PlayerDeath(_checkpointPlatform.transform.position.y);
            }
            else
            {
                On_PlayerDeath(0.2f);
            }
        }
    }

    void UpdateKillBoxAndCheckpointPosition(float newYPosition, int checkpointPlatform)
    {
        //Debug.Log ("New kill box position: " + newYPosition);
        transform.position = new Vector3(transform.position.x, newYPosition - cameraPositionBuffer, transform.position.z);

        var levelPlatforms = PlatformController.Instance.levelPlatforms;

        _checkpointPlatform = levelPlatforms[checkpointPlatform]; // get one platform above the killbox (check CameraMovement.killBoxBuffer)
        //Debug.Log ("Checkpoint platform is: " + _checkpointPlatform.name);
    }
}
