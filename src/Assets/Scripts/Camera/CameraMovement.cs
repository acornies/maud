using System.Linq;
using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    private Transform _playerTarget;
    private bool _zoomToGame;
    private float _zoomTimer;
	private bool _shouldZoomOut;
	private float _timedDestroyZoomTimer;
	private float _previousMinY;
	private float _previousCameraY;

	//HACK
	private bool _playerSuspended; 

	//HACK
	private float _playerVisibleTimer;
	private bool _playerIsVisible;

	public float playerVisibleTime = 1f;
	public float zoomWarmTime = 0.5f;
    public Vector3 gameCameraPosition;
    public float zoomSpeed = 10f;
	public float ghostZoomDepth = -25f;
	public float timedDestroyZoomDepth = -40f;
    public bool isTracking;
    public float XMargin = 1.0f;
    public float YMargin = 1.0f;
    public float XSmooth = 10.0f;
    public float YSmooth = 10.0f;
    public int minCameraUpdatePlatform = 5;
    public Vector2 MaxXandY;
    public Vector2 MinXandY;
    public Transform CameraTarget;
    public static float defaultCameraSpeed = 4.5f;
	public float timedDestroyZoomTime = 3f;
	public bool isTimedDestroyCutscene;
	
    public delegate void UpdatedCameraMinY(float yPosition);
    public static event UpdatedCameraMinY On_CameraUpdatedMinY;

    public delegate void DestroyLowerPlatforms(int platformNumber);
    public static event DestroyLowerPlatforms On_DestroyLowerPlatforms;

    public delegate void MovePlayerToGamePosition(Vector3 playerPosition);
    public static event MovePlayerToGamePosition OnMovePlayerToGamePosition;

	public delegate void RestorePlayerState();
	public static event RestorePlayerState OnRestorePlayerState;

    // Subscribe to events
    void OnEnable()
    {
        PlatformController.On_ReachedCheckpoint += UpdateMinYFromCheckpoint;
        PlatformController.On_NewPlatform += HandleNewPlatform; ;
        KillBox.On_PlayerDeath += HandlePlayerDeath;
        BoundaryController.On_PlayerDeath += HandlePlayerDeath;
        GameController.OnPlayerResurrection += HandlePlayerResurrection;
        UpAndDown.OnUpdateCameraSpeed += HandleUpdateCameraSpeed;
        UpAndDown.OnReturnCameraSpeed += HandleReturnCameraSpeed;
        CloudBehaviour.OnUpdateCameraSpeed += HandleUpdateCameraSpeed;
        CloudBehaviour.OnReturnCameraSpeed += HandleReturnCameraSpeed;
        StartPlatform.OnUpdateCameraSpeed += HandleUpdateCameraSpeed;
        StartPlatform.OnReturnCameraSpeed += HandleReturnCameraSpeed;
        IntroTrigger.OnZoomToGamePosition += HandleOnGameStart;
		MusicController.OnFastMusicStart += HandleOnFastMusicStart;
		PlayerRenderer.OnPlayerBecameVisible += HandleOnPlayerBecameVisible;
		PlayerRenderer.OnPlayerBecameInvisible += HandleOnPlayerBecameInvisible;
    }

    void HandleOnPlayerBecameVisible (Transform player)
    {
		_playerIsVisible = true;
    }

	void HandleOnPlayerBecameInvisible (Transform player)
	{
		_playerIsVisible = false;
	}

    void HandleOnFastMusicStart (float timedSpeed)
    {
		isTracking = true;
		isTimedDestroyCutscene = true;
		_shouldZoomOut = true;
		_timedDestroyZoomTimer = timedDestroyZoomTime;
		_playerSuspended = true;

    }

    private void HandleOnGameStart()
    {
        _zoomToGame = true;
        _zoomTimer = zoomWarmTime;
        //isTracking = true;
    }

    private void HandleReturnCameraSpeed()
    {
        YSmooth = defaultCameraSpeed;
        //Debug.Log("Return camera to default speed.");
    }

    private void HandleUpdateCameraSpeed(float speed)
    {
        //if (!(speed > YSmooth)) return;
        YSmooth = speed;
        //Debug.Log("Match camera speed to: " + speed);
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
        PlatformController.On_ReachedCheckpoint -= UpdateMinYFromCheckpoint;
        PlatformController.On_NewPlatform -= HandleNewPlatform;
        KillBox.On_PlayerDeath -= HandlePlayerDeath;
        BoundaryController.On_PlayerDeath -= HandlePlayerDeath;
        GameController.OnPlayerResurrection -= HandlePlayerResurrection;
        UpAndDown.OnUpdateCameraSpeed -= HandleUpdateCameraSpeed;
        UpAndDown.OnReturnCameraSpeed -= HandleReturnCameraSpeed;
        CloudBehaviour.OnUpdateCameraSpeed -= HandleUpdateCameraSpeed;
        CloudBehaviour.OnReturnCameraSpeed -= HandleReturnCameraSpeed;
        StartPlatform.OnUpdateCameraSpeed -= HandleUpdateCameraSpeed;
        StartPlatform.OnReturnCameraSpeed -= HandleReturnCameraSpeed;
        IntroTrigger.OnZoomToGamePosition -= HandleOnGameStart;
		MusicController.OnFastMusicStart -= HandleOnFastMusicStart;
		PlayerRenderer.OnPlayerBecameVisible -= HandleOnPlayerBecameVisible;
		PlayerRenderer.OnPlayerBecameInvisible -= HandleOnPlayerBecameInvisible;
    }

    void Awake()
    {
        _playerTarget = GameObject.Find("Player").transform.FindChild("CharacterTarget");
    }

    bool CheckXMargin()
    {
        return Mathf.Abs(transform.position.x - this.CameraTarget.position.x) > this.XMargin;
    }

    bool CheckYMargin()
    {
        return Mathf.Abs(transform.position.y - this.CameraTarget.position.y) > this.YMargin;
    }

    void Update()
    {
        if (_zoomToGame)
        {
            _zoomTimer -= Time.deltaTime;
            if (_zoomTimer <= 0)
            {
                transform.position = Vector3.Lerp(transform.position, gameCameraPosition, zoomSpeed * Time.deltaTime);
                _zoomTimer = 0;
            }

            if (transform.position.z >= (gameCameraPosition.z - 0.1f))
            {
                //Debug.Log("Fully zoomed in");
				if (OnMovePlayerToGamePosition != null && CameraTarget.parent.position.z != GameController.Instance.playerZPosition)
				{
					//Debug.Log("Move player once");
					OnMovePlayerToGamePosition(new Vector3(-1, 23.5f, GameController.Instance.playerZPosition));
				}
                transform.position = new Vector3(transform.position.x, transform.position.y, gameCameraPosition.z);
                _zoomToGame = false;
                /*if (!GameController.Instance.playerIsDead)
                {
                    isTracking = true;   
                }*/
                //isTracking = true;
                GameController.Instance.heightCounter.rectTransform.anchoredPosition = new Vector2(-20f, -20f);
                //GameController.Instance.countHeight = true;
            }

        }

        if (isTimedDestroyCutscene)
        {
            if (CameraTarget != PlatformController.Instance.timedDestroyCameraTarget.transform &&
                PlatformController.Instance.timedDestroyCameraTarget != null)
            {
                CameraTarget = PlatformController.Instance.timedDestroyCameraTarget.transform;
				if (On_CameraUpdatedMinY != null)
				{
					On_CameraUpdatedMinY(PlatformController.Instance.levelPlatforms.Keys.Min());
				}
            }

            //YSmooth = defaultCameraSpeed / 2;
			_previousMinY = MinXandY.y;
            MinXandY = new Vector2(0, PlatformController.Instance.timedDestroyCameraTarget.transform.position.y);
            _timedDestroyZoomTimer -= Time.deltaTime;
            if (_timedDestroyZoomTimer <= 0)
            {
				_playerVisibleTimer = playerVisibleTime;
				isTimedDestroyCutscene = false;
                _shouldZoomOut = (GameController.Instance.playerIsDead) ? true : false;
				//YSmooth = defaultCameraSpeed;
                CameraTarget = _playerTarget;
            }
        }
		// HACK
		else if (!isTimedDestroyCutscene && _playerIsVisible && _playerSuspended)
		{
			_playerVisibleTimer -= Time.deltaTime;
			if (_playerVisibleTimer <= 0)
			{
				isTracking = (GameController.Instance.playerIsDead) ? false : true;
				if (OnRestorePlayerState != null)
				{
					MinXandY = new Vector2(0, _previousMinY);
					OnRestorePlayerState();
					_playerSuspended = false;

				}
			}
		}
    }

	void HandleZoomInAndOut(float zoomDepth)
	{
	    transform.position = Vector3.Lerp(transform.position, _shouldZoomOut ? new Vector3(transform.position.x, transform.position.y, zoomDepth) : new Vector3(transform.position.x, transform.position.y, gameCameraPosition.z), zoomSpeed * Time.deltaTime);
	}

    void LateUpdate()
    {
        if (isTracking)
        {
            TrackPlayer();
        }

		if (GameController.Instance.countHeight) 
		{
			HandleZoomInAndOut(isTimedDestroyCutscene ? timedDestroyZoomDepth : ghostZoomDepth);
		}

        /*else if (!isTracking && !GameController.Instance.playerIsDead)
        {
            _introTimer -= Time.deltaTime;
            if (!(_introTimer <= 0)) return;
            isTracking = true;
            _introTimer = introTime;
        }*/
    }

    void TrackPlayer()
    {
        if (CameraTarget == null) return;
        var targetX = transform.position.x;
        var targetY = transform.position.y;

        if (this.CheckXMargin())
        {
            targetX = Mathf.Lerp(transform.position.x, this.CameraTarget.position.x, this.XSmooth * Time.deltaTime);
        }

        if (this.CheckYMargin())
        {
            targetY = Mathf.Lerp(transform.position.y, this.CameraTarget.position.y, this.YSmooth * Time.deltaTime);
        }

        targetX = Mathf.Clamp(targetX, this.MinXandY.x, this.MaxXandY.x);
        targetY = Mathf.Clamp(targetY, this.MinXandY.y, this.MaxXandY.y);

        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }

    void UpdateMinYFromCheckpoint(int checkpointPlatform)
    {
        var levelPlatforms = PlatformController.Instance.levelPlatforms;
        if (levelPlatforms == null || levelPlatforms.Count <= 0) return;

        GameObject checkoutPlatformObj;
        if (checkpointPlatform <= PlatformController.Instance.checkpointBuffer ||
            !levelPlatforms.TryGetValue(checkpointPlatform, out checkoutPlatformObj)) return;
        float newCameraMinY = checkoutPlatformObj.transform.position.y;

        // update only if height is greater than previous since platforms will be destroyed underneath 
        if (!(newCameraMinY > MinXandY.y) || checkpointPlatform < minCameraUpdatePlatform) return;

        MinXandY = new Vector2(MinXandY.x, newCameraMinY);

        if (On_CameraUpdatedMinY != null)
		{
			On_CameraUpdatedMinY(newCameraMinY);
		}

        On_DestroyLowerPlatforms(checkpointPlatform - PlatformController.Instance.checkpointBuffer - PlatformController.Instance.platformSpawnBuffer);
    }

    void HandleNewPlatform(float yPosition)
    {
        MaxXandY = new Vector2(MaxXandY.x, yPosition);
    }

    void HandlePlayerDeath()
    {
        isTracking = false;
		_shouldZoomOut = true;
        //Debug.Log("Turn off tracking");
    }

    void HandlePlayerResurrection()
    {
        isTracking = true;
		_shouldZoomOut = false;
        //Debug.Log("Turn on tracking");
    }
}