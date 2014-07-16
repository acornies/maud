using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	// Subscribe to events
	void OnEnable()
	{
		//CameraMovement.On_CameraUpdatedMinY += UpdateKillBoxPosition;
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
		//CameraMovement.On_CameraUpdatedMinY -= UpdateKillBoxPosition;
	}

	void Awake ()
	{
		//Application.targetFrameRate = 30;
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
