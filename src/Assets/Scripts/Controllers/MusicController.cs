using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{

    private IDictionary<string, AudioSource> _allSongs;
	private bool fadeIntroMusic;
	private AudioSource forestMusicSlow;
	private AudioSource forestMusicFast;
	private AudioSource cloudMusicSlow;
	private AudioSource cloudMusicFast;
	private AudioSource stratosphereMusic;

	public static MusicController Instance { get; private set;}
	[Range(0, 1)]
    public float maxMusicVolume = 0.5f;
    
	public float musicFadeSpeed = 0.1f;
    public int forestMusicSlowLimit = 50;
    public int forestMusicFastLimit = 100;
    public int cloudMusicSlowLimit = 200;
    public int cloudMusicFastLimit = 300;
    public int stratosphereLimit = 500;

	public float forestMusicFastDestroySpeed = 1f;
	public float cloudMusicFastDestroySpeed = 1f;
	public float stratosphereMusicFastDestroySpeed = 1f;

	public delegate void FastMusicStart(float timedSpeed);
	public static event FastMusicStart OnFastMusicStart;

	public delegate void FastMusicStop();
	public static event FastMusicStop OnFastMusicStop;

    //public AudioClip deathSound;

    // Subscribe to events
    void OnEnable()
    {
		GameController.OnGameStart += HandleOnGameStart;
		CameraMovement.OnMovePlayerToGamePosition += HandleOnMovePlayerToGamePosition;
        GameController.OnToggleMusic += ToggleMusic;
    }

    void HandleOnMovePlayerToGamePosition (Vector3 playerPosition)
    {
        forestMusicSlow.Play ();
        forestMusicSlow.volume = PlayerState.Instance.Data.playMusic ? maxMusicVolume : 0;
    }

    void HandleOnGameStart ()
    {
		fadeIntroMusic = true;
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
		GameController.OnGameStart -= HandleOnGameStart;
		CameraMovement.OnMovePlayerToGamePosition -= HandleOnMovePlayerToGamePosition;
        GameController.OnToggleMusic -= ToggleMusic;
    }

    void Awake()
    {
		if (Instance == null)
		{
			//DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		_allSongs = GetComponents<AudioSource>().ToDictionary(s => s.clip.name);
        foreach (AudioSource source in _allSongs.Values)
        {
            source.playOnAwake = false;
        }

        forestMusicSlow = _allSongs["Sunset_Theme_1_loop"];
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
        this.ToggleMusic(PlayerState.Instance.Data.playMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIntroMusic) 
		{
			stratosphereMusic.volume = Mathf.Lerp(stratosphereMusic.volume, 0, musicFadeSpeed * Time.deltaTime);
			if (stratosphereMusic.volume <= 0.01f){
				//stratosphereMusic.volume = 0;
				stratosphereMusic.Stop();
				fadeIntroMusic = false;
			}
		}

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
		    if (OnFastMusicStop != null)
		    {
		        OnFastMusicStop();
		    }
		}

		if (next.isPlaying && next.volume < 0.5f && current.isPlaying)
		{
			//Debug.Log("Fade in: " + next.clip.name + " from: " + current.clip.name);
            next.volume = Mathf.Lerp(next.volume, maxMusicVolume, musicFadeSpeed * Time.deltaTime);
			current.volume = Mathf.Lerp(current.volume, 0f, musicFadeSpeed * Time.deltaTime);

			if (current.volume <= 0.01f)
			{
				current.Stop();
			}
		}
	}

    private void NextSong(int currentPlatform, int limit, AudioSource current, AudioSource next)
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

    void MusicTransition(AudioSource currentSong, AudioSource nextSong)
    {
        if (nextSong == null) return;
        //Debug.Log("Start transition from " + currentSong.clip.name + " to " + nextSong.clip.name);
        nextSong.Play();
		if (OnFastMusicStart != null && nextSong == forestMusicFast && GameController.Instance.gameState != LegendPeak.GameState.Over)
		{
			OnFastMusicStart(forestMusicFastDestroySpeed);
		}
		else if (OnFastMusicStart != null && nextSong == cloudMusicFast && GameController.Instance.gameState != LegendPeak.GameState.Over)
		{
			OnFastMusicStart(cloudMusicFastDestroySpeed);
		}

        currentSong.volume = 0.0f;
        currentSong.Stop();
        nextSong.volume = (PlayerState.Instance.Data.playMusic) ? maxMusicVolume : 0;
    }

    void ToggleMusic(bool playMusic)
    {
        AudioSource currentSong = _allSongs.Values.FirstOrDefault(x => x.isPlaying);
        if (currentSong != null)
        {
            currentSong.volume = playMusic ? maxMusicVolume : 0;
        }
    }
}
