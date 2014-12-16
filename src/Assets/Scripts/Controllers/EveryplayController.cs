using UnityEngine;
using System.Collections;

public class EveryplayController : MonoBehaviour
{
    public bool isReady;
    private bool _isSupported;

    void Awake()
    {
        //_isSupported = Everyplay.IsRecordingSupported();
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

    public void HandleOnReadyForRecording(bool isReady)
    {
        //Debug.Log("Everyplay ready? " + isReady);
        this.isReady = isReady;
    }

}
