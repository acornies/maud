﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EveryplayController : MonoBehaviour
{
	private Image _recordButtonImage;
	private Button _recordButtonBehaviour;
	private bool _serviceReady;
	
	public static EveryplayController Instance { get; private set; }
    
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
    }

    void OnEnable()
    {
        // Register for the Everyplay ReadyForRecording event
		//IntroLedge.OnShowMenuButtons += ToggleRec
		Everyplay.ReadyForRecording += HandleOnReadyForRecording;
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
    }
    
    // Use this for initialization
    void Start()
    {
        //_isSupported = Everyplay.IsRecordingSupported();
		_recordButtonBehaviour.interactable = isReady;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void HandleOnReadyForRecording(bool serviceReady)
    {
        //GameObject.Find("EveryplayDebug").GetComponent<Text>().text = "Support";
		_serviceReady = serviceReady;
		_recordButtonBehaviour.interactable = serviceReady && Everyplay.IsRecordingSupported();
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
		}

		//GameObject.Find ("EveryplayDebug").GetComponent<Text> ().text = isRecording.ToString ();
	}

	public void StopRecording()
	{
		Everyplay.StopRecording ();
		Everyplay.SetMetadata ("height_count", GameController.Instance.highestPoint);
		Everyplay.ShowSharingModal();
	}

}
