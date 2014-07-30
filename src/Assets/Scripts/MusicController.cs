using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour {
	
	public float musicFadeSpeed = 0.1f;
	public int forestMusicSlowLimit = 50;
	public int forestMusicFastLimit = 100;
	public AudioSource forestMusicSlow;
	public AudioSource forestMusicFast;

	// Subscribe to events
	void OnEnable()
	{
		 //PlayerMovement.On_PlatformReached += HandlePlatformReached;
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
		//PlayerMovement.On_PlatformReached -= HandlePlatformReached;
	}

	void Awake()
	{
		forestMusicSlow = audio;
	}

	// Use this for initialization
	void Start () {
		forestMusicFast = transform.FindChild("ForestFast").audio;
	}
	
	// Update is called once per frame
	void Update () {
		int currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber();

		if (currentPlatform >= forestMusicSlowLimit)
		{
			audio.volume = Mathf.Lerp(audio.volume, 0.0f, musicFadeSpeed * Time.deltaTime);
			forestMusicFast.volume = Mathf.Lerp(forestMusicFast.volume, 0.5f, musicFadeSpeed * Time.deltaTime);
		}

	}
}
