using UnityEngine;
using System.Collections;

public class CloudBehaviour : MonoBehaviour
{
    private float _disappearTimer;
    private bool _shouldDisappear;
    
    public float disappearTime = 2;
    public float speed = 1;
    public Vector3? targetPosition;
    public float disappearingSpeed = 10f;
    public float cameraSpeed = 1f;

    public delegate void CloudDestroy();
    public static event CloudDestroy On_CloudDestroy;

    public delegate void UpdateCameraSpeed(float speed);
    public static event UpdateCameraSpeed OnUpdateCameraSpeed;

    public delegate void ReturnCameraSpeed();
    public static event ReturnCameraSpeed OnReturnCameraSpeed;

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
            //On_CloudDestroy();
            //Destroy(transform.gameObject);
            _shouldDisappear = true;
        }

        else if (transform.position == targetPosition)
        {
            //On_CloudDestroy();
           // Destroy(transform.gameObject);
            _shouldDisappear = true;
        }

        if (_shouldDisappear)
        {
            renderer.material.color = Color.Lerp(renderer.material.color,
           new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b,
               0f), disappearingSpeed * Time.deltaTime);
            if (renderer.material.color.a <= 0.05)
            {
                On_CloudDestroy();
                Destroy(transform.gameObject);
            }
        }
    }

    void HandleOnPlatformReached(Transform platform, Transform player)
    {
        if (platform.GetInstanceID() != transform.GetInstanceID()) return;
		if (player.rigidbody.velocity.y > 0) return;
 
        //Debug.Log("Stand on cloud!");
        collider.isTrigger = player.GetComponent<PlayerMovement>().isHittingHead;
		//player.rigidbody.velocity = new Vector3(player.rigidbody.velocity.x, 0, player.rigidbody.velocity.z);
        _disappearTimer -= Time.deltaTime;

        player.parent = transform;

        if (OnUpdateCameraSpeed != null)
        {
            OnUpdateCameraSpeed(cameraSpeed);
        }

    }

    void HandleOnPlayerAirborne(Transform player)
    {
        collider.isTrigger = true;
        player.parent = null;
        if (OnReturnCameraSpeed != null)
        {
            OnReturnCameraSpeed();
        }
    }
}
