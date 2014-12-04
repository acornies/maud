using UnityEngine;
using System.Collections;

public class IntroLedge : MonoBehaviour
{
    private Vector3 _introLocation = Vector3.zero;

    public delegate void ShowPlayButton();
    public static event ShowPlayButton OnShowPlayButton;
    
    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {
        IntroTrigger.OnNewIntroLedegePosition += OnNewIntroLedegePosition;
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
        IntroTrigger.OnNewIntroLedegePosition -= OnNewIntroLedegePosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnNewIntroLedegePosition(Vector3 newPosition)
    {
        _introLocation = newPosition;
        transform.position = newPosition;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name != "Player" || _introLocation == Vector3.zero) return;

        if (OnShowPlayButton != null)
        {
            OnShowPlayButton();
        }
    }
}
