﻿using System.Linq;
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

    public AudioSource forestMusicSlow;
    public AudioSource forestMusicFast;
    public AudioSource cloudMusicSlow;
    public AudioSource cloudMusicFast;

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
        _allSongs = GetComponents<AudioSource>().ToDictionary(s => s.clip.name);
        foreach (AudioSource source in _allSongs.Values)
        {
            source.playOnAwake = false;
        }

        forestMusicSlow = _allSongs["Jumpergame90bpm_280714"];
        forestMusicFast = _allSongs["Jumpergame_280714"];
        cloudMusicSlow = _allSongs["Cloudlevel121Bpm_280814"];
        cloudMusicFast = _allSongs["Cloudlevel141Bpm_280814"];
    }

    // Use this for initialization
    void Start()
    {
        //forestMusicFast = transform.FindChild("ForestFast").audio;
        forestMusicSlow.Play();
    }

    // Update is called once per frame
    void Update()
    {
        var currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber();
        NextSong(currentPlatform, forestMusicSlowLimit, forestMusicSlow, forestMusicFast);
        NextSong(currentPlatform, forestMusicFastLimit, forestMusicFast, cloudMusicSlow);
        NextSong(currentPlatform, cloudMusicSlowLimit, cloudMusicSlow, cloudMusicFast);
    }

    static void NextSong(int currentPlatform, int limit, AudioSource current, AudioSource next)
    {
        //Debug.Log("Current song is");
        if (currentPlatform < limit || !current.isPlaying) return;
        current.loop = false;
        Debug.Log("Try to play " + next.clip.name);
        Debug.Log("current: " + current.time + " clip length: " + current.clip.length);
        if (Mathf.Approximately(current.time, current.clip.length))
        {
            MusicTransition(current, next);
        }
		else if ( !current.isPlaying && !next.isPlaying ) {
			Debug.Log ("It was silent.");
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
