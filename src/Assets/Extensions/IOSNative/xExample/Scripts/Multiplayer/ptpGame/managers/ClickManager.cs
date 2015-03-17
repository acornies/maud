////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using UnionAssets.FLE;
using System.Collections;

public class ClickManager : MonoBehaviour {
	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		GameCenterMultiplayer.OnDataReceived += OnData;
	}


	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			pos.z = 1f;

			PTPGameController.instance.createGreenSphere (pos);



			ObjectCreatePackage p = new ObjectCreatePackage (pos.x, pos.y);
			p.send ();

		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private  void OnData(GameCenterDataPackage package) {




		ByteBuffer b = new ByteBuffer (package.buffer);

		int pId = b.readInt();

		switch(pId) {
		case 1:
			Debug.Log("Sphere pack");
			Vector3 pos = new Vector3 (0, 0, 1);
			pos.x = b.readFloat ();
			pos.y = b.readFloat ();
			
			
			
			PTPGameController.instance.createRedSphere (pos);
			break;
		default:
			Debug.Log("Got pack wit id: " + pId);
			break;
		}



	

	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
