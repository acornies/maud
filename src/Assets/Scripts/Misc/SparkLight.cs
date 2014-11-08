using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class SparkLight : MonoBehaviour {

	private bool _shouldDim;
	private Light _sparkLight;

	public float dimSpeed = 2f;
	public float lightIntensity = 0.7f;

	// Subscribe to events
	void OnEnable()
	{
		TelekinesisController.On_PlayerPowersStart += HandleOnPlayerPowersStart;
	}

	void HandleOnPlayerPowersStart ()
	{
		_sparkLight.intensity = lightIntensity;
		_shouldDim = true;
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
		TelekinesisController.On_PlayerPowersStart -= HandleOnPlayerPowersStart;
	}

	// Use this for initialization
	void Start () {
		_sparkLight = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		if (_shouldDim){
			_sparkLight.intensity = Mathf.Lerp(_sparkLight.intensity, 0, dimSpeed * Time.deltaTime);
			if (_sparkLight.intensity == 0){
				_shouldDim = false;
			}
		}
	}
}
