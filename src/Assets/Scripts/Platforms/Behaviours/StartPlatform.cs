using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(Rigidbody)]
public class StartPlatform : PlatformBehaviour 
{
	private CameraMovement _cameraMovement;

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
		_cameraMovement = GameObject.Find ("Main Camera").GetComponent<CameraMovement>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate ();

		if (child.localPosition.y < maxLocalY)
		{
			child.localPosition = Vector3.Lerp(child.localPosition, new Vector3(child.localPosition.x, maxLocalY, child.localPosition.z), speed * Time.deltaTime);
		}

		if (child.localPosition.y >= (maxLocalY - 0.1f) && _cameraMovement.MinXandY.y == 1)
		{
			_cameraMovement.MinXandY = new Vector2(0, cameraUpdateY);
			Debug.Log("Update min camera to " + cameraUpdateY);
		}
	}

    public override void HandleOnPlatformReached(Transform platform, Transform player)
    {
        base.HandleOnPlatformReached(platform, player);

        if (platform.GetInstanceID() != child.GetInstanceID()) return;

        player.parent = child;

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
			OnReturnCameraSpeed ();
		}
	}
}
