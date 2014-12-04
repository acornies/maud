using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{

    private IDictionary<string, AudioSource> _allSongs;

    public float musicFadeSpeed = 0.1f;
    public int forestMusicSlowLimit = 50;
    public int forestMusicFastLimit = 100;
    public int cloudMusicSlowLimit = 200;
    public int cloudMusicFastLimit = 300;
    public int stratosphereLimit = 500;

    private AudioSource forestMusicSlow;
    private AudioSource forestMusicFast;
    private AudioSource cloudMusicSlow;
    private AudioSource cloudMusicFast;
    private AudioSource stratosphereMusic;

    //public AudioClip deathSound;

    // Subscribe to events
    void OnEnable()
    {
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
    }

    void Awake()
    {
        _allSongs = GetComponents<AudioSource>().ToDictionary(s => s.clip.name);
        foreach (AudioSource source in _allSongs.Values)
        {
            source.playOnAwake = false;
        }

        forestMusicSlow = _allSongs["Jumpergame90bpm_280714"];
        forestMusicFast = _allSongs["Jumpergame_280714"];
        cloudMusicSlow = _allSongs["Cloudlevel121Bpm_280814"];
        cloudMusicFast = _allSongs["Cloudlevel141Bpm_280814"];
        stratosphereMusic = _allSongs["Stratosphere_w-tail_70bpm_281114"];
    }

    // Use this for initialization
    void Start()
    {
        //forestMusicFast = transform.FindChild("ForestFast").audio;
        //forestMusicSlow.Play();
        stratosphereMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        var currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber();
        NextSong(currentPlatform, forestMusicSlowLimit, forestMusicSlow, forestMusicFast);
        FadeTransition(currentPlatform, forestMusicFastLimit, forestMusicFast, cloudMusicSlow);
        NextSong(currentPlatform, cloudMusicSlowLimit, cloudMusicSlow, cloudMusicFast);
        FadeTransition(currentPlatform, cloudMusicFastLimit, cloudMusicFast, stratosphereMusic);
    }

	void FadeTransition(int currentPlatform, int limit, AudioSource current, AudioSource next)
	{
		if (currentPlatform > limit && current.isPlaying && !next.isPlaying) {
			next.volume = 0f;
			next.Play();
			next.loop = true;
		}

		if (next.isPlaying && next.volume < 0.5f && current.isPlaying)
		{
			Debug.Log("Fade in: " + next.clip.name + " from: " + current.clip.name);
			next.volume = Mathf.Lerp(next.volume, 0.5f, musicFadeSpeed * Time.deltaTime);
			current.volume = Mathf.Lerp(current.volume, 0f, musicFadeSpeed * Time.deltaTime);

			if (current.volume <= 0.01f)
			{
				current.Stop();
			}
		}
	}

    private static void NextSong(int currentPlatform, int limit, AudioSource current, AudioSource next)
    {
        if (currentPlatform > limit && current.isPlaying)
        {
            current.loop = false;
        }
        else if (!current.isPlaying && !current.loop)
        {
            current.loop = true;
            MusicTransition(current, next);
        }
    }

    static void MusicTransition(AudioSource currentSong, AudioSource nextSong)
    {
        if (nextSong == null) return;
        Debug.Log("Start transition from " + currentSong.clip.name + " to " + nextSong.clip.name);
        nextSong.Play();
        currentSong.volume = 0.0f;
        currentSong.Stop();
        nextSong.volume = 0.5f;
    }
}
