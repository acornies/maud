using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour {

	public delegate void PlayerDeath();
	public static event PlayerDeath On_PlayerDeath; 

	// Subscribe to events
	void OnEnable()
	{
		CameraMovement.On_CameraUpdatedMinY += UpdateKillBoxPosition;
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
		CameraMovement.On_CameraUpdatedMinY -= UpdateKillBoxPosition;
	}
	
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.name == "Player") 
		{
			On_PlayerDeath();
		}
	}

	void UpdateKillBoxPosition(float newYPosition)
	{
		Debug.Log ("New kill box position: " + newYPosition);
		transform.position = new Vector3 (transform.position.x, newYPosition, transform.position.z);
	}
}
