using UnityEngine;
using System.Collections;

public class VisibleToggle : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnBecameVisible()
    {
        Debug.Log("Player visible");
        Debug.Log(transform.root.name);
    }
}
