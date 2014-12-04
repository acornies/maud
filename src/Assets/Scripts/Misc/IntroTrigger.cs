using UnityEngine;
using System.Collections;

public class IntroTrigger : MonoBehaviour {

    public Vector3 introLedgePosition;

    public delegate void NewIntroLedegePosition(Vector3 newPosition);
    public static event NewIntroLedegePosition OnNewIntroLedegePosition;
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TriggerNewPosition()
    {
        if (OnNewIntroLedegePosition != null)
        {
            OnNewIntroLedegePosition(introLedgePosition);
        }
    }
}
