using UnityEngine;
using System.Collections;

public class SkyboxCameraMovement : MonoBehaviour
{
    public static float speedMultiplier = 0.02f;
    public float rotationSpeed = 1f;

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
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); //rotates 50 degrees per second around z axis
    }

    private void HandleOnMaxHeightIncrease(float amount)
    {
        rotationSpeed = amount;
        //Debug.Log("Increased skybox rotation to: " + amount);
    }
}
