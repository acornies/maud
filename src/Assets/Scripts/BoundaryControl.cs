using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class BoundaryControl : MonoBehaviour 
{
	public Transform player;

	void Awake()
	{
	}

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
	}
}
