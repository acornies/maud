using UnityEngine;
using System.Collections;

public class PlayerRenderer : MonoBehaviour
{
	public delegate void PlayerBecameVisible(Transform player);
	public static event PlayerBecameVisible OnPlayerBecameVisible;

	public delegate void PlayerBecameInvisible(Transform player);
	public static event PlayerBecameInvisible OnPlayerBecameInvisible;
	
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
        //Debug.Log("Player visible");
		if (OnPlayerBecameVisible != null)
		{
			OnPlayerBecameVisible(transform.root);
		}
    }

	void OnBecameInvisible()
	{
		if (OnPlayerBecameInvisible != null)
		{
			OnPlayerBecameInvisible(transform.root);
		}
	}
}
