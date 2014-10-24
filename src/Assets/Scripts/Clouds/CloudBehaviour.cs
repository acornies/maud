using UnityEngine;
using System.Collections;

public class CloudBehaviour : MonoBehaviour
{
    private float _disappearTimer;
    
    public float disappearTime = 2;
    public float speed = 1;
    public Vector3? targetPosition;

    public delegate void CloudDestroy();
    public static event CloudDestroy On_CloudDestroy;

    void OnEnable()
    {
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
            On_CloudDestroy();
            Destroy(transform.gameObject);
        }

        if (transform.position == targetPosition)
        {
            On_CloudDestroy();
            Destroy(transform.gameObject);
        }
    }

    void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
		if (player.rigidbody.velocity.y > 5f) return;
 
        //Debug.Log("Stand on cloud!");
        collider.isTrigger = false;
        _disappearTimer -= Time.deltaTime;

        player.parent = transform;

    }

    void HandleOnPlayerAirborne(Transform player)
    {
        collider.isTrigger = true;
        player.parent = null;
    }
}
