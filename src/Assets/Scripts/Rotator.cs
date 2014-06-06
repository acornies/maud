using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	public Transform platform;

	public float sensitivity = 0.4f;

	private Vector3 _pointerReference;
	private Vector3 _pointerOffset;
	private Vector3 _rotation = Vector3.zero;
	private bool _isRotating;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{	
		SelectPlatform();

		if(_isRotating && this.platform != null)
		{
			// offset
			_pointerOffset = (Input.mousePosition - _pointerReference);
			
			// apply rotation
			_rotation.y = -(_pointerOffset.x + _pointerOffset.y) * sensitivity;
			
			// rotate
			platform.Rotate(_rotation);
			
			// store mouse
			_pointerReference = Input.mousePosition;
			Debug.Log(platform.gameObject.name + " is rotating.");
		}
	}

	private void SelectPlatform()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
			if (hit) 
			{
				Debug.Log("Hit " + hitInfo.transform.gameObject.name);
				if (hitInfo.transform.gameObject.tag == "Rotatable")
				{
					//Debug.Log ("It's active");
					this.platform = hitInfo.transform;
				}
				if (hitInfo.transform.gameObject.tag == "Stoppable")
				{
					Debug.Log("Hit child, rotate parent: " + hitInfo.transform.parent.name);
					this.platform = hitInfo.transform.parent;
				}
			}
		}
	}

	void OnMouseDown()
	{
		// rotating flag
		_isRotating = true;
		
		// store mouse
		_pointerReference = Input.mousePosition;
	}

	void OnMouseUp()
	{
		// rotating flag
		_isRotating = false;
	}
}
