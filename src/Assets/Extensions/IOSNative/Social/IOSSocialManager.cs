//#define SA_DEBUG_MODE
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using System;
using UnityEngine;
using UnionAssets.FLE;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSSocialManager : EventDispatcher {

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_TwPost(string text);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_TwPostWithMedia(string text, string encodedMedia);
	

	[DllImport ("__Internal")]
	private static extern void _ISN_FbPost(string text);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_FbPostWithMedia(string text, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_MediaShare(string text, string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_SendMail(string subject, string body,  string recipients, string encodedMedia);


	#endif

	private static IOSSocialManager _instance = null;


	//Actions
	public Action<ISN_Result> OnFacebookPostResult;
	public Action<ISN_Result> OnTwitterPostResult;
	public Action<ISN_Result> OnMailResult;
	
	//Events

	public const string TWITTER_POST_FAILED  = "twitter_post_failed";
	public const string TWITTER_POST_SUCCESS = "twitter_post_success";
	
	public const string FACEBOOK_POST_FAILED  = "facebook_post_failed";
	public const string FACEBOOK_POST_SUCCESS = "facebook_post_success";

	public const string MAIL_FAILED  = "mail_failed";
	public const string MAIL_SUCCESS = "mail_success";


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void ShareMedia(string text) {
		ShareMedia(text, null);
	}

	public void ShareMedia(string text, Texture2D texture) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			if(texture != null) {
				byte[] val = texture.EncodeToPNG();
				string bytesString = System.Convert.ToBase64String (val);
				_ISN_MediaShare(text, bytesString);
			} else {
				_ISN_MediaShare(text, "");
			}
		#endif
	}

	public void TwitterPost(string text) {
		TwitterPost(text, null);
	}


	public void TwitterPost(string text, Texture2D texture) {
		if(texture == null) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
				_ISN_TwPost(text);
			#endif
		} else {


			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
				byte[] val = texture.EncodeToPNG();
				string bytesString = System.Convert.ToBase64String (val);

				_ISN_TwPostWithMedia(text, bytesString);
			#endif
		}

	}


	public void FacebookPost(string text) {
		FacebookPost(text, null);
	}
	
	public void FacebookPost(string text, Texture2D texture) {
		if(texture == null) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
				_ISN_FbPost(text);
			#endif
		} else {

			
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
				byte[] val = texture.EncodeToPNG();
				string bytesString = System.Convert.ToBase64String (val);
				_ISN_FbPostWithMedia(text, bytesString);
			#endif
		}
	}


	public void SendMail(string subject, string body, string recipients) {
		SendMail(subject, body, recipients, null);
	}
	
	public void SendMail(string subject, string body, string recipients, Texture2D texture) {
		if(texture == null) {
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_SendMail(subject, body, recipients, "");
			#endif
		} else {
			
			
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			byte[] val = texture.EncodeToPNG();
			string bytesString = System.Convert.ToBase64String (val);
			_ISN_SendMail(subject, body, recipients, bytesString);
			#endif
		}
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public static IOSSocialManager instance  {
		get {
			if(_instance == null) {
				GameObject go =  new GameObject("IOSSocialManager");
				_instance = go.AddComponent<IOSSocialManager>();
			}

			return _instance;
		}
	}
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnTwitterPostFailed() {
		dispatch(TWITTER_POST_FAILED);

		if(OnTwitterPostResult != null) {
			ISN_Result result = new ISN_Result(false);
			OnTwitterPostResult(result);
		}
	}

	private void OnTwitterPostSuccess() {
		dispatch(TWITTER_POST_SUCCESS);

		if(OnTwitterPostResult != null) {
			ISN_Result result = new ISN_Result(true);
			OnTwitterPostResult(result);
		}

	}

	private void OnFacebookPostFailed() {
		dispatch(FACEBOOK_POST_FAILED);

		if(OnFacebookPostResult != null) {
			ISN_Result result = new ISN_Result(false);
			OnFacebookPostResult(result);
		}
	}
	
	private void OnFacebookPostSuccess() {
		dispatch(FACEBOOK_POST_SUCCESS);

		if(OnFacebookPostResult != null) {
			ISN_Result result = new ISN_Result(true);
			OnFacebookPostResult(result);
		}
	}

	private void OnMailFailed() {
		dispatch(MAIL_FAILED);

		if(OnMailResult != null) {
			ISN_Result result = new ISN_Result(false);
			OnMailResult(result);
		}
	}

	private void OnMailSuccess() {
		dispatch(MAIL_SUCCESS);

		if(OnMailResult != null) {
			ISN_Result result = new ISN_Result(true);
			OnMailResult(result);
		}
	}


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------


	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
