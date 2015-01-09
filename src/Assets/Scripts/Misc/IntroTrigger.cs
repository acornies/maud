using UnityEngine;
using System.Collections;

public class IntroTrigger : MonoBehaviour
{
    private Animator _animator;
	private bool _canSkip = true;
    
    public Vector3 introLedgePosition;
	public float skipIntroSpeed = 10f;
	public float startSpeed = 1.5f;

    public delegate void NewIntroLedgePosition(Vector3 newPosition);
    public static event NewIntroLedgePosition OnNewIntroLedgePosition;


    public delegate void ZoomToGamePosition();
    public static event ZoomToGamePosition OnZoomToGamePosition;

    void OnEnable()
    {
        GameController.OnGameStart += HandleOnGameStart;
		EasyTouch.On_SimpleTap += HandleOnSimpleTap;
    }

	private void HandleOnGameStart()
	{
		_canSkip = false;
		_animator.speed = startSpeed;
		_animator.SetBool("started", true);
	}

	private void HandleOnSimpleTap(Gesture gesture)
    {
		if (_canSkip)
		{
			_animator.speed = skipIntroSpeed;
		}
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
		EasyTouch.On_SimpleTap -= HandleOnSimpleTap;
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

        gameObject.SetActive(false);
    }
}
