using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
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

    //public bool isDead;
    public bool canMove;
    public bool isUsingPowers;
    public float maxSpeed = 6.0f;
    public float ghostSpeed = 1f;
    public bool facingRight = true;
    public float moveDirection;
    public float jumpForce = 900.0f;
    //public float longJumpForce = 300.0f;
    public bool isGrounded;
    public bool isHittingHead;
    public bool forcePushed;
    public float forcePushedInterval = 0.5f;
    public float stickyBuffer = 0.4f;
    public LayerMask whatIsGround;
    public float groundedRadius = 0.15f;
    public float headHitRadius = 0.1f;
    public int additionalJumps = 1;
    public float additionalJumpForce = 500.0f;
    public float highJumpTimeout = 0.5f;

    //public bool useAcceleration = false;
    public float accelerometerMultiplier = 1.5f;

    public delegate void ReachedPlatformAction(Transform platform, Transform player);
    public static event ReachedPlatformAction On_PlatformReached;

    public delegate void HitHeadAction(Transform platform);
    public static event HitHeadAction On_HitHead;

    public delegate void PlayerAirborne();
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
    }

    void HandlePlayerPowersEnd()
    {
        isUsingPowers = false;
    }

    void HandlePlayerPowersStart()
    {
        isUsingPowers = true;
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
    }

    void Awake()
    {
        _groundCheck = GameObject.Find("GroundCheck").transform;
        _heightCheck = GameObject.Find("HeightCheck").transform;
        _playerModel = transform.FindChild("PlayerModel").gameObject;
    }

    void Start()
    {
    }

    void Update()
    {
        //isDead = GameController.Instance.playerIsDead;
        if (GameController.Instance.playerIsDead && !GameController.Instance.initiatingRestart)
        {
            //Debug.Log("isGhostMoving: " + _isGhostMoving + " ghostTargetPosition: " + _ghostTouchTargetPosition);

            _playerModel.renderer.material.color = new Color(1, 1, 1, 0.5f);
            if (_isGhostMoving && _ghostTouchTargetPosition != Vector3.zero)
            {
                //Debug.Log("Move ghost!");
                transform.position = Vector3.Lerp(transform.position, _ghostTouchTargetPosition, ghostSpeed * Time.deltaTime);
            }
            if (_isGhostMoving && transform.position == _ghostTouchTargetPosition)
            {
                //Debug.Log("Stop ghost!");
                _isGhostMoving = false;
                _ghostTouchTargetPosition = Vector3.zero;
            }
        }
        else
        {
            //Debug.Log(_playerModel.renderer.material.color);
            _playerModel.renderer.material.color = new Color(1, 1, 1, 1);
        }
    }

    // Use this for physics updates
    void FixedUpdate()
    {
        rigidbody.useGravity = !GameController.Instance.playerIsDead;
        rigidbody.isKinematic = GameController.Instance.playerIsDead;

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
                if (_consectutiveJumpCounter >= 2)
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
            if (heightCollider != null && isHittingHead && On_HitHead != null)
            {
                On_HitHead(heightCollider.transform);
            }
        }

        if (!isGrounded && On_PlayerAirborne != null)
        {
            On_PlayerAirborne();
        }

        if (isUsingPowers) return;
        // flip player on the y axis
        if (this.moveDirection > 0.0f && !this.facingRight)
        {
            Flip();
        }
        else if (this.moveDirection < 0.0f && this.facingRight)
        {
            Flip();
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
            _ghostTouchTargetPosition = gesture.GetTouchToWordlPoint(transform.position.z, true);
            _isGhostMoving = true;
            GameController.Instance.movedFromSpawnPosition = true;
        }
        else
        {
            if (_highJumpTimer > 0 && _consectutiveJumpCounter == 1)
            {
                Jump(gesture, additionalJumpForce);
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
        if (!GameController.Instance.useAcceleration)
        {
            moveDirection = 0;
        }

        if (gesture.swipe == EasyTouch.SwipeType.Up)
        {
            Jump(gesture);
        }
    }

    void HandleSwipe(Gesture gesture)
    {
        if (GameController.Instance.playerIsDead && !GameController.Instance.initiatingRestart)
        {
            _ghostTouchTargetPosition = Vector3.zero;
            transform.position = Vector3.Lerp(transform.position, gesture.GetTouchToWordlPoint(transform.position.z, true), ghostSpeed * 0.01f);
            GameController.Instance.movedFromSpawnPosition = true;
        }
        else
        {
            if (gesture.swipe == EasyTouch.SwipeType.Left || gesture.swipe == EasyTouch.SwipeType.Right && !GameController.Instance.useAcceleration)
            {
                var touchDir = (gesture.position.x - gesture.startPosition.x);
                float touchDirMultiplied = touchDir * 0.01f;
                moveDirection = Mathf.Clamp(touchDirMultiplied, -1f, 1f);
            }
        }

    }

    void HandleDoubleTap(Gesture gesture)
    {
        //Jump(gesture);
    }

    void HandleStickyPhysics()
    {
        //if (isGrounded) return;

        // draw ray near the head of the player
        Vector3 headRay = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
        Vector3 midRay = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        Vector3 footRay = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
        Debug.DrawRay(midRay, Vector3.right, Color.white);
        Debug.DrawRay(midRay, Vector3.left, Color.white);
        Debug.DrawRay(midRay, new Vector3(1, 0, 1), Color.green);
        Debug.DrawRay(midRay, new Vector3(-1, 0, 1), Color.green);
        Debug.DrawRay(footRay, Vector3.right, Color.white);
        Debug.DrawRay(footRay, Vector3.left, Color.white);
        Debug.DrawRay(headRay, Vector3.right, Color.white);
        Debug.DrawRay(headRay, Vector3.left, Color.white);

        // stop player from sticking to colliders in mid-air
        RaycastHit hit;
        if (
            Physics.Raycast(footRay, Vector3.right, out hit, stickyBuffer)
            || Physics.Raycast(footRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
            || Physics.Raycast(footRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
            || Physics.Raycast(footRay, Vector3.left, out hit, stickyBuffer)
            || Physics.Raycast(midRay, Vector3.right, out hit, stickyBuffer)
            || Physics.Raycast(midRay, Vector3.left, out hit, stickyBuffer)
            || Physics.Raycast(midRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
            || Physics.Raycast(midRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
            || Physics.Raycast(headRay, Vector3.right, out hit, stickyBuffer)
            || Physics.Raycast(headRay, Vector3.left, out hit, stickyBuffer)
            || Physics.Raycast(headRay, new Vector3(1, 0, 1), out hit, stickyBuffer)
            || Physics.Raycast(headRay, new Vector3(-1, 0, 1), out hit, stickyBuffer)
            )
        {
            // the "walkable" layer
            if (hit.transform.gameObject.layer == 8 && !isGrounded)
            {
                canMove = false;
            }
            else
            {
                canMove = true;
            }
        }
        else
        {
            canMove = true;
        }
    }

    private void Move()
    {
        if (GameController.Instance.useAcceleration)
        {
            //Debug.Log(Input.acceleration.x);
            moveDirection = Input.acceleration.x;
        }

        // move player
        if (!forcePushed && !rigidbody.isKinematic)
        {
            rigidbody.velocity = ApplyVelocity();
        }
    }

    private Vector2 ApplyVelocity()
    {
        if (canMove && !isUsingPowers && !GameController.Instance.useAcceleration)
        {
            return new Vector2(this.moveDirection * maxSpeed, rigidbody.velocity.y);
        }

        if (canMove && !isUsingPowers && GameController.Instance.useAcceleration)
        {
            //var accelerometerMultiplier = 1.5f;
            float newSpeed = (maxSpeed * accelerometerMultiplier);
            return new Vector2(this.moveDirection * newSpeed, rigidbody.velocity.y);
        }

        return new Vector2(0, rigidbody.velocity.y);
    }

    void Flip()
    {
        this.facingRight = !facingRight;
        transform.Rotate(Vector3.up, 180.0f, Space.World);
    }

    public void Jump(Gesture gesture, float extraForce = 0)
    {
        if (!canMove) return;
        if (gesture.touchCount > 1) return; // prevents dual tap super jump
        if (this.isGrounded)
        {
            rigidbody.AddForceAtPosition(new Vector3(0, jumpForce + extraForce, 0), transform.position);
            _isHighJumping = extraForce > 0;
            _consectutiveJumpCounter++;
        }

        // conditions for mid-air jump
        if (isGrounded || _additionalJumpCount >= additionalJumps || _isHighJumping || forcePushed) return;
        rigidbody.AddForce(new Vector3(0, additionalJumpForce, 0));
        _additionalJumpCount++;
    }
}
