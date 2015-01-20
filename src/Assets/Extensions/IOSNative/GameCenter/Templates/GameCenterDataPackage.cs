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

public class GameCenterDataPackage  {

	private string _playerID;


	private byte[] _buffer;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GameCenterDataPackage(string player, string receivedData) {

		_playerID = player;


		string[] array;
		array = receivedData.Split("," [0]);

		List<byte> l = new List<byte> ();
		foreach(string s in array) {
			l.Add (System.Convert.ToByte(s));
		}

		_buffer = l.ToArray ();
	
	}



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------


	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public string playerID {
		get {
			return _playerID;
		}
	}




	public byte[] buffer {
		get {
			return _buffer;
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
