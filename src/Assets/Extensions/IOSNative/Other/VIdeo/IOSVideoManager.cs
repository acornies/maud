//#define SA_DEBUG_MODE
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
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSVideoManager : ISN_Singleton<IOSVideoManager>  {

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
	[DllImport ("__Internal")]
	private static extern void _ISN_SteamVideo(string videoUrl);
	

	[DllImport ("__Internal")]
	private static extern void _ISN_OpenYoutubeVideo(string videoUrl);
	
	#endif



	public void PlaySteamingVideo(string videoUrl) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_SteamVideo(videoUrl);
		#endif
	}
	
	public void OpenYoutubeVideo(string videoUrl) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_OpenYoutubeVideo(videoUrl);
		#endif
	}
}
