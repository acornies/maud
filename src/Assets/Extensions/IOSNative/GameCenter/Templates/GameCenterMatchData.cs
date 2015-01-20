////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCenterMatchData  {

	private List<string> _playerIDs =  new List<string> ();

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GameCenterMatchData(List<string> ids) {
		_playerIDs = ids;
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------


	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public List<string> playerIDs {
		get {
			return _playerIDs;
		}
	}
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
