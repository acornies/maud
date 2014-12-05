using UnityEngine;
using System.Collections;

public class IntroTrigger : MonoBehaviour {

    public Vector3 introLedgePosition;

    public delegate void NewIntroLedgePosition(Vector3 newPosition);
    public static event NewIntroLedgePosition OnNewIntroLedgePosition;
    
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TriggerNewPosition()
    {
        if (OnNewIntroLedgePosition != null)
        {
            OnNewIntroLedgePosition(introLedgePosition);
        }
    }
}
