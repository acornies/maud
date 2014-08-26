using UnityEngine;
using System.Collections;

public class CloudBehaviour : MonoBehaviour
{
    private float _disappearTimer;
    private bool _isCollidingWithPlatform;
    
    public float disappearTime = 2;
    public float speed = 1;
    public Vector3? targetPosition;

    public delegate void CloudDestroy();
    public static event CloudDestroy On_CloudDestroy;

    void OnEnable()
    {
        //CameraMovement.On_CameraUpdatedMinY += UpdateKillBoxAndCheckpointPosition;
        PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne += HandleOnPlayerAirborne;
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
        //CameraMovement.On_CameraUpdatedMinY -= UpdateKillBoxAndCheckpointPosition;
        PlayerMovement.On_PlatformReached -= HandleOnPlatformReached;
        PlayerMovement.On_PlayerAirborne -= HandleOnPlayerAirborne;
    }

    // Use this for initialization
    void Start()
    {
        _disappearTimer = disappearTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPosition.HasValue && transform.position != targetPosition)
        {
            //Debug.Log("Moving cloud");
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.Value, speed * Time.deltaTime);
        }

        if (_disappearTimer <= 0)
        {
            Destroy(transform.gameObject);
            On_CloudDestroy();
        }

        if (transform.position == targetPosition)
        {
            Destroy(transform.gameObject);
            On_CloudDestroy();
        }
    }

    void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
        if (_isCollidingWithPlatform) return;
 
        //Debug.Log("Stand on cloud!");
        collider.isTrigger = false;
        _disappearTimer -= Time.deltaTime;

    }

    void HandleOnPlayerAirborne()
    {
        collider.isTrigger = true;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag != "Stoppable") return;
        //Debug.Log("Cloud hitting " + collision.transform.parent.name);
        _isCollidingWithPlatform = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag != "Stoppable") return;
        //Debug.Log("Cloud exiting " + collision.transform.parent.name);
        _isCollidingWithPlatform = false;
    }
}
