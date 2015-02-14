using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(AudioSource))]
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

	private AudioSource[] _audioSources;
	private AudioSource _openBus;
	private AudioSource _currentBus;
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
		return _currentTrackListing.FirstOrDefault (x => source.clip.name == x.clip.name);
	}

    void HandleOnMovePlayerToGamePosition (Vector3 playerPosition)
    {
		_audioSources [0].Play ();
		currentClipInfo = GetClipInfoFromAudioSource (_audioSources [0]);
		_audioSources [0].volume = PlayerState.Instance.Data.playMusic ? maxMusicVolume : 0;
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
		foreach (ClipInfo track in _currentTrackListing)
		{
			var newSource = gameObject.AddComponent<AudioSource>();
			newSource.clip = track.clip;
			newSource.playOnAwake = false;
			newSource.loop = true;
			newSource.volume = maxMusicVolume;
		}

		_audioSources = GetComponents<AudioSource> ();
		var lastIndex = _currentTrackListing.Length - 1;

		//forestMusicFast = transform.FindChild("ForestFast").audio;
        //forestMusicSlow.Play();
       	//stratosphereMusic.Play();
		_audioSources [lastIndex].Play ();
		currentClipInfo = GetClipInfoFromAudioSource (_audioSources [lastIndex]);
        this.ToggleMusic(PlayerState.Instance.Data.playMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIntroMusic) 
		{
			var lastIndex = _currentTrackListing.Length - 1;
			_audioSources [lastIndex].volume = Mathf.Lerp(_audioSources [lastIndex].volume, 0, musicFadeSpeed * Time.deltaTime);
			if (_audioSources [lastIndex].volume <= 0.01f){
				//stratosphereMusic.volume = 0;
				_audioSources [lastIndex].Stop();
				fadeIntroMusic = false;
			}
		}

		if (currentClipInfo.transitionType == MusicTransitionType.None) return; // if we've reached the end, loop forever

		var currentPlatform = GameController.Instance.highestPoint;

		foreach (ClipInfo track in _currentTrackListing)
		{
			if (currentPlatform > track.transitionPlatform 
			    && currentClipInfo.order == track.order)
			{
				 
				var nextClipInfo = _currentTrackListing[track.order];
				var currentBus = _audioSources[track.order - 1];
				var nextBus = _audioSources[track.order];
				//Debug.Log ("start transition to: " + nextClipInfo.clip.name);
				//openBus.clip = nextClipInfo.clip;
				currentBus.loop = false;
				switch(track.transitionType)
				{
					case MusicTransitionType.TrackEnd:
						if (!currentBus.isPlaying)
						{
							Debug.Log("Switch to " + nextClipInfo.clip.name);
							nextBus.loop = true;
							nextBus.volume = (PlayerState.Instance.Data.playMusic) ? maxMusicVolume : 0;
							nextBus.Play();							

							// trigger timed destroy
							StartOrStopTimedDestroy(nextClipInfo.destroySpeed);
							currentClipInfo = GetClipInfoFromAudioSource (nextBus);
						}
						break;

					case MusicTransitionType.Fade:
						
						if (PlayerState.Instance.Data.playMusic)
						{
							
							if (currentBus.isPlaying && !nextBus.isPlaying)
							{
								nextBus.Play();
								nextBus.loop = true;
								nextBus.volume = 0;
								Debug.Log("Trigger Fade in: " + nextBus.clip.name + " from: " + currentBus.clip.name);
								// trigger timed destroy
								StartOrStopTimedDestroy(nextClipInfo.destroySpeed);
							}

							if (nextBus.isPlaying && nextBus.volume < 0.5f && currentBus.isPlaying)
							{
								Debug.Log("Fading in: " + nextBus.clip.name + " from: " + currentBus.clip.name);
								nextBus.volume = Mathf.Lerp(nextBus.volume, maxMusicVolume, musicFadeSpeed * Time.deltaTime);
								currentBus.volume = Mathf.Lerp(currentBus.volume, 0f, musicFadeSpeed * Time.deltaTime);
								
								if (currentBus.volume <= 0.01f)
								{
									currentBus.Stop();
									currentClipInfo = GetClipInfoFromAudioSource(nextBus);
								}
							}	
						}
						else
						{
							Debug.Log("Fade mute switch to " + nextClipInfo.clip.name);
							nextBus.loop = true;
							nextBus.volume = 0;
							nextBus.Play();
							
							// trigger timed destroy
							StartOrStopTimedDestroy(nextClipInfo.destroySpeed);
							currentClipInfo = GetClipInfoFromAudioSource (nextBus);
						}
						break;

				}
			}
		}
    }

	private void StartOrStopTimedDestroy(float destroySpeed)
	{
		if (destroySpeed > 0 && OnFastMusicStart != null)
		{
			OnFastMusicStart(destroySpeed);
		}
		else if (destroySpeed == 0 && OnFastMusicStop != null)
		{
			OnFastMusicStop();
		}
	}

    void ToggleMusic(bool playMusic)
    {
        AudioSource currentSong = _audioSources.FirstOrDefault(x => x.isPlaying);
        if (currentSong != null)
        {
            currentSong.volume = playMusic ? maxMusicVolume : 0;
        }
    }
}



