using UnityEngine;
using System.Collections;

public class LightBeam : MonoBehaviour {

	private LineRenderer _lineRenderer;
	private float _width;

    public float maxWidth;
	public float minWidth;
	public float flickerInterval = .1f;
	public float widthFluxInterval = .5f;
	public float transitionSpeed = 10f;

	void OnEnable()
	{
		PlatformController.On_NewPlatform += HandleOnNewPlatform;
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
		PlatformController.On_NewPlatform -= HandleOnNewPlatform;
	}
	
	// Use this for initialization
	IEnumerator Start () 
	{
		_lineRenderer = GetComponent<LineRenderer> ();
		_width = minWidth;

	    while (true) 
		{
			yield return StartCoroutine("Flucuate");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//var transition = Mathf.Lerp (_previousWidth, _width, transitionSpeed * Time.deltaTime);
		//_lineRenderer.SetWidth (transition, transition);
	}

	IEnumerator Flucuate()
	{
	    _width = Random.Range (minWidth, maxWidth);
		_lineRenderer.SetWidth (_width, _width);
		yield return new WaitForSeconds (widthFluxInterval);
	}

	void HandleOnNewPlatform(float newPlatformYPosition, float maxCameraY)
	{
		_lineRenderer.SetPosition (1, new Vector3 (0, newPlatformYPosition, transform.position.z));
		//Debug.Log ("Light beam height: " + newPlatformYPosition);
	}
}
