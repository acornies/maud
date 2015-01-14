using UnityEngine;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
	private float _shakeTimer;
	private float _moveDirection;
	private bool _shouldTimeoutShake;
    private Transform _groundCheck;
    private Transform _heightCheck;
    private int _additionalJumpCount;
    private bool _isHighJumping;
    private float _pushedTimer;
    private GameObject _playerModel;
    private Vector3 _ghostTouchTargetPosition;
    private bool _isGhostMoving;
    private float _highJumpTimer;
    private int _consectutiveJumpCounter;
    private Animator _animator;
    private bool _canMove;
    private bool _isUsingPowers;
    private bool _facingRight = true;
    private bool _isFacingCamera = true;
    //private Collider _playerCollider;
    private Transform _sparkEffect;
    private GameObject[] _playerModelObjects;
    private Transform _modelBody;
    private Transform _leftEye;
    private Transform _rightEye;
	private int _currentMaterialIndex;
	private Vector3 _savedVelocity;
	private Vector3 _savedAngularVelocity;

    public bool disabled;
    public bool isGrounded;

    public float headRayOffset;
    public float noseRayOffset;
    //public float chinRayOffset;
    public float chestRayOffset;
    public float midRayOffset;
    //public float bellyRayOffset;
    public float legRayOffset;
    //public float kneeRayOffset;
    public float footRayOffset;

    public float sphereColliderRadius;

    public bool isHittingHead;
	public float shakeTimeout = 1f;
    public float maxSpeed = 6.0f;
    public float ghostSpeed = 1f;
    public float jumpForce = 900.0f;
    public bool forcePushed;
    public float forcePushedInterval = 0.5f;
    public float stickyBuffer = 0.4f;
    public LayerMask whatIsGround;
    public float groundedRadius = 0.15f;
    public float headHitRadius = 0.1f;
    public int additionalJumps = 1;
    public float additionalJumpForce = 500.0f;
    public int jumpsForHighJump = 3;
    public float highJumpForce = 200f;
    public float highJumpTimeout = 0.5f;
    public float swipeJumpTolerenceTime = 1f;
    public float moveThreshold = 0.1f;
	public float swipeControlModeDivider = 2f;

    public Material[] bodyMaterials;
    public Material ghostBodyMaterial;
    public Material normalEyeMaterial;
    public Material ghostEyeMaterial;

    public AudioClip jumpSound;
    public AudioClip highJumpSound;
    public AudioClip midAirJumpSound;

    public delegate void ReachedPlatformAction(Transform platform, Transform player);
    public static event ReachedPlatformAction On_PlatformReached;

    public delegate void PlayerAirborne(Transform player);
    public static event PlayerAirborne On_PlayerAirborne;

    // Subscribe to events
    void OnEnable()
    {
        EasyTouch.On_DoubleTap += HandleDoubleTap;
        EasyTouch.On_Swipe += HandleSwipe;
        EasyTouch.On_SwipeEnd += HandleSwipeEnd;
        EasyTouch.On_SimpleTap += HandleSimpleTap;
        TelekinesisController.On_PlayerPowersStart += HandlePlayerPowersStart;
        TelekinesisController.On_PlayerPowersEnd += HandlePlayerPowersEnd;
        KillBox.On_PlayerDeath += HandleOnPlayerDeath;
        BoundaryController.On_PlayerDeath += HandleOnPlayerDeath;
        GameController.OnPlayerResurrection += HandleOnPlayerResurrection;
		GameController.OnGameOver += HandleOnGameOver;
		CameraMovement.OnMovePlayerToGamePosition += HandleOnMovePlayerToGamePosition;
		CameraMovement.OnRestorePlayerState += HandleOnRestorePlayerState;
        CloudBehaviour.On_CloudDestroy += HandleOnCloudDestroy;
		IntroTrigger.OnZoomToGamePosition += HandleOnZoomToGamePosition;
		IntroLedge.OnShowMenuButtons += HandleOnShowMenuButtons;
		SkyboxCameraMovement.OnPlayerMaterialUpdate += HandleOnPlayerMaterialUpdate;
		MusicController.OnFastMusicStart += HandleOnFastMusicStart;
    }

    void HandleOnRestorePlayerState ()
    {
		if (!GameController.Instance.playerIsDead)
		{
			rigidbody.isKinematic = false;
			rigidbody.velocity = _savedVelocity;
			rigidbody.angularVelocity = _savedAngularVelocity;
		}
		disabled = false;
    }

    void HandleOnGameOver ()
    {
		rigidbody.isKinematic = false;
    }

    void HandleOnFastMusicStart (float timedSpeed)
    {
		disabled = true;
		rigidbody.isKinematic = true;

		// save velocity
		_savedVelocity = rigidbody.velocity;
		_savedAngularVelocity = rigidbody.angularVelocity;
    }

    void HandleOnPlayerMaterialUpdate (int materialIndex)
    {
		//normalBodyMaterial = newMaterial;
		//TODO: Complete during skin pack work
		_currentMaterialIndex = materialIndex;
		//Debug.Log ("Update skin to material index: " + materialIndex);
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
        EasyTouch.On_DoubleTap -= HandleDoubleTap;
        EasyTouch.On_Swipe -= HandleSwipe;
        EasyTouch.On_SwipeEnd -= HandleSwipeEnd;
        EasyTouch.On_SimpleTap -= HandleSimpleTap;
        TelekinesisController.On_PlayerPowersStart -= HandlePlayerPowersStart;
        TelekinesisController.On_PlayerPowersEnd -= HandlePlayerPowersEnd;
        KillBox.On_PlayerDeath -= HandleOnPlayerDeath;
        BoundaryController.On_PlayerDeath -= HandleOnPlayerDeath;
        GameController.OnPlayerResurrection -= HandleOnPlayerResurrection;
		GameController.OnGameOver -= HandleOnGameOver;
		CameraMovement.OnMovePlayerToGamePosition -= HandleOnMovePlayerToGamePosition;
		CameraMovement.OnRestorePlayerState -= HandleOnRestorePlayerState;
        CloudBehaviour.On_CloudDestroy -= HandleOnCloudDestroy;
		IntroTrigger.OnZoomToGamePosition -= HandleOnZoomToGamePosition;
		IntroLedge.OnShowMenuButtons -= HandleOnShowMenuButtons;
		SkyboxCameraMovement.OnPlayerMaterialUpdate -= HandleOnPlayerMaterialUpdate;
		MusicController.OnFastMusicStart -= HandleOnFastMusicStart;
    }

    void Awake()
    {
        _groundCheck = GameObject.Find("GroundCheck").transform;
        _heightCheck = GameObject.Find("HeightCheck").transform;
        _playerModel = transform.FindChild("PlayerModel").gameObject;
        _animator = _playerModel.GetComponent<Animator>();
        _sparkEffect = transform.FindChild("Spark").transform;
        _playerModelObjects = GameObject.FindGameObjectsWithTag("PlayerModel");
    }

    void Start()
    {
        _modelBody = _playerModelObjects.First(x => x.transform.name == "Body").transform;
        _leftEye = _playerModelObjects.First(x => x.transform.name == "LeftEye").transform;
        _rightEye = _playerModelObjects.First(x => x.transform.name == "RightEye").transform;
    }

    void Update()
    {
		if (GameController.Instance.playerIsDead && _modelBody.renderer.material != ghostBodyMaterial)
		{
			_modelBody.renderer.material = ghostBodyMaterial;
            _leftEye.renderer.material = ghostEyeMaterial;
            _rightEye.renderer.material = ghostEyeMaterial;
        }
		else if (!GameController.Instance.playerIsDead && _modelBody.renderer.material != bodyMaterials[_currentMaterialIndex])
		{
			collider.enabled = true;
			_modelBody.renderer.material = bodyMaterials[_currentMaterialIndex];
			_leftEye.renderer.material = normalEyeMaterial;
            _rightEye.renderer.material = normalEyeMaterial;
        }

        if (GameController.Instance.playerIsDead && !GameController.Instance.initiatingRestart)
        {
            collider.enabled = false;

            if (_isGhostMoving)
            {
                //Debug.Log("Move ghost!");
                transform.position = Vector3.Lerp(transform.position, _ghostTouchTargetPosition,
                    ghostSpeed * Time.deltaTime);
            }
            else if (_isGhostMoving && transform.position == _ghostTouchTargetPosition)
            {
                //Debug.Log("Stop ghost!");
                _isGhostMoving = false;
                //_ghostTouchTargetPosition = Vector3.zero;
            }
        }

		if (_shouldTimeoutShake){
			_shakeTimer -= Time.deltaTime;
			if (_shakeTimer <= 0)
			{
				_shouldTimeoutShake = false;
				_animator.SetBool ("shake", false);
			}
		}
    }

    public void SetSpawnPosition(Vector3 newPosition)
    {
        _ghostTouchTargetPosition = newPosition;
        _isGhostMoving = true;
    }

    // Use this for physics updates
    void FixedUpdate()
    {
        //rigidbody.useGravity = SetGravity();
        //rigidbody.isKinematic = GameController.Instance.playerIsDead && !GameController.Instance.initiatingRestart;

        HandleAnimations();

        if (GameController.Instance.playerIsDead && !GameController.Instance.initiatingRestart) return;

        HandleStickyPhysics();
        HandleForcePushed();
        Move();

        // handle jump/is grounded control
        var groundColliders = Physics.OverlapSphere(_groundCheck.position, groundedRadius, whatIsGround);
        if (groundColliders != null)
        {
            isGrounded = groundColliders.Length > 0 ? true : false;
            var groundCollider = groundColliders.FirstOrDefault();
            if (groundCollider != null && isGrounded)
            {

                _additionalJumpCount = 0;
                forcePushed = false;

                On_PlatformReached(groundCollider.transform, transform); // trigger event for finding current platform
                _highJumpTimer -= Time.deltaTime;
                if (_consectutiveJumpCounter >= jumpsForHighJump)
                {
                    _consectutiveJumpCounter = 0;
                    _highJumpTimer = highJumpTimeout;
                }

                if (_highJumpTimer <= 0)
                {
                    _consectutiveJumpCounter = 0;
                }
            }
        }

        // handle head room
        var heightColliders = Physics.OverlapSphere(_heightCheck.position, headHitRadius, whatIsGround);
        if (heightColliders != null)
        {
            isHittingHead = heightColliders.Length > 0 ? true : false;
            var heightCollider = heightColliders.FirstOrDefault();
            if (heightCollider != null && isHittingHead)
            {
                if (heightColliders.Any(x => x.tag == "Hazzard"))
                {
                    _animator.SetBool("isHittingHead", true);
                }
            }
            else
            {
                _animator.SetBool("isHittingHead", false);
            }
        }

        if (!isGrounded && On_PlayerAirborne != null)
        {
            On_PlayerAirborne(transform);
            transform.parent = null;
        }

        if (_isUsingPowers) return;
        // flip player on the y axis
        if (_moveDirection > moveThreshold && !this._facingRight)
        {
            Flip();
        }
        else if (_moveDirection < -moveThreshold && this._facingRight)
        {
            Flip();
        }
    }

    void HandleOnMovePlayerToGamePosition(Vector3 playerPosition)
    {
        transform.position = playerPosition;
        //disabled = false;
        transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        _isFacingCamera = false;
        //disabled = false;
    }

    void HandleOnShowMenuButtons()
    {
        _animator.SetBool("shake", true);
        _shakeTimer = shakeTimeout;
        _shouldTimeoutShake = true;
    }

	// Handles jump for the intro
	void HandleOnZoomToGamePosition()
	{
		var theForce = jumpForce + highJumpForce;
		rigidbody.AddForceAtPosition(new Vector3(0, theForce , 0), transform.position);
	    _isHighJumping = true;
		PlayJumpSound(theForce);
	}
	
    void HandlePlayerPowersEnd()
    {
		//if (disabled) return;
        _isUsingPowers = false;
        _sparkEffect.GetComponent<ParticleSystem>().Stop();
        if (!_isFacingCamera) return;
		if (GameController.Instance.playerIsDead) return;
        TurnToAndAwayFromCamera((_facingRight) ? -90f : 90f);
    }

    void HandlePlayerPowersStart()
    {
        if (disabled) return;
        _isUsingPowers = true;
        if (_isFacingCamera) return;
        _sparkEffect.GetComponent<ParticleSystem>().Play();
        if (!isGrounded) return;           
        TurnToAndAwayFromCamera((_facingRight) ? 90f : -90f);
    }

    private void HandleOnPlayerDeath()
    {
		rigidbody.isKinematic = true;
		//rigidbody.useGravity = f
		if (_isFacingCamera) return;
        TurnToAndAwayFromCamera((_facingRight) ? 90f : -90f);
    }

    private void HandleOnPlayerResurrection()
    {
        //Debug.Log("Resurrection event handler!");
		rigidbody.isKinematic = false;
		_consectutiveJumpCounter = 0;
		if (!_isFacingCamera) return;
        TurnToAndAwayFromCamera((_facingRight) ? -90f : 90f);
    }

    private void TurnToAndAwayFromCamera(float degrees)
    {
        if (!_isFacingCamera)
        {
            transform.Rotate(Vector3.up, degrees, Space.World);
            _isFacingCamera = true;
        }
        else if (_isFacingCamera)
        {
            transform.Rotate(Vector3.up, degrees, Space.World);
            _isFacingCamera = false;
        }
    }

    void HandleForcePushed()
    {
        if (forcePushed)
        {
            _pushedTimer -= Time.deltaTime;
            if (!(_pushedTimer <= 0)) return;
            forcePushed = false;
        }
        else
        {
            _pushedTimer = forcePushedInterval;
        }
    }

    void HandleSimpleTap(Gesture gesture)
    {
        if (GameController.Instance.playerIsDead && !GameController.Instance.initiatingRestart)
        {
			GhostMovement(gesture);
            //GameController.Instance.movedFromSpawnPosition = true;
        }
        else
        {
            if (_highJumpTimer > 0 && _consectutiveJumpCounter == (jumpsForHighJump - 1))
            {
                Jump(gesture, highJumpForce);
                _isHighJumping = true;
            }
            else
            {
                Jump(gesture);
            }
            _highJumpTimer = highJumpTimeout;
        }
    }

    void HandleSwipeEnd(Gesture gesture)
    {
		if (PlayerState.Instance.Data.controlMode == ControlMode.FingerSwipe)
        {
            _moveDirection = 0;
        }

        if (gesture.actionTime < swipeJumpTolerenceTime && PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer)
        {
            Jump(gesture);
        }
    }

	private void GhostMovement(Gesture gesture)
	{
		if (disabled) return;
		_ghostTouchTargetPosition = gesture.GetTouchToWordlPoint(transform.position.z, true);
		_isGhostMoving = true;
	}

    void HandleSwipe(Gesture gesture)
    {
        if (GameController.Instance.playerIsDead && !GameController.Instance.initiatingRestart)
        {
			GhostMovement(gesture);
            //GameController.Instance.movedFromSpawnPosition = true;
        }
        
		// handle swipe control mode
		else if (!GameController.Instance.playerIsDead && PlayerState.Instance.Data.controlMode == ControlMode.FingerSwipe)
        {
			if( disabled) return;
			if (gesture.swipe == EasyTouch.SwipeType.Left || gesture.swipe == EasyTouch.SwipeType.Right)
            {
                var touchDir = (gesture.position.x - gesture.startPosition.x);
                float touchDirMultiplied = touchDir * 0.01f;
                _moveDirection = Mathf.Clamp(touchDirMultiplied, -1f, 1f);
            }
        }

    }

    void HandleDoubleTap(Gesture gesture)
    {
        Jump(gesture);
    }

    void HandleStickyPhysics()
    {
        // draw ray near the head of the player
        Vector3 headRay = new Vector3(transform.position.x, transform.position.y + headRayOffset, transform.position.z);
        Vector3 noseRay = new Vector3(transform.position.x, transform.position.y + noseRayOffset, transform.position.z);
        //Vector3 chinRay = new Vector3(transform.position.x, transform.position.y + chinRayOffset, transform.position.z);
        Vector3 chestRay = new Vector3(transform.position.x, transform.position.y + chestRayOffset, transform.position.z);
        Vector3 midRay = new Vector3(transform.position.x, transform.position.y + midRayOffset, transform.position.z);
        //Vector3 bellyRay = new Vector3(transform.position.x, transform.position.y - bellyRayOffset, transform.position.z);
        Vector3 legRay = new Vector3(transform.position.x, transform.position.y - legRayOffset, transform.position.z);
        //Vector3 kneeRay = new Vector3(transform.position.x, transform.position.y - kneeRayOffset, transform.position.z);
        Vector3 footRay = new Vector3(transform.position.x, transform.position.y - footRayOffset, transform.position.z);
        Debug.DrawRay(midRay, Vector3.right, Color.green);
        Debug.DrawRay(midRay, Vector3.left, Color.green);
        Debug.DrawRay(noseRay, Vector3.left, Color.yellow);
        Debug.DrawRay(noseRay, Vector3.right, Color.yellow);
        Debug.DrawRay(chestRay, Vector3.left, Color.grey);
        Debug.DrawRay(chestRay, Vector3.right, Color.grey);
        //Debug.DrawRay(chinRay, Vector3.left, Color.black);
        //Debug.DrawRay(chinRay, Vector3.right, Color.black);
        //Debug.DrawRay(bellyRay, Vector3.left, Color.magenta);
        //Debug.DrawRay(bellyRay, Vector3.right, Color.magenta);
        Debug.DrawRay(midRay, new Vector3(1, 0, 1), Color.green);
        Debug.DrawRay(midRay, new Vector3(-1, 0, 1), Color.green);
        Debug.DrawRay(midRay, new Vector3(1, 0, -1), Color.green);
        Debug.DrawRay(legRay, Vector3.left, Color.cyan);
        Debug.DrawRay(legRay, Vector3.right, Color.cyan);
        Debug.DrawRay(midRay, new Vector3(-1, 0, -1), Color.green);
        Debug.DrawRay(footRay, Vector3.right, Color.red);
        Debug.DrawRay(footRay, Vector3.left, Color.red);
        //Debug.DrawRay(kneeRay, Vector3.right, Color.white);
        //Debug.DrawRay(kneeRay, Vector3.left, Color.white);
        Debug.DrawRay(headRay, Vector3.right, Color.white);
        Debug.DrawRay(headRay, Vector3.left, Color.white);

        // stop player from sticking to colliders in mid-air
        RaycastHit hit;
        if (
            (Physics.Raycast(footRay, Vector3.right, out hit, stickyBuffer)
             || Physics.Raycast(footRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(footRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(legRay, Vector3.right, out hit, stickyBuffer)
             || Physics.Raycast(legRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(legRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             //|| Physics.Raycast(chinRay, Vector3.right, out hit, stickyBuffer)
             //|| Physics.Raycast(chinRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             //|| Physics.Raycast(chinRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(chestRay, Vector3.right, out hit, stickyBuffer)
             || Physics.Raycast(chestRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(chestRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             //|| Physics.Raycast(kneeRay, Vector3.right, out hit, stickyBuffer)
             //|| Physics.Raycast(kneeRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             //|| Physics.Raycast(kneeRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             //|| Physics.Raycast(bellyRay, Vector3.right, out hit, stickyBuffer)
             //|| Physics.Raycast(bellyRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             //|| Physics.Raycast(bellyRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(midRay, Vector3.right, out hit, stickyBuffer)
             || Physics.Raycast(midRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(midRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(noseRay, Vector3.right, out hit, stickyBuffer)
             || Physics.Raycast(noseRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(noseRay, new Vector3(1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(headRay, Vector3.right, out hit, stickyBuffer)
             || Physics.Raycast(headRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(headRay, new Vector3(1, 0, -1), out hit, stickyBuffer))
            && (hit.transform.gameObject.layer == 8 && hit.transform.tag == "Stoppable" && !isGrounded))
        {
            _canMove = !(_moveDirection > 0);
            //Debug.Log("Hit with sphere on right, moveDirection: " + _moveDirection + " canMove: " + _canMove);
        }
        else if (
            (Physics.Raycast(footRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(footRay, Vector3.left, out hit, stickyBuffer)
             || Physics.Raycast(footRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(legRay, Vector3.left, out hit, stickyBuffer)
             || Physics.Raycast(legRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(legRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             //|| Physics.Raycast(chinRay, Vector3.left, out hit, stickyBuffer)
             //|| Physics.Raycast(chinRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             //|| Physics.Raycast(chinRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(chestRay, Vector3.left, out hit, stickyBuffer)
             || Physics.Raycast(chestRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(chestRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             //|| Physics.Raycast(kneeRay, Vector3.left, out hit, stickyBuffer)
             //|| Physics.Raycast(kneeRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             //|| Physics.Raycast(kneeRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             //|| Physics.Raycast(bellyRay, Vector3.left, out hit, stickyBuffer)
             //|| Physics.Raycast(bellyRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             //|| Physics.Raycast(bellyRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(midRay, Vector3.left, out hit, stickyBuffer)
             || Physics.Raycast(midRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(midRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(noseRay, Vector3.left, out hit, stickyBuffer)
             || Physics.Raycast(noseRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(noseRay, new Vector3(-1, 0, -1), out hit, stickyBuffer)
             || Physics.Raycast(headRay, Vector3.left, out hit, stickyBuffer)
             || Physics.Raycast(headRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
             || Physics.Raycast(headRay, new Vector3(-1, 0, -1), out hit, stickyBuffer))
            && (hit.transform.gameObject.layer == 8 && hit.transform.tag == "Stoppable" && !isGrounded))
        {
            _canMove = !(_moveDirection < 0);
            //Debug.Log("Hit with sphere on left, moveDirection: " + _moveDirection + " canMove: " + _canMove);
        }
        else
        {
            _canMove = true;
        }
    }

    private void HandleAnimations()
    {
        // don't play animations underneith the moveThreshold
        if (_moveDirection < -moveThreshold || _moveDirection > moveThreshold)
        {
            _animator.SetFloat("speed", Mathf.Abs(_moveDirection));
        }
        else
        {
            _animator.SetFloat("speed", 0);
        }
        _animator.SetFloat("vSpeed", rigidbody.velocity.y);
        _animator.SetBool("isGrounded", isGrounded);
        _animator.SetBool("isHighJump", _isHighJumping);
        _animator.SetBool("isDead", GameController.Instance.playerIsDead);
        _animator.SetBool("isUsingPowers", _isUsingPowers);
    }

    private void Move()
    {
        if (disabled) return;
		if (PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer)
        {
            //Debug.Log(Input.acceleration.x);
            _moveDirection = Input.acceleration.x;
        }

        // move player
        if (!forcePushed && !rigidbody.isKinematic)
        {
            rigidbody.velocity = ApplyVelocity();
        }
    }

    private Vector2 ApplyVelocity()
    {
        //Debug.Log (_moveDirection);
        if (PlayerState.Instance.Data.controlMode == ControlMode.Accelerometer && 
		    _canMove && !_isUsingPowers && (_moveDirection < -moveThreshold || _moveDirection > moveThreshold))
        {
            return new Vector2(_moveDirection * maxSpeed, rigidbody.velocity.y);
        }
		else if (PlayerState.Instance.Data.controlMode == ControlMode.FingerSwipe && _canMove && !_isUsingPowers)
		{
			return new Vector2(_moveDirection * (maxSpeed / swipeControlModeDivider), rigidbody.velocity.y);
		}

        return new Vector2(0, rigidbody.velocity.y);
    }

	public void UpdateSpeed(float newSpeed)
	{
		maxSpeed = newSpeed;
	}

    void Flip()
    {
        this._facingRight = !_facingRight;
        transform.Rotate(Vector3.up, 180.0f, Space.World);
    }

    public void Jump(Gesture gesture, float extraForce = 0)
    {
		if (GameController.Instance.playerIsDead || rigidbody.isKinematic) return;
        if (disabled) return;

		if (gesture.touchCount > 1) return; // prevents dual tap super jump
        if (this.isGrounded)
        {
            var theForce = (jumpForce + extraForce);
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z); // reset for consistent jump
            rigidbody.AddForceAtPosition(new Vector3(0, theForce, 0), transform.position);
            PlayJumpSound(theForce);
            _isHighJumping = extraForce > 0;
            _consectutiveJumpCounter++;
        }

        // conditions for mid-air jump
        if (isGrounded || _additionalJumpCount >= additionalJumps || _isHighJumping || forcePushed) return;

        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z); // reset for consistent double jump
        rigidbody.AddForceAtPosition(new Vector3(0, additionalJumpForce, 0), transform.position);
        _additionalJumpCount++;
        _consectutiveJumpCounter = 0; // reset consecutive jump counter
        if (midAirJumpSound != null && midAirJumpSound.isReadyToPlay)
        {
            audio.PlayOneShot(midAirJumpSound, 1);
        }
    }

    private void PlayJumpSound(float force = 0)
    {
        if (jumpSound == null)
        {
            Debug.LogWarning("Please assign a jump sound to this script.");
        }

        if (highJumpSound == null)
        {
            Debug.LogWarning("Please assign a high jump sound to this script.");
        }

        if (jumpSound != null && jumpSound.isReadyToPlay && force <= jumpForce)
        {
            audio.PlayOneShot(jumpSound, 1);
        }

        if (highJumpSound != null && highJumpSound.isReadyToPlay && force > jumpForce)
        {
            audio.PlayOneShot(highJumpSound, 1);
        }
    }

    void HandleOnCloudDestroy()
    {
        transform.parent = null;
    }
}
