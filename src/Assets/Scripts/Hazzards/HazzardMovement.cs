using UnityEngine;
using System.Collections;

public class HazzardMovement : MonoBehaviour
{

    public Vector3 targetPosition;
    public float speed = 1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (targetPosition == Vector3.zero && targetPosition == transform.position) return;
        
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            Destroy(gameObject);
        }
    }
}
