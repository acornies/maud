using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EveryplayController : MonoBehaviour
{
    public static EveryplayController Instance { get; private set; }
    
    public bool isReady;

    void Awake()
    {
        //_isSupported = Everyplay.IsRecordingSupported();
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // Register for the Everyplay ReadyForRecording event
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
        Everyplay.ReadyForRecording += HandleOnReadyForRecording;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleOnReadyForRecording(bool serviceReady)
    {
        //Debug.Log("Everyplay ready? " + isReady);
        GameObject.Find("EveryplayDebug").GetComponent<Text>().text = serviceReady.ToString();
        this.isReady = serviceReady && Everyplay.IsRecordingSupported();
    }

}
