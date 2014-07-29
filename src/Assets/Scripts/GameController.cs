using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{

	private Transform _player;
	private const float _playerZPosition = -2.8f;

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

	void OnGUI()
	{
		GUI.Label(new Rect(Screen.width - 110, 10, 100, 25), highestPoint + "m", new GUIStyle(){ alignment = TextAnchor.UpperRight});
		GUI.Label(new Rect(10, 10, 100, 25), "<3 x " + lives, new GUIStyle(){ alignment = TextAnchor.UpperLeft});
	}
	
	// Update is called once per frame
	void Update () 
	{

		if (_player.position.y > highestPoint)
		{
			highestPoint = Mathf.Round(_player.position.y);
		}

		//Debug.Log ("Highest point is: " + highestPoint);

		if (lives <= -1) 
		{
			Restart();
		}
	}

	void Restart()
	{
		Application.LoadLevel (0);
	}

	void HandleOn_PlayerDeath (float spawnYPosition)
	{
		lives--;
		//Debug.Log ("Lives: " + lives);

		_player.transform.position = new Vector3 (0, spawnYPosition, _playerZPosition);
	}
}
