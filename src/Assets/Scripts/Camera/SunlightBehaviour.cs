using UnityEngine;
using System.Collections;

public class SunlightBehaviour : MonoBehaviour {

	public float rotationSpeed = 1f;
	public float maxRotationSpeed = 100f;

	void OnEnable()
	{
		GameController.OnMaxHeightIncrease += HandleOnMaxHeightIncrease;
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
		GameController.OnMaxHeightIncrease -= HandleOnMaxHeightIncrease;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.Self);

	}

	private void HandleOnMaxHeightIncrease(int delta, float amount)
	{
		rotationSpeed = Mathf.Clamp(amount, 1f, maxRotationSpeed);
		//Debug.Log("Increased skybox rotation to: " + amount);
	}
}
