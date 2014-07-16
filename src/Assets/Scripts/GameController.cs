using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	private Transform _player;

	public int lives = 3;
	public float highestPoint = 0.0f;
	public static GameController Instance { get; private set;}

	// Subscribe to events
	void OnEnable()
	{
		KillBox.On_PlayerDeath += HandleOn_PlayerDeath;
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
		KillBox.On_PlayerDeath -= HandleOn_PlayerDeath;
	}

	void Awake ()
	{
		if (Instance == null)
		{
			//DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () 
	{
		_player = GameObject.Find ("Player").transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void HandleOn_PlayerDeath (float spawnYPosition)
	{
		//Debug.Log ("Spawn another player at:" + spawnPosition);
		// TODO: pass a Vector3 for the spawn position
		/*GameObject newPlayer = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/Player"), 
		                                                new Vector3 (0, spawnYPosition, -2.8f), Quaternion.identity);
		newPlayer.name = "Player";*/

		lives--;
		Debug.Log ("Lives: " + lives);

		_player.transform.position = new Vector3 (0, spawnYPosition, -2.8f);
	}
}
