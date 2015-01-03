using UnityEngine;
using System.Collections;

public class SkyboxCameraMovement : MonoBehaviour
{
    public Transform directionalLight;
    public static float speedMultiplier = 0.05f;
    public float rotationSpeed = 1f;
    public Skybox[] skyboxes;
    public bool rotateLight;
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
    void Start()
    {
        if (directionalLight == null)
        {
            Debug.LogError("No directional light set on Directional Light property of SkyboxCameraMovement.cs");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        if (rotateLight)
        {
            directionalLight.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        
    }

    private void HandleOnMaxHeightIncrease(float amount)
    {
        rotationSpeed = Mathf.Clamp(amount, 1f, maxRotationSpeed);
        //Debug.Log("Increased skybox rotation to: " + amount);
    }
}
