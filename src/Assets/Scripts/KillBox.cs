using UnityEngine;
using System.Collections;

public class KillBox : MonoBehaviour {

	private GameObject _checkpointPlatform;

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

	/*void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.name.StartsWith ("Platform")) 
		{
			int platformNumber = int.Parse(collider.gameObject.name.Split('_')[1]);
			_spawnPlatform = PlatformSpawnControl.Instance.levelPlatforms[platformNumber + 1]; // get one platform above the killbox (check CameraMovement.killBoxBuffer)
			Debug.Log ("TriggerStay() Spawnning platform is: " + _spawnPlatform.name);
		}
	}*/

	void UpdateKillBoxAndCheckpointPosition(float newYPosition, int checkpointPlatform)
	{
		Debug.Log ("New kill box position: " + newYPosition);
		transform.position = new Vector3 (transform.position.x, newYPosition, transform.position.z);

		var levelPlatforms = PlatformSpawnControl.Instance.levelPlatforms;

		_checkpointPlatform = levelPlatforms[checkpointPlatform]; // get one platform above the killbox (check CameraMovement.killBoxBuffer)
		Debug.Log ("Checkpoint platform is: " + _checkpointPlatform.name);
	}
}
