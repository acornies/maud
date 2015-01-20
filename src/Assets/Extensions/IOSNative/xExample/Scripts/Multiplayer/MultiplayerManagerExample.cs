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

public class MultiplayerManagerExample : MonoBehaviour {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {

		GameCenterManager.init();

		GameCenterMultiplayer.instance.addEventListener (GameCenterMultiplayer.PLAYER_CONNECTED, OnGCPlayerConnected);
		GameCenterMultiplayer.instance.addEventListener (GameCenterMultiplayer.PLAYER_DISCONNECTED, OnGCPlayerDisconnected);

		GameCenterMultiplayer.instance.addEventListener (GameCenterMultiplayer.MATCH_STARTED, OnGCMatchStart);
		GameCenterMultiplayer.instance.addEventListener (GameCenterMultiplayer.DATA_RECEIVED, OnGCDataReceived);
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		if(GUI.Button(new Rect(170, 70, 150, 50), "Find Match")) {
			GameCenterMultiplayer.instance.FindMatch (2, 2);
		}

		if(GUI.Button(new Rect(170, 130, 150, 50), "Send Data to All")) {
			string msg = "hello world";
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
			byte[] data = encoding.GetBytes(msg);
			GameCenterMultiplayer.instance.SendDataToAll (data, GameCenterDataSendType.RELIABLE);
		}


		if(GUI.Button(new Rect(170, 190, 150, 50), "Send to Player")) {
			string msg = "hello world";
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
			byte[] data = encoding.GetBytes(msg);

			GameCenterMultiplayer.instance.sendDataToPlayers (data, GameCenterDataSendType.RELIABLE, GameCenterMultiplayer.instance.match.playerIDs[0]);
		}

		if(GUI.Button(new Rect(170, 250, 150, 50), "Disconnect")) {
			GameCenterMultiplayer.instance.disconnect ();
		}

#endif


		//turn based
	/*	if(GUI.Button(new Rect(330, 70, 150, 50), "Trun Based Match")) {
			GameCenterMultiplayer.instance.FindTurnBasedMatch (2, 2);
		} */

	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnGCPlayerConnected(CEvent e) {
		string playerID = e.data as string;
		IOSNativePopUpManager.showMessage ("Player Connected", "playerid: " + playerID);
	}

	private void OnGCPlayerDisconnected(CEvent e) {
		string playerID = e.data as string;
		IOSNativePopUpManager.showMessage ("Player Disconnected", "playerid: " + playerID);
	}

	private void OnGCMatchStart(CEvent e) {
		GameCenterMatchData match = e.data as GameCenterMatchData;

		IOSNativePopUpManager.showMessage ("OnMatchStart", "let's playe now \n  Other player count: " + match.playerIDs.Count);
	}

	private void OnGCDataReceived(CEvent e) {
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		GameCenterDataPackage package = e.data as GameCenterDataPackage;

		System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
		string str = enc.GetString(package.buffer);

		IOSNativePopUpManager.showMessage ("Data received", "player ID: " + package.playerID + " \n " + "data: " + str);
#endif

	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
