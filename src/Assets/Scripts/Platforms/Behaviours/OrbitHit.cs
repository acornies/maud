using UnityEngine;
using System.Collections;

public class OrbitHit : MonoBehaviour
{

    private Orbit _parentOrbitBehaviour;

    // Use this for initialization
    void Start()
    {
        _parentOrbitBehaviour = transform.parent.GetComponent<Orbit>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// TODO: Move script to child object
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("collision enter");
        HandlePlayerCollisions(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("collision stay");
        HandlePlayerCollisions(collision);
    }
    void HandlePlayerCollisions(Collision collision)
    {
        var playerMovement = collision.transform.GetComponent<PlayerMovement>();
        if (collision.gameObject.name != "Player" || _parentOrbitBehaviour.isOnPlatform || _parentOrbitBehaviour.isStopped)
        {
            //Debug.Log("no force because: " + isOnPlatform + playerMovement.isHittingHead + _isStopped);
            return;
        }

        if (!_parentOrbitBehaviour.canStopWithHead && playerMovement.isHittingHead) return; 

        /*if (_parentOrbitBehaviour.axis.y > 0)
        {
            collision.rigidbody.AddForce(-1 * (rigidbody.mass * _parentOrbitBehaviour.orbitRotationSpeed), collision.transform.position.y, collision.transform.position.z);
            Debug.Log("orbit hit on right with force: " + (rigidbody.mass * _parentOrbitBehaviour.orbitRotationSpeed));
        }
        else
        {
            collision.rigidbody.AddForce(1 * (rigidbody.mass * _parentOrbitBehaviour.orbitRotationSpeed), collision.transform.position.y, collision.transform.position.z);
            Debug.Log("orbit hit on left with force: " + (rigidbody.mass * _parentOrbitBehaviour.orbitRotationSpeed));
        }
*/
        playerMovement.forcePushed = true;
        _parentOrbitBehaviour.isStopped = true;
    }
}
