using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{

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
    void Start()
    {
        forestMusicFast = transform.FindChild("ForestFast").audio;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        int currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber();

        if (currentPlatform < forestMusicSlowLimit || !audio.isPlaying) return;

        if (Mathf.Approximately(audio.time, audio.clip.length))
        {
            MusicTransition(audio, forestMusicFast);
        }
    }

    static void MusicTransition(AudioSource currentSong, AudioSource nextSong)
    {
        Debug.Log("Start transition.");
        nextSong.Play();
        currentSong.volume = 0.0f;
        currentSong.Stop();
        nextSong.volume = 0.5f;
    }
}
