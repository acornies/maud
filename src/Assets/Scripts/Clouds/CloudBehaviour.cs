using UnityEngine;
using System.Collections;

public class CloudBehaviour : MonoBehaviour
{
    private float _disappearTimer;
    private bool _isCollidingWithPlatform;
    private bool _solidify;
    private float _solidifyTimer;
    
    public float disappearTime = 2;
    public float solidifyTime = 0.15f;
    public float speed = 1;
    public Vector3? targetPosition;

    public delegate void CloudDestroy();
    public static event CloudDestroy On_CloudDestroy;

    void OnEnable()
    {
        PlayerMovement.On_PlatformReached += HandleOnPlatformReached;
        //PlayerMovement.On_PlayerAirborne += HandleOnPlayerAirborne;
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
        //PlayerMovement.On_PlayerAirborne -= HandleOnPlayerAirborne;
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

        if (_solidify && collider.isTrigger)
        {
            _solidifyTimer -= Time.deltaTime;
            if (_solidifyTimer <= 0)
            {
                collider.isTrigger = false;
                _solidifyTimer = solidifyTime;
                Debug.Log("Turn on collider");
            }
           
        }
        else
        {
            _solidifyTimer = solidifyTime;
            //_solidify = false;
        }
    }

    void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
        if (_isCollidingWithPlatform) return;
 
        //Debug.Log("Stand on cloud!");
        //collider.isTrigger = false;
        _solidify = true;
        _disappearTimer -= Time.deltaTime;

        player.parent = transform;

    }

    void HandleOnPlayerAirborne(Transform player)
    {
        collider.isTrigger = true;
        player.parent = null;
        _solidify = false;
        //_solidifyTimer = solidifyTime;
        Debug.Log("Turn on trigger");
    }

    /*void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag != "Stoppable") return;
        //Debug.Log("Cloud hitting " + collision.transform.parent.name);
        _isCollidingWithPlatform = true;
    }*/

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.name != "Player") return;
        collider.isTrigger = true;
        _solidify = false;
        //_solidifyTimer = solidifyTime;
        Debug.Log("Turn on trigger");
    }

    /*void OnTriggerStay(Collider triggerCollider)
    {
        if (triggerCollider.name == "Player")
        {
            _solidify = true;
            //collider.isTrigger = false;
        }
    }*/

    /*void OnTriggerExit(Collider triggerCollider)
    {
        if (triggerCollider.name == "Player")
        {
            if (_solidify = true;
            //collider.isTrigger = false;
        }
    }*/

    /*void OnCollisionExit(Collision triggerCollision)
    {
        if (triggerCollision.transform.name == "Player")
        {
            collider.isTrigger = true;
            _solidify = false;
            Debug.Log("Turn on trigger");
        }
    }*/

}
