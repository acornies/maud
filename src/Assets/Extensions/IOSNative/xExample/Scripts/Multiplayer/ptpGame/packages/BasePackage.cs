////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.IO;

public abstract class BasePackage  {

	protected MemoryStream buffer = new MemoryStream();
	protected BinaryWriter writer;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public void initWriter() {
		writer = new BinaryWriter (buffer);
	}


	public abstract int getId();




	public byte[] getBytes() {
		return buffer.ToArray ();
	}

	public void send() {

		NetwrokManager.send (this);
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void writeInt(int val) {
		writer.Write (val);
	}
	
	public void writeString(string val) {

		//writer.Write (val);
	}

	public void writeFloat(float val) {
		writer.Write (val);
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
