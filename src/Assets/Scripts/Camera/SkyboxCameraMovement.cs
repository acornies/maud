using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class SkyboxCameraMovement : MonoBehaviour
{
    private Light _sunlightComponent;
    
    public Transform sunlightDirectionalLight;
    public static float speedMultiplier = 0.02f;
    public float rotationSpeed = 1f;
    public Material[] skyboxes;
    public float maxRotationSpeed = 100f;
	public float sunlightHorizontalRotation = 1f;

    public int dawnSkyboxTransitionPlatform = 61;
    public int cloudSkyboxTransitionPlatform = 150;
    public float transitionSpeed = 3f;

	public Material playerCloudMaterial;
	public Material playerAboveCloudMaterial;
    public Color sunlightColorDawn;
    public Color sunlightColorAboveClouds;
    //public int aboveSkyboxTransitionPlatform = 150;

	public delegate void PlayerMaterialUpdate(Material newMaterial);
	public static event PlayerMaterialUpdate OnPlayerMaterialUpdate;

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
        RenderSettings.skybox.SetFloat("_Blend", 0);

        if (sunlightDirectionalLight == null)
        {
            Debug.LogError("No directional light set on Sunlight Directional Light property of SkyboxCameraMovement.cs");
        }
        else
        {
            _sunlightComponent = sunlightDirectionalLight.GetComponent<Light>();
        }
    }

    // Update is called once per frame
    void Update()
    {
		transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        // rotate light opposite of skybox camera, x2 speed since light is rotating due to it's parent
		//sunlightDirectionalLight.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0), Space.Self);

        var currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber();
        SkyboxTransition(0, currentPlatform, dawnSkyboxTransitionPlatform, sunlightColorAboveClouds, playerCloudMaterial);
        SkyboxTransition(1, currentPlatform, cloudSkyboxTransitionPlatform, sunlightColorAboveClouds, playerAboveCloudMaterial);

    }

	void SkyboxTransition (int currentSkybox, int currentPlatform, int transitionPlatform, Color sunlightColor, Material playerMaterial)
    {
        if (currentPlatform < transitionPlatform) return;

        if (RenderSettings.skybox != skyboxes[currentSkybox]) return;

        var blend = Mathf.Lerp(RenderSettings.skybox.GetFloat("_Blend"), 1f, transitionSpeed * Time.deltaTime);
        RenderSettings.skybox.SetFloat("_Blend", blend);

        _sunlightComponent.color = Color.Lerp(_sunlightComponent.color, sunlightColor, transitionSpeed*Time.deltaTime);
		sunlightDirectionalLight.rotation = Quaternion.Lerp (sunlightDirectionalLight.rotation, Quaternion.Euler(40f, sunlightDirectionalLight.rotation.y, sunlightDirectionalLight.rotation.z), transitionSpeed * Time.deltaTime);
		//sunlightDirectionalLight.localEulerAngles.y = 0;
		//sunlightDirectionalLight.localEulerAngles.z = 0;
		//Debug.Log ("Tilt sunlight rotation...");



        var nextSkybox = (currentSkybox + 1);
        if (!(RenderSettings.skybox.GetFloat("_Blend") >= 0.99f) || nextSkybox > (skyboxes.Length - 1)) return;

		if (OnPlayerMaterialUpdate != null)
		{
			OnPlayerMaterialUpdate(playerMaterial);
		}

       // Debug.Log("Set RenderSettings to " + nextSkybox + " skybox");
        RenderSettings.skybox = skyboxes[nextSkybox];
        RenderSettings.skybox.SetFloat("_Blend", 0);
    }

    private void HandleOnMaxHeightIncrease(float amount)
    {
        rotationSpeed = Mathf.Clamp(amount, 1f, maxRotationSpeed);
        //Debug.Log("Increased skybox rotation to: " + amount);
    }
}
