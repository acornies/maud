using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{

    //private IDictionary<string, AudioSource> _allSongs;
	private bool fadeIntroMusic;

	/*private AudioSource forestMusicSlow;
	private AudioSource forestMusicFast;
	private AudioSource cloudMusicSlow;
	private AudioSource cloudMusicFast;
	private AudioSource stratosphereMusic;
	*/

	private AudioSource[] _soundBuses;
	private ClipInfo[] _currentTrackListing;
	public ClipInfo currentClipInfo;
	private bool _inTransition;

	public static MusicController Instance { get; private set;}

	public SoundPackInfo[] soundTracks;

	[Range(0, 1)]
    public float maxMusicVolume = 0.5f;
    
	public float musicFadeSpeed = 0.1f;
    /*public int forestMusicSlowLimit = 50;
    public int forestMusicFastLimit = 100;
    public int cloudMusicSlowLimit = 200;
    public int cloudMusicFastLimit = 300;
    public int stratosphereLimit = 500;

	public float forestMusicFastDestroySpeed = 1f;
	public float cloudMusicFastDestroySpeed = 1f;
	public float stratosphereMusicFastDestroySpeed = 1f;
	*/

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

	private ClipInfo GetClipInfoFromAudioSource(AudioSource source)
	{
		return _currentTrackListing.FirstOrDefault (x => source.clip == x.clip);
	}

    void HandleOnMovePlayerToGamePosition (Vector3 playerPosition)
    {
		_soundBuses [0].Play ();
		currentClipInfo = GetClipInfoFromAudioSource (_soundBuses [0]);
		_soundBuses [0].volume = PlayerState.Instance.Data.playMusic ? maxMusicVolume : 0;
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

		_soundBuses = GetComponents<AudioSource> ();

		/*_allSongs = GetComponents<AudioSource>().ToDictionary(s => s.clip.name);
        foreach (AudioSource source in _allSongs.Values)
        {
            source.playOnAwake = false;
        }

		forestMusicSlow = _allSongs["Jumpergame90bpm_280714"];
		forestMusicFast = _allSongs["Jumpergame_280714"];
        cloudMusicSlow = _allSongs["Cloudlevel121Bpm_280814"];
        cloudMusicFast = _allSongs["Cloudlevel141Bpm_280814"];
        stratosphereMusic = _allSongs["Stratosphere_w-tail_70bpm_281114"];
        */
    }

    // Use this for initialization
    void Start()
    {
        
		// assign first and last audio tracks to both buses using player selection
		var soundTrackSelection = PlayerState.Instance.Data.soundTrackSelection;
		_currentTrackListing = soundTracks [soundTrackSelection].clips.OrderBy (x => x.order).ToArray();
		_soundBuses [0].clip = _currentTrackListing[0].clip;
		_soundBuses [1].clip = _currentTrackListing[_currentTrackListing.Length - 1].clip;

		//forestMusicFast = transform.FindChild("ForestFast").audio;
        //forestMusicSlow.Play();
       	//stratosphereMusic.Play();
		_soundBuses [1].Play ();
		currentClipInfo = GetClipInfoFromAudioSource (_soundBuses [1]);
        this.ToggleMusic(PlayerState.Instance.Data.playMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIntroMusic) 
		{
			_soundBuses [1].volume = Mathf.Lerp(_soundBuses [1].volume, 0, musicFadeSpeed * Time.deltaTime);
			if (_soundBuses [1].volume <= 0.01f){
				//stratosphereMusic.volume = 0;
				_soundBuses [1].Stop();
				fadeIntroMusic = false;
			}
		}

		var currentPlatform = PlatformController.Instance.GetCurrentPlatformNumber();

		foreach (ClipInfo track in _currentTrackListing)
		{
			if (currentPlatform > track.transitionPlatform 
			    && currentClipInfo.order == track.order)
			{
				var nextClipInfo = _currentTrackListing.FirstOrDefault(x => x.order == track.order+1);
				var currentBus = _soundBuses.FirstOrDefault(x => x.clip == currentClipInfo.clip);
				var openBus = _soundBuses.FirstOrDefault(x => x.clip != currentClipInfo.clip);
				Debug.Log ("start transition to: " + nextClipInfo.clip.name);
				openBus.clip = nextClipInfo.clip;
				currentBus.loop = false;
				switch(track.transitionType)
				{
					case MusicTransitionType.TrackEnd:
						if (!currentBus.isPlaying)
						{
							Debug.Log("Switch to " + nextClipInfo.clip.name);
							openBus.loop = true;
							openBus.volume = (PlayerState.Instance.Data.playMusic) ? maxMusicVolume : 0;
							openBus.Play();
							currentClipInfo = GetClipInfoFromAudioSource (openBus);

							// trigger timed destroy
							if (nextClipInfo.destroySpeed > 0 && OnFastMusicStart != null)
							{
								OnFastMusicStart(nextClipInfo.destroySpeed);
							}
						}
						break;
					case MusicTransitionType.Fade:
						break;
				}
			}
		}

        /*NextSong(currentPlatform, forestMusicSlowLimit, forestMusicSlow, forestMusicFast);
        FadeTransition(currentPlatform, forestMusicFastLimit, forestMusicFast, cloudMusicSlow);
        NextSong(currentPlatform, cloudMusicSlowLimit, cloudMusicSlow, cloudMusicFast);
        FadeTransition(currentPlatform, cloudMusicFastLimit, cloudMusicFast, stratosphereMusic);
        */
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
		/*if (OnFastMusicStart != null && nextSong == forestMusicFast && GameController.Instance.gameState != LegendPeak.GameState.Over)
		{
			OnFastMusicStart(forestMusicFastDestroySpeed);
		}
		else if (OnFastMusicStart != null && nextSong == cloudMusicFast && GameController.Instance.gameState != LegendPeak.GameState.Over)
		{
			OnFastMusicStart(cloudMusicFastDestroySpeed);
		}*/

        currentSong.volume = 0.0f;
        currentSong.Stop();
        nextSong.volume = (PlayerState.Instance.Data.playMusic) ? maxMusicVolume : 0;
    }

    void ToggleMusic(bool playMusic)
    {
        AudioSource currentSong = _soundBuses.FirstOrDefault(x => x.isPlaying);
        if (currentSong != null)
        {
            currentSong.volume = playMusic ? maxMusicVolume : 0;
        }
    }
}

[Serializable]
public class SoundPackInfo
{
	public ClipInfo[] clips;
	public string identifier;
}

[Serializable]
public class ClipInfo
{
	public AudioClip clip;
	public float destroySpeed;
	public int transitionPlatform;
	public MusicTransitionType transitionType;
	public int order;
}

[Serializable]
public enum MusicTransitionType
{
	TrackEnd = 0,
	Fade = 1
}
