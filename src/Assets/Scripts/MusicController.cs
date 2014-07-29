using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour {

	//private PlatformController _platformController;
	public float musicFadeSpeed = 0.1f;
	public int forestMusicSlowLimit = 50;
	//public int forestMusicFastLimit = 50;
	//public AudioSource forestMusicSlow;
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
	}

	// Use this for initialization
	void Start () {
		//_platformController = PlatformController.Instance;
		forestMusicFast = transform.FindChild("ForestFast").audio;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (_platformController.GetCurrentPlatformNumber());
		int currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber();
		//Debug.Log(audio.time);

		if (currentPlatform >= forestMusicSlowLimit)
		{
			//forestMusicFast.Play();
			audio.volume = Mathf.Lerp(audio.volume, 0.0f, musicFadeSpeed * Time.deltaTime);
			//audio.volume = 0.0f;
			//forestMusicFast.volume = 0.5f;
			//Debug.Log(audio.clip.length);

			forestMusicFast.volume = Mathf.Lerp(forestMusicFast.volume, 0.5f, musicFadeSpeed * Time.deltaTime);
		}

	}
}
