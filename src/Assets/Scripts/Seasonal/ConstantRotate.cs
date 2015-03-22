using UnityEngine;
using System.Collections;

public class ConstantRotate : MonoBehaviour
{
    public Vector3 axis;
    public float speed = 1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime, Space.Self);
    }
}
