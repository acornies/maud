using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TelekinesisHandler : MonoBehaviour
{
    private PlatformBehaviour[] _platformScripts;
    private HazzardMovement[] _hazzardScripts;
    private float _stabilizeTimer;

    protected Quaternion? rotationTarget;

    public bool isClone;
    public bool isStable;
    public float rotationSpeed = 1;
    public float stabilizationTime = 3;

    public virtual void OnEnable()
    {
        TelekinesisController.On_NewTelekinesisRotation += HandleNewTelekinesisRotation;
        TelekinesisController.On_TelekinesisStabilize += HandleTelekinesisStabilize;
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
    }

    // Use this for initialization
    void Start()
    {
        _platformScripts = GetComponentsInChildren<PlatformBehaviour>();
        _hazzardScripts = GetComponentsInChildren<HazzardMovement>();
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
        
        HandlePlatforms();
        HandleHazzards();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateToTarget();
    }

    protected void HandleNewTelekinesisRotation(Transform platform, Quaternion rotation)
    {
        if (platform.GetInstanceID() == this.transform.GetInstanceID())
        {
            rotationTarget = rotation;
        }
    }

    protected virtual void RotateToTarget()
    {
        if (rotationTarget == null) return;
        if (transform.root.localRotation != rotationTarget)
        {
            transform.root.rotation = Quaternion.Lerp(transform.root.localRotation, rotationTarget.Value, rotationSpeed * Time.deltaTime);
        }
        if (transform.root.localRotation == rotationTarget)
        {
            rotationTarget = null;
        }
    }

    void HandleTelekinesisStabilize(Transform stabilizedObject)
    {
        if (stabilizedObject.GetInstanceID() != this.transform.GetInstanceID()) return;

        isStable = true;
    }

    void HandlePlatforms()
    {
        if (isStable && _platformScripts != null && _platformScripts.Any(x => x.enabled))
        {
            Debug.Log("Stabilize platform " + transform.name);
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

    void HandleHazzards()
    {
        if (isStable && _hazzardScripts != null && _hazzardScripts.Any(x => x.enabled))
        {
            Debug.Log("Stabilize hazzard " + transform.name);
            _hazzardScripts.ToList().ForEach(x =>
            {
                x.enabled = false;
            });
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
}
