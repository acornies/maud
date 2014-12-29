using UnityEngine;
using System.Collections;

public class MobileHelper : MonoBehaviour {

	public static bool isTablet {
		get { return Screen.height > 2000;}
	}
}
