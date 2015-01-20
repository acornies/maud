////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using UnionAssets.FLE;
using System.Collections.Generic;

public class PTPGameController : MonoBehaviour {

	public GameObject pref;

	private DisconnectButton d;
	private ConectionButton b;
	private ClickManager m;

	public static PTPGameController instance;


	private List<GameObject> spheres =  new List<GameObject>();

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {

		instance = this;


		GameCenterManager.OnAuthFinished += OnAuthFinished;
		GameCenterManager.init ();



		b = gameObject.AddComponent<ConectionButton> ();
		b.enabled = false;

		d = gameObject.AddComponent<DisconnectButton> ();
		d.enabled = false;

		m = gameObject.GetComponent<ClickManager> ();
		m.enabled = false;


		GameCenterMultiplayer.instance.addEventListener (GameCenterMultiplayer.PLAYER_DISCONNECTED, OnGCPlayerDisconnected);
		GameCenterMultiplayer.instance.addEventListener (GameCenterMultiplayer.MATCH_STARTED, OnGCMatchStart);

	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void createRedSphere(Vector3 pos) {
		GameObject s = Instantiate(pref) as GameObject;
		s.transform.position = pos;

		s.GetComponent<Renderer>().enabled = true;
		s.GetComponent<Renderer>().material = new Material (s.GetComponent<Renderer>().material);
		s.GetComponent<Renderer>().material.color = Color.red;

		spheres.Add (s);

	}

	public void createGreenSphere(Vector3 pos) {
		GameObject s = Instantiate(pref) as GameObject;
		s.transform.position = pos;

		s.GetComponent<Renderer>().enabled = true;
		s.GetComponent<Renderer>().material = new Material (s.GetComponent<Renderer>().material);
		s.GetComponent<Renderer>().material.color = Color.green;

		spheres.Add (s);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	void OnAuthFinished (ISN_Result res) {
		if (res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authed ", "ID: " + GameCenterManager.player.playerId + "\n" + "Name: " + GameCenterManager.player.displayName);
			cleanUpScene ();
		}

	}



	private void OnGCPlayerDisconnected(CEvent e) {
		IOSNativePopUpManager.showMessage ("Disconnect", "Game finished");
		cleanUpScene ();
	}

	private void OnGCMatchStart(CEvent e) {
		GameCenterMatchData match = e.data as GameCenterMatchData;

		IOSNativePopUpManager.showMessage ("OnMatchStart", "let's playe now \n  Other player count: " + match.playerIDs.Count);




		m.enabled = true;
		b.enabled = false;
		d.enabled = true;

	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void cleanUpScene() {
		b.enabled = true;
		m.enabled = false;
		d.enabled = false;

		foreach(GameObject sp in spheres) {
			Destroy (sp);
		}

		spheres.Clear ();
	}
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
