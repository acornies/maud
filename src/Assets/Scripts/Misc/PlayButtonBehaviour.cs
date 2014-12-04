
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayButtonBehaviour : MonoBehaviour
{
    private Image _buttonImage; 
    void Awake()
    {
        _buttonImage = GetComponent<Image>();
    }
    
    void OnEnable()
    {
        IntroLedge.OnShowPlayButton += HandleOnShowPlayButton;
    }

    private void HandleOnShowPlayButton()
    {
        Debug.Log("Show play button");
        _buttonImage.enabled = true;
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
        IntroLedge.OnShowPlayButton += HandleOnShowPlayButton;
    }
    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
