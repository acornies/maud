using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class SkyboxCameraMovement : MonoBehaviour
{   
    public Transform directionalLight;
    public static float speedMultiplier = 0.02f;
    public float rotationSpeed = 1f;
    public Material[] skyboxes;
    public bool rotateLight;
    public float maxRotationSpeed = 100f;

    public int dawnSkyboxTransitionPlatform = 61;
    public int cloudSkyboxTransitionPlatform = 150;
    public float transitionSpeed = 3f;
    //public int aboveSkyboxTransitionPlatform = 150;

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
        RenderSettings.skybox = skyboxes[0];
        RenderSettings.skybox.SetFloat("_Blend",0);
        
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
            directionalLight.Rotate(0, (-rotationSpeed * 2) * Time.deltaTime, 0);
        }

        if (PlatformController.Instance.GetCurrentPlatformNumber() >= dawnSkyboxTransitionPlatform)
        {
            var blend = Mathf.Lerp(RenderSettings.skybox.GetFloat("_Blend"), 1f, transitionSpeed * Time.deltaTime);
            RenderSettings.skybox.SetFloat("_Blend", blend);
            //TODO: transition/change directional light
        }

    }

    private void HandleOnMaxHeightIncrease(float amount)
    {
        rotationSpeed = Mathf.Clamp(amount, 1f, maxRotationSpeed);
        //Debug.Log("Increased skybox rotation to: " + amount);
    }
}
