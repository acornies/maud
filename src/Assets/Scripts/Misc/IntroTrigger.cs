using UnityEngine;
using System.Collections;

public class IntroTrigger : MonoBehaviour
{
    private Animator _animator;
    
    public Vector3 introLedgePosition;

    public delegate void NewIntroLedgePosition(Vector3 newPosition);
    public static event NewIntroLedgePosition OnNewIntroLedgePosition;


    public delegate void ZoomToGamePosition();
    public static event ZoomToGamePosition OnZoomToGamePosition;

    void OnEnable()
    {
        GameController.OnGameStart += HandleOnGameStart;
    }

    private void HandleOnGameStart()
    {
        _animator.SetBool("started", true);
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
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TriggerNewPosition()
    {
        if (OnNewIntroLedgePosition != null)
        {
            OnNewIntroLedgePosition(introLedgePosition);
        }
    }

    public void TriggerCameraZoom()
    {
        if (OnZoomToGamePosition != null)
        {
            OnZoomToGamePosition();
        }
    }
}
