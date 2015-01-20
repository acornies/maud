////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
using UnityEngine;
#else
using UnityEngine.iOS;
#endif


using UnionAssets.FLE;
using System.Collections;

public class NotificationExample : BaseIOSFeaturePreview {


	private int lastNotificationId = 0;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		IOSNotificationController.instance.RequestNotificationPermitions();
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {

		UpdateToStartPos();
		
		UnityEngine.GUI.Label(new UnityEngine.Rect(StartX, StartY, UnityEngine.Screen.width, 40), "Local and Push Notifications", style);
		
		
		StartY+= YLableStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Schedule Notification Silet")) {
			IOSNotificationController.instance.OnNotificationScheduleResult += OnNotificationScheduleResult;
			lastNotificationId = IOSNotificationController.instance.ScheduleNotification (5, "Your Notification Text No Sound", false);
		}

		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Schedule Notification")) {
			IOSNotificationController.instance.OnNotificationScheduleResult += OnNotificationScheduleResult;
			lastNotificationId = IOSNotificationController.instance.ScheduleNotification (5, "Your Notification Text", true);
		}


		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Cancel All Notifications")) {
			IOSNotificationController.instance.CancelAllLocalNotifications();
		}

		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Cansel Last Notification")) {
			IOSNotificationController.instance.CancelLocalNotificationById(lastNotificationId);
		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Reg Device For Push Notif. ")) {



			#if UNITY_IPHONE

			IOSNotificationController.instance.RegisterForRemoteNotifications (RemoteNotificationType.Alert |  RemoteNotificationType.Badge |  RemoteNotificationType.Sound);
			IOSNotificationController.instance.addEventListener (IOSNotificationController.DEVICE_TOKEN_RECEIVED, OnTokenReived);
			#endif


		}

		StartX += XButtonStep;
		if(UnityEngine.GUI.Button(new UnityEngine.Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Notification Banner")) {
			IOSNotificationController.instance.ShowNotificationBanner("Title", "Message");
		}


	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnTokenReived(CEvent e) {
		IOSNotificationDeviceToken token = e.data as IOSNotificationDeviceToken;
		UnityEngine.Debug.Log ("OnTokenReived");
		UnityEngine.Debug.Log (token.tokenString);

		IOSDialog.Create("OnTokenReived", token.tokenString);

		IOSNotificationController.instance.removeEventListener (IOSNotificationController.DEVICE_TOKEN_RECEIVED, OnTokenReived);
	}

	private void OnNotificationScheduleResult (ISN_Result res) {
		IOSNotificationController.instance.OnNotificationScheduleResult -= OnNotificationScheduleResult;



		string msg = string.Empty;

		if(res.IsSucceeded) {
			msg += "Notification was successfully scheduled \n allowed notifications types: \n";
		

			if((IOSNotificationController.AllowedNotificationsType & IOSUIUserNotificationType.Alert) != 0) {
				msg += "Alert ";
			}

			if((IOSNotificationController.AllowedNotificationsType & IOSUIUserNotificationType.Sound) != 0) {
				msg += "Sound ";
			}

			if((IOSNotificationController.AllowedNotificationsType & IOSUIUserNotificationType.Badge) != 0) {
				msg += "Badge ";
			}

		} else {
			msg += "Notification scheduling failed";
		}


		IOSMessage.Create("On Notification Schedule Result", msg);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
