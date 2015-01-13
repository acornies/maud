using UnityEngine;
using System.Collections;

public class PlayerRenderer : MonoBehaviour
{
	public delegate void PlayerBecameVisible(Transform player);
	public static event PlayerBecameVisible OnPlayerBecameVisible;
	
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
		if (OnPlayerBecameVisible != null)
		{
			OnPlayerBecameVisible(transform.root);
		}
    }
}
