using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
	private bool _zoomToGame;

	public Vector3 gameCameraPosition;
	public float zoomSpeed = 10f;
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

    public delegate void UpdatedCameraMinY(float yPosition, int checkpointPlatform);
    public static event UpdatedCameraMinY On_CameraUpdatedMinY;

    public delegate void DestroyLowerPlatforms(int platformNumber, int childPlatformToDeleteIndex);
    public static event DestroyLowerPlatforms On_DestroyLowerPlatforms;

    // Subscribe to events
    void OnEnable()
    {
        PlatformController.On_ReachedCheckpoint += UpdateMinYFromCheckpoint;
        PlatformController.On_NewPlatform += HandleNewPlatform;;
        KillBox.On_PlayerDeath += HandlePlayerDeath;
        BoundaryController.On_PlayerDeath += HandlePlayerDeath;
        GameController.OnPlayerResurrection += HandlePlayerResurrection;
        UpAndDown.OnUpdateCameraSpeed += HandleUpdateCameraSpeed;
        UpAndDown.OnReturnCameraSpeed += HandleReturnCameraSpeed;
        StartPlatform.OnUpdateCameraSpeed += HandleUpdateCameraSpeed;
		StartPlatform.OnReturnCameraSpeed += HandleReturnCameraSpeed;
		//IntroTrigger.OnNewIntroLedgePosition += HandleNewIntroLedgePosition;
		IntroTrigger.OnZoomToGamePosition += HandleOnGameStart;
    }

	private void HandleOnGameStart()
	{
		_zoomToGame = true;
	    isTracking = true;
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
        StartPlatform.OnUpdateCameraSpeed -= HandleUpdateCameraSpeed;
		StartPlatform.OnReturnCameraSpeed -= HandleReturnCameraSpeed;
		//IntroTrigger.OnNewIntroLedgePosition -= HandleNewIntroLedgePosition;
        IntroTrigger.OnZoomToGamePosition -= HandleOnGameStart;
    }

    void Awake()
    {
        
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
			transform.position = Vector3.Lerp(transform.position, gameCameraPosition, zoomSpeed * Time.deltaTime);
		}
	}

    void LateUpdate()
    {
        if (isTracking)
        {
            TrackPlayer();
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

    void UpdateMinYFromCheckpoint(int checkpointPlatform, int childPlatformToDeleteIndex)
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

        On_CameraUpdatedMinY(newCameraMinY, checkpointPlatform);

        On_DestroyLowerPlatforms(checkpointPlatform - PlatformController.Instance.checkpointBuffer - PlatformController.Instance.platformSpawnBuffer, childPlatformToDeleteIndex);
    }

    void HandleNewPlatform(float yPosition)
    {
        MaxXandY = new Vector2(MaxXandY.x, yPosition);
    }

    void HandlePlayerDeath()
    {
        isTracking = false;
        //Debug.Log("Turn off tracking");
    }

    void HandlePlayerResurrection()
    {
        isTracking = true;
        //Debug.Log("Turn on tracking");
    }
}