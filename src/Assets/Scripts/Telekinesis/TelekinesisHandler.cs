using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TelekinesisHandler : MonoBehaviour
{
    private PlatformBehaviour[] _platformScripts;
    private HazzardMovement[] _hazzardScripts;
    private float _stabilizeTimer;
    private Transform _hazzardModel;
    private float _teleportTimer;
    private ParticleSystem _powerEffect;

    protected Quaternion? rotationTarget;
    protected Vector3? teleportLocation;

    public bool isClone;
    public bool isStable;
    public float rotationSpeed = 1;
    public float stabilizationTime = 3;
    public float teleportTime = 1;

    public virtual void OnEnable()
    {
        TelekinesisController.On_NewTelekinesisRotation += HandleNewTelekinesisRotation;
        TelekinesisController.On_TelekinesisStabilize += HandleTelekinesisStabilize;
        TelekinesisController.On_NewTelekinesisMovePosition += HandleNewTelekinesisMovePosition;
    }

    public virtual void OnDisable()
    {
        UnsubscribeEvent();
    }

    public virtual void OnDestroy()
    {
        UnsubscribeEvent();
    }

    public virtual void UnsubscribeEvent()
    {
        TelekinesisController.On_NewTelekinesisRotation -= HandleNewTelekinesisRotation;
        TelekinesisController.On_TelekinesisStabilize -= HandleTelekinesisStabilize;
        TelekinesisController.On_NewTelekinesisMovePosition -= HandleNewTelekinesisMovePosition;
    }

    // Use this for initialization
    void Awake()
    {
        _platformScripts = GetComponentsInChildren<PlatformBehaviour>();
        _hazzardScripts = GetComponentsInChildren<HazzardMovement>();
        _hazzardModel = transform.FindChild("ProtoModel");
        _powerEffect = transform.GetComponentInChildren<ParticleSystem>();

    }

    void Update()
    {
        if (isStable)
        {
            _stabilizeTimer -= Time.deltaTime;
        }
        else
        {
            _stabilizeTimer = stabilizationTime;
        }

        HandleStabilizePlatforms();
        HandleStabilizeHazzards();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateToTarget();
        Teleport();
    }

    protected void HandleNewTelekinesisRotation(Transform platform, Quaternion rotation)
    {
        if (platform.GetInstanceID() != this.transform.GetInstanceID()) return;
        rotationTarget = rotation;
        if (_powerEffect != null)
        {
            _powerEffect.Play();
        }
    }

    protected virtual void RotateToTarget()
    {
        if (!rotationTarget.HasValue) return;
        if (transform.root.localRotation != rotationTarget)
        {
            transform.root.rotation = Quaternion.Lerp(transform.root.localRotation, rotationTarget.Value, rotationSpeed * Time.deltaTime);
        }
        if (transform.root.localRotation == rotationTarget)
        {
            rotationTarget = null;
        }
    }

    void Teleport()
    {
        if (teleportLocation.HasValue)
        {
            if (transform.position != teleportLocation.Value)
            {
                _teleportTimer -= Time.deltaTime;
                if (_hazzardModel != null)
                {
                    _hazzardModel.renderer.enabled = false;
                }
                if (_teleportTimer <= 0)
                {
                    transform.position = teleportLocation.Value;
                    if (_hazzardModel != null)
                    {
                        _hazzardModel.renderer.enabled = true;
                    }
                }
            }

            if (transform.position == teleportLocation.Value)
            {
                _teleportTimer = teleportTime;
                teleportLocation = null;
            }
        }
        else
        {
            _teleportTimer = teleportTime;
        }


    }

    void HandleTelekinesisStabilize(Transform stabilizedObject)
    {
        if (stabilizedObject.GetInstanceID() != this.transform.GetInstanceID()) return;

        isStable = true;
    }

    void HandleStabilizePlatforms()
    {
        if (isStable && _platformScripts != null && _platformScripts.Any(x => x.enabled))
        {
            //Debug.Log("Stabilize platform " + transform.name);
            _platformScripts.ToList().ForEach(x =>
            {
                x.enabled = false;
            });
        }

        if (!isClone && isStable && _stabilizeTimer <= 0 && _platformScripts != null && _platformScripts.Any(x => !x.enabled))
        {
            _platformScripts.ToList().ForEach(x =>
            {
                x.enabled = true;
            });
            isStable = false;
        }
    }

    void HandleStabilizeHazzards()
    {
        if (isStable && _hazzardScripts != null && _hazzardScripts.Any(x => x.enabled))
        {
            //Debug.Log("Stabilize hazzard " + transform.name);
            _hazzardScripts.ToList().ForEach(x =>
            {
                x.enabled = false;
            });
            //rigidbody.useGravity = false;
        }

        if (!isClone && isStable && _stabilizeTimer <= 0 && _hazzardScripts != null && _hazzardScripts.Any(x => !x.enabled))
        {
            _hazzardScripts.ToList().ForEach(x =>
            {
                x.enabled = true;
            });
            isStable = false;
        }
    }

    void HandleNewTelekinesisMovePosition(Transform objectToMove, Vector3 newPosition)
    {
        if (objectToMove.GetInstanceID() != this.transform.GetInstanceID()) return;

        teleportLocation = newPosition;
    }
}
