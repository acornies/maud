////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class iCloudManager : ISN_Singleton<iCloudManager> {


	//Events
	public const string CLOUD_INITIALIZED	        = "cloud_initialized";
	public const string CLOUD_INITIALIZE_FAILED	    = "cloud_initialize_failed";

	public const string CLOUD_DATA_CHANGED	        = "cloud_data_changed";
	public const string CLOUD_DATA_RECEIVE       = "cloud_data_receive";

	//Actions
	public Action<ISN_Result> OnCloundInitAction = delegate {};
	public Action OnCloundDataChangedAction = delegate {};
	public Action<iCloudData> OnCloudDataReceivedAction = delegate {};


	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _initCloud();

	[DllImport ("__Internal")]
	private static extern void _setString(string key, string val);

	[DllImport ("__Internal")]
	private static extern void _setDouble(string key, float val);
	
	[DllImport ("__Internal")]
	private static extern void _setData(string key, string val);

	[DllImport ("__Internal")]
	private static extern void _requestDataForKey(string key);
	#endif
	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void init() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_initCloud ();
		#endif
	}

	public void setString(string key, string val) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_setString(key, val);
		#endif
	}

	public void setFloat(string key, float val) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_setDouble(key, val);
		#endif
	}

	public void setData(string key, byte[] val) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

			string b = "";
			int len = val.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					b += ",";
				}

				b += val[i].ToString();
			}

			_setData(key, b);
		#endif
	}

	public void requestDataForKey(string key) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_requestDataForKey(key);
		#endif
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

	private void OnCloudInit() {
		ISN_Result res =  new ISN_Result(true);
		OnCloundInitAction(res);
		dispatch (CLOUD_INITIALIZED);
	}

	private void OnCloudInitFail() {
		ISN_Result res =  new ISN_Result(false);
		OnCloundInitAction(res);
		dispatch (CLOUD_INITIALIZE_FAILED);
	}

	private void OnCloudDataChanged() {
		OnCloundDataChangedAction();
		dispatch (CLOUD_DATA_CHANGED);
	}


	private void OnCloudData(string array) {
		string[] data;
		data = array.Split("|" [0]);

		iCloudData package = new iCloudData (data[0], data [1]);

		OnCloudDataReceivedAction(package);
		dispatch (CLOUD_DATA_RECEIVE, package);
	}

	private void OnCloudDataEmpty(string array) {
		string[] data;
		data = array.Split("|" [0]);

		iCloudData package = new iCloudData (data[0], "null");


		OnCloudDataReceivedAction(package);
		dispatch (CLOUD_DATA_RECEIVE, package);
	}



	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
