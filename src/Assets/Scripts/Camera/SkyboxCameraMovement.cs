using UnityEngine;
using System.Collections;

public class SkyboxCameraMovement : MonoBehaviour
{

    public float rotationSpeed = 1f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); //rotates 50 degrees per second around z axis
    }
}
