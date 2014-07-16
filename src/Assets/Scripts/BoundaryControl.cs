using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoundaryControl : MonoBehaviour {

	private GameObject _leftBoundary;
	private GameObject _rightBoundary;

	public float leftBoundaryX = -4.0f;
	public float rightBoundaryX = 4.0f;
	public float verticalBoundaryY = -2.8f;
	public float boundaryHeight;

	// Use this for initialization
	void Start () {

		GameObject leftBoundary = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/Boundary"), 
		                                       new Vector3 (leftBoundaryX, 0, verticalBoundaryY), Quaternion.identity);
		leftBoundary.name = "LeftBoundary";
		leftBoundary.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		GameObject rightBoundary = (GameObject)Instantiate (Resources.Load<GameObject> ("Prefabs/Boundary"), 
		                                                   new Vector3 (rightBoundaryX, 0, verticalBoundaryY), Quaternion.identity);
		rightBoundary.name = "RightBoundary";
		rightBoundary.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

		_leftBoundary = leftBoundary;
		_rightBoundary = rightBoundary;

	}
	
	// Update is called once per frame
	void Update () {
		UpdateVerticalBoundaries ();
	}

	void UpdateVerticalBoundaries()
	{
		var levelPlatforms = PlatformSpawnControl.Controller.levelPlatforms;
		if (levelPlatforms != null && levelPlatforms.Count > 0) 
		{
			GameObject highestPlatform = levelPlatforms [levelPlatforms.Count];
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
