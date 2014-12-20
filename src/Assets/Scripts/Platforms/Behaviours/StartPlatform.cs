using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//[RequireComponent(typeof(Rigidbody)]
public class StartPlatform : PlatformBehaviour
{
    private CameraMovement _cameraMovement;
    private PlayerMovement _playerMovement;

    public float maxLocalY;
    public float speed;
    public float cameraUpdateY;
    public float cameraSpeed = 1.0f;

    public delegate void UpdateCameraSpeed(float speed);
    public static event UpdateCameraSpeed OnUpdateCameraSpeed;

    public delegate void ReturnCameraSpeed();
    public static event ReturnCameraSpeed OnReturnCameraSpeed;

    void Awake()
    {
        _cameraMovement = GameObject.Find("Main Camera").GetComponent<CameraMovement>();
        _playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    public override void HandleOnPlatformReached(Transform platform, Transform player)
    {
        base.HandleOnPlatformReached(platform, player);

        if (platform.GetInstanceID() != child.GetInstanceID()) return;

       	if (!_cameraMovement.isTracking) 
		{
			_cameraMovement.isTracking = true;
			Debug.Log("Start platform turn on tracking");

			_playerMovement.disabled = false;
		}
    }

    /*public override void HandlePlayerAirborne(Transform player)
    {
        base.HandlePlayerAirborne(player);
        if (OnReturnCameraSpeed != null)
        {
            OnReturnCameraSpeed();
        }
    }*/
}
