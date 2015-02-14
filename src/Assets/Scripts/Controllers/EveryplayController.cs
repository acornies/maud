﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EveryplayController : MonoBehaviour
{
	private Image _recordButtonImage;
	private Button _recordButtonBehaviour;
	private bool _serviceReady;
	private Image _recIndicator;

	public static EveryplayController Instance { get; private set; }
    
	public Sprite recordStop;
	public Sprite recordCapture;
	public Sprite cameraImage;
    public bool isReady 
	{
		get
		{
			return (_serviceReady || Everyplay.IsRecordingSupported());
		}
	}
	public bool isRecording 
	{
		get
		{ 
			return Everyplay.IsRecording(); 
		}
	}
	/*public bool isPaused
	{
		get 
		{
			return Everyplay.IsPaused();
		}
	}*/

    void Awake()
    {
        //_isSupported = Everyplay.IsRecordingSupported();
        if (Instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

		var recordButton = GameObject.Find ("RecordButton");
		_recordButtonImage = recordButton.GetComponent<Image>();
		_recordButtonBehaviour = recordButton.GetComponent<Button>();
		_recIndicator = GameObject.Find ("RecIndicator").GetComponent<Image>();
    }

    void OnEnable()
    {
        // Register for the Everyplay ReadyForRecording event
		//IntroLedge.OnShowMenuButtons += ToggleRec
		Everyplay.ReadyForRecording += HandleOnReadyForRecording;
		GameController.OnGameRestart += HandleOnGameRestart;
    }

    void HandleOnGameRestart (int sceneIndex)
    {
		_recordButtonBehaviour.interactable = false;
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
        Everyplay.ReadyForRecording -= HandleOnReadyForRecording;
		GameController.OnGameRestart -= HandleOnGameRestart;
    }
    
    // Use this for initialization
    void Start()
    {
        //_isSupported = Everyplay.IsRecordingSupported();
		//_recordButtonBehaviour.interactable = isReady;
		VideoOrStillCapture ();
    }

    // Update is called once per frame
    void OnGUI()
    {
		if (this.isRecording && !_recIndicator.enabled)
		{
			_recIndicator.enabled = true;
			_recordButtonImage.sprite = recordStop;
		}
		else if (!this.isRecording && _recIndicator.enabled)
		{
			_recIndicator.enabled = false;
			_recordButtonImage.sprite = recordCapture;
		}
    }

    void HandleOnReadyForRecording(bool serviceReady)
    {
        //GameObject.Find("EveryplayDebug").GetComponent<Text>().text = "Support";
		_serviceReady = serviceReady;
		_recordButtonBehaviour.interactable = serviceReady && Everyplay.IsRecordingSupported();
    }

	private void VideoOrStillCapture()
	{
		switch (Application.platform)
		{
		
			case RuntimePlatform.IPhonePlayer:
				_recordButtonBehaviour.interactable = isReady;
				_recordButtonBehaviour.onClick.AddListener(ButtonRecord);
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.Android:
				_recordButtonImage.sprite = cameraImage;
				_recordButtonBehaviour.interactable = true;
				_recordButtonBehaviour.onClick.AddListener(ButtonScreenshot);
				break;
		}
	}

	public void ButtonRecord()
	{
		if (isRecording) 
		{
			StopRecording();
		}
		/*else if (isPaused)
		{
			Everyplay.ResumeRecording();
		}*/
		else
		{
			Everyplay.StartRecording();
			//_recIndicator.enabled = true;
		}

		//GameObject.Find ("EveryplayDebug").GetComponent<Text> ().text = isRecording.ToString ();
	}

	public void ButtonScreenshot()
	{
		StoreController.Instance.Native.screenCapture ();
	}

	public void StopRecording()
	{
		Everyplay.StopRecording ();
		Everyplay.SetMetadata ("height_count", GameController.Instance.highestPoint);
		Everyplay.ShowSharingModal();
	}

}
