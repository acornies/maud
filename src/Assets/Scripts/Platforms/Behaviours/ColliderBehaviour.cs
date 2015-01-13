using UnityEngine;
using System.Collections;

public class ColliderBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//collider.enabled = renderer.isVisible;
	}

	void OnBecameVisible()
	{
		//Debug.Log (transform.parent.name + " cube became visible.");
		collider.enabled = true;
	}

	void OnBecameInvisible()
	{
		//Debug.Log (transform.parent.name + " cube became invisible.");
		collider.enabled = false;
	}
}
