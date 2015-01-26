//#define PUSH_ENABLED
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using System;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
using UnityEngine;
#else
using UnityEngine.iOS;
#endif


public class IOSNotificationController : ISN_Singleton<IOSNotificationController>
{


	private static IOSNotificationController _instance;

	private static int _AllowedNotificationsType = -1;



	//Events
	public const string DEVICE_TOKEN_RECEIVED = "device_token_received";
	public const string REMOTE_NOTIFICATION_RECEIVED = "remote_notification_received";
	public const string NOTIFICATION_SCHEDULE_RESULT = "notification_schedule_result";

	//Actions
	public Action<IOSNotificationDeviceToken> OnDeviceTokenReceived = delegate {};
	public Action<ISN_Result>  OnNotificationScheduleResult = delegate {};
	#if (UNITY_IPHONE && !UNITY_EDITOR && PUSH_ENABLED) || SA_DEBUG_MODE
	[NonSerialized]
	public Action<RemoteNotification> OnRemoteNotificationReceived = delegate {};
	#endif



	private const string PP_ID_KEY = "IOSNotificationControllerKey_ID";

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_ScheduleNotification (int time, string message, bool sound, string nId, int badges);
	
	[DllImport ("__Internal")]
	private static extern  void _ISN_ShowNotificationBanner (string title, string messgae);
	
	[DllImport ("__Internal")]
	private static extern void _ISN_CancelNotifications();


	[DllImport ("__Internal")]
	private static extern void _ISN_RequestNotificationPermitions();

	[DllImport ("__Internal")]
	private static extern void _ISN_CancelNotificationById(string nId);

	[DllImport ("__Internal")]
	private static extern  void _ISN_ApplicationIconBadgeNumber (int badges);


	[DllImport ("__Internal")]
	private static extern void _ISN_RegisterForRemoteNotifications(int types);

	#endif

	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}



	#if (UNITY_IPHONE && !UNITY_EDITOR && PUSH_ENABLED) || SA_DEBUG_MODE
	void FixedUpdate() {
		if(NotificationServices.remoteNotificationCount > 0) {
			foreach(var rn in NotificationServices.remoteNotifications) {
				if(!IOSNativeSettings.Instance.DisablePluginLogs) 
					UnityEngine.Debug.Log("Remote Noti: " + rn.alertBody);
				IOSNotificationController.instance.ShowNotificationBanner("", rn.alertBody);
				dispatch(REMOTE_NOTIFICATION_RECEIVED, rn);
				OnRemoteNotificationReceived(rn);
			}
			NotificationServices.ClearRemoteNotifications();
		}
	}
	#endif




	#if UNITY_IPHONE
	public void RegisterForRemoteNotifications(RemoteNotificationType notificationTypes) {

		#if (UNITY_IPHONE && !UNITY_EDITOR && PUSH_ENABLED) || SA_DEBUG_MODE

		string sysInfo = SystemInfo.operatingSystem;
		sysInfo = sysInfo.Replace("iPhone OS ", "");
		string[] chunks = sysInfo.Split('.');
		int majorVersion = int.Parse(chunks[0]);
		if (majorVersion >= 8) {
			_ISN_RegisterForRemoteNotifications((int) notificationTypes);
		} 

		NotificationServices.RegisterForRemoteNotificationTypes(notificationTypes);

		DeviceTokenListner.Create ();

		#endif
	}
	#endif

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public void RequestNotificationPermitions() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_RequestNotificationPermitions ();
		#endif

	}
	
	public void ShowNotificationBanner (string title, string messgae) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ShowNotificationBanner (title, messgae);
		#endif
	}

	[System.Obsolete("CancelNotifications is deprecated, please use CancelAllLocalNotifications instead.")]
	public void CancelNotifications () {
		CancelAllLocalNotifications();
	}


	public void CancelAllLocalNotifications () {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_CancelNotifications();
		#endif
	}

	public void CancelLocalNotificationById (int notificationId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_CancelNotificationById(notificationId.ToString());
		#endif
	}

	public int ScheduleNotification (int time, string message, bool sound, int badges = 0) {
		int nid = GetNextId;

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		string notificationId = nid.ToString();
		_ISN_ScheduleNotification (time, message, sound, notificationId, badges);
		#endif

		return nid;
	}

	public void ApplicationIconBadgeNumber (int badges) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISN_ApplicationIconBadgeNumber (badges);
		#endif

	}

	
	


	


	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------


	private int GetNextId {
		get {
			int id = 1;
			if(UnityEngine.PlayerPrefs.HasKey(PP_ID_KEY)) {
				id = UnityEngine.PlayerPrefs.GetInt(PP_ID_KEY);
				id++;
			} 
			
			UnityEngine.PlayerPrefs.SetInt(PP_ID_KEY, id);
			return id;
		}
		
	}

	public static int AllowedNotificationsType {
		get {
			return _AllowedNotificationsType;
		}
	}
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	public void OnDeviceTockeReceivedAction (IOSNotificationDeviceToken token) {
		dispatch (DEVICE_TOKEN_RECEIVED, token);
		OnDeviceTokenReceived(token);
	}

	private void OnNotificationScheduleResultAction (string array) {

		string[] data;
		data = array.Split("|" [0]);


		ISN_Result result = null;

		if(data[0].Equals("0")) {
			result =  new ISN_Result(false);
		} else {
			result =  new ISN_Result(true);
		}


		_AllowedNotificationsType = System.Convert.ToInt32(data[1]);

		OnNotificationScheduleResult(result);
		dispatch(NOTIFICATION_SCHEDULE_RESULT, result);

	
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------




	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
