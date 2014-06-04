using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
	
	public float XMargin = 1.0f;
	public float YMargin = 1.0f;
	
	public float XSmooth = 10.0f;
	public float YSmooth = 10.0f;
	
	public Vector2 MaxXandY;
	public Vector2 MinXandY;
	
	public Transform CameraTarget;
	
	void Awake ()
	{
		this.CameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
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
		this.TrackPlayer();
	}
	
	void TrackPlayer()
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