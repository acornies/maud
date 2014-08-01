using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
	public float XMargin = 1.0f;
	public float YMargin = 1.0f;
	
	public float XSmooth = 10.0f;
	public float YSmooth = 10.0f;

    public int minCameraUpdatePlatform = 5;
	
	public Vector2 MaxXandY;
	public Vector2 MinXandY;
	
	public Transform CameraTarget;

	public delegate void UpdatedCameraMinY(float yPosition, int checkpointPlatform);
	public static event UpdatedCameraMinY On_CameraUpdatedMinY;

    public delegate void DestroyLowerPlatforms(int platformNumber, int childPlatformToDeleteIndex);
	public static event DestroyLowerPlatforms On_DestroyLowerPlatforms;

	// Subscribe to events
	void OnEnable()
	{
		PlatformController.On_ReachedCheckpoint += UpdateMinYFromCheckpoint;
		PlatformController.On_NewPlatform += HandleNewPlatform;
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
		PlatformController.On_ReachedCheckpoint -= UpdateMinYFromCheckpoint;
		PlatformController.On_NewPlatform -= HandleNewPlatform;
	}
	
	void Awake ()
	{
	}
	
	bool CheckXMargin()
	{
		return Mathf.Abs(transform.position.x - this.CameraTarget.position.x) > this.XMargin;
	}
	
	bool CheckYMargin()
	{
		return Mathf.Abs(transform.position.y - this.CameraTarget.position.y) > this.YMargin;
	}
	
	void FixedUpdate () {
		TrackPlayer();
	}
	
	void TrackPlayer()
	{
		if (CameraTarget != null)
		{
			var targetX = transform.position.x;
			var targetY = transform.position.y;
			
			if (this.CheckXMargin())
			{
				targetX = Mathf.Lerp(transform.position.x, this.CameraTarget.position.x, this.XSmooth * Time.deltaTime);
			}
			
			if (this.CheckYMargin())
			{
				targetY = Mathf.Lerp(transform.position.y, this.CameraTarget.position.y, this.YSmooth * Time.deltaTime);
			}
			
			targetX = Mathf.Clamp(targetX, this.MinXandY.x, this.MaxXandY.x);
			targetY = Mathf.Clamp(targetY, this.MinXandY.y, this.MaxXandY.y);
			
			transform.position = new Vector3(targetX, targetY, transform.position.z);
		}	
	}

    void UpdateMinYFromCheckpoint(int checkpointPlatform, int childPlatformToDeleteIndex)
	{
		var levelPlatforms = PlatformController.Instance.levelPlatforms;
		if (levelPlatforms != null && levelPlatforms.Count > 0) 
		{
			//Debug.Log ("New checkpoint is: " + checkpointPlatform);
			if (checkpointPlatform > PlatformController.Instance.checkpointBuffer)
			{
				float newCameraMinY = levelPlatforms[checkpointPlatform].transform.position.y;

				// update only if height is greater than previous since platforms will be destroyed underneath 
				if (newCameraMinY > MinXandY.y && checkpointPlatform >= minCameraUpdatePlatform)
				{
					//Debug.Log("Update camera min y to: " + newCameraMinY);
					MinXandY = new Vector2(MinXandY.x, newCameraMinY);

					On_CameraUpdatedMinY(newCameraMinY, checkpointPlatform);

                    On_DestroyLowerPlatforms(checkpointPlatform - PlatformController.Instance.checkpointBuffer - PlatformController.Instance.platformSpawnBuffer, childPlatformToDeleteIndex);
				}
			}
		}
		
	}

	void HandleNewPlatform (float yPosition)
	{
		//Debug.Log ("Increase max y to: " + yPosition);
		MaxXandY = new Vector2 (MaxXandY.x, yPosition);
	}
}