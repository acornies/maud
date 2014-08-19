using UnityEngine;
using System.Collections;

public class RotationHandler : TelekinesisHandler
{
    protected Quaternion? rotationTarget;

    public float rotationSpeed = 1;

    public virtual void OnEnable()
    {
        TelekinesisController.On_NewTelekinesisRotation += HandleNewTelekinesisRotation;
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
    }

    // Use this for initialization
    void Start()
    {

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
            //Debug.Log("new rotation: " + rotation + " for " + platform.name);
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
}
