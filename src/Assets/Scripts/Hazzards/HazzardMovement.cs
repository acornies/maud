using UnityEngine;
using System.Collections;

public class HazzardMovement : MonoBehaviour
{

    private bool _isStable;
    private TelekinesisHandler _handler;

    //public Vector3 startPosition;
    public Vector3 targetPosition;
    public float speed = 1;

    // Use this for initialization
    void Start()
    {
        _handler = GetComponent<TelekinesisHandler>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _isStable = _handler.isStable;
        
        if (targetPosition == Vector3.zero && targetPosition == transform.position) return;

        if (_isStable) return;
        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            Destroy(gameObject);
        }
    }
}
