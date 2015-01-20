////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class NetwrokManager  {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public static void send(BasePackage pack) {
		GameCenterMultiplayer.instance.SendDataToAll (pack.getBytes(), GameCenterDataSendType.RELIABLE);
	}


	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
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
