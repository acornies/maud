using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BoundaryControl : MonoBehaviour {

	private GameObject _leftBoundary;
	private GameObject _rightBoundary;

	//public static BoundaryControl Instance { get; private set;}
	public float leftBoundaryX = -4.0f;
	public float rightBoundaryX = 4.0f;
	public float verticalBoundaryY = -2.8f;
	public float boundaryHeight;

	void Awake()
	{
		/*if (Instance == null)
		{
			//DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}*/
	}

	// Use this for initialization
	void Start () 
	{
		// create left and right boundary objects
		GameObject leftBoundary = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/Boundary"), 
		                                       new Vector3 (leftBoundaryX, 0, verticalBoundaryY), Quaternion.identity);
		leftBoundary.name = "LeftBoundary";
		leftBoundary.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		//leftBoundary.layer = 8;

		GameObject rightBoundary = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/Boundary"), 
		                                                   new Vector3 (rightBoundaryX, 0, verticalBoundaryY), Quaternion.identity);
		rightBoundary.name = "RightBoundary";
		rightBoundary.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		//rightBoundary.layer = 8;

		_leftBoundary = leftBoundary;
		_rightBoundary = rightBoundary;

		// create kill box

		GameObject killBox = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/Boundary"), 
		                                                    new Vector3 (0, -10.0f, verticalBoundaryY), Quaternion.identity);
		killBox.name = "KillBox";
		killBox.collider.isTrigger = true;
		killBox.transform.localScale = new Vector3(20.0f, 1.0f, 10.0f);

		killBox.AddComponent<KillBox> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateVerticalBoundaries ();
	}

	void UpdateVerticalBoundaries()
	{
		var levelPlatforms = PlatformSpawnControl.Instance.levelPlatforms;
		if (levelPlatforms != null && levelPlatforms.Count > 0) 
		{
			GameObject highestPlatform = levelPlatforms [levelPlatforms.Keys.Max()];
			float highestPlatformY = highestPlatform.transform.position.y;
			//Debug.Log("Highest platform y: " + highestPlatformY);
			boundaryHeight = highestPlatformY + (highestPlatform.transform.localScale.y / 2);
			//Debug.Log(boundaryHeight);
			_leftBoundary.transform.localScale = new Vector3(_leftBoundary.transform.localScale.x, boundaryHeight, _leftBoundary.transform.localScale.z);
			_rightBoundary.transform.localScale = new Vector3(_rightBoundary.transform.localScale.x, boundaryHeight, _rightBoundary.transform.localScale.z);
			_leftBoundary.transform.position = new Vector3(_leftBoundary.transform.position.x, boundaryHeight / 2, _leftBoundary.transform.position.z);
			_rightBoundary.transform.position = new Vector3(_rightBoundary.transform.position.x, boundaryHeight / 2, _rightBoundary.transform.position.z);
		}
	}
}
