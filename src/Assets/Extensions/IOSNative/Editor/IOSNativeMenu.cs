////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

public class IOSNativeMenu : EditorWindow {
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	#if UNITY_EDITOR


	//--------------------------------------
	//  EDIT
	//--------------------------------------

	[MenuItem("Window/IOS Native/Edit Settings", false, 1)]
	public static void Edit() {
		Selection.activeObject = IOSNativeSettings.Instance;
	}

	//--------------------------------------
	//  Setup
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/Setup/Plugin set up")]
	public static void SetupPluginSetUp() {
		string url = "https://unionassets.com/iosnative/plugin-set-up-2";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Setup/Certificate and Provisioning")]
	public static void SetupCertificateAndProvisioning() {
		string url = "https://unionassets.com/iosnative/certificate-and-provisioning-5";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Setup/Creating iTunes app")]
	public static void SetupCreatingITunesApp() {
		string url = "https://unionassets.com/iosnative/creating-itunes-app-6";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  In-App Purchases
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/In-App Purchases/Manage In-App Purchases")]
	public static void InAppManagePurchases() {
		string url = "https://unionassets.com/iosnative/manage-in-app-purchases-8";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/In-App Purchases/Coding Guidelines")]
	public static void InAppCodingGuidelines() {
		string url = "https://unionassets.com/iosnative/coding-guidelines-15";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/In-App Purchases/Transactions Validation")]
	public static void InAppTransactionsValidation() {
		string url = "https://unionassets.com/iosnative/transactions-validation-16";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/In-App Purchases/Restoring Purchases")]
	public static void InAppRestoringPurchases() {
		string url = "https://unionassets.com/iosnative/restoring-purchases-21";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  Game Center
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/Game Center/Manage Game Center")]
	public static void GameCenterManage() {
		string url = "https://unionassets.com/iosnative/manage-game-center-7";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/IOS Native/Documentation/Game Center/Init The Game Center")]
	public static void GameCenterInit() {
		string url = "https://unionassets.com/iosnative/init-the-game-center-44";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Game Center/Leaderboards")]
	public static void GameCenterLeaderboards() {
		string url = "https://unionassets.com/iosnative/leaderboards-45";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Game Center/Achievements")]
	public static void GameCenterAchievements() {
		string url = "https://unionassets.com/iosnative/achievements-46";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Game Center/Challenges")]
	public static void GameCenterChallenges() {
		string url = "https://unionassets.com/iosnative/challenges-47";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Game Center/Friends")]
	public static void GameCenterFriends() {
		string url = "https://unionassets.com/iosnative/friends-48";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Game Center/Real-Time Multiplayer")]
	public static void GameCenterRealTimeMultiplayer() {
		string url = "https://unionassets.com/iosnative/real-time-multiplayer-49";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  iAd App Network
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/iAd App Network/iAd Setup")]
	public static void iAdAppSetup() {
		string url = "https://unionassets.com/iosnative/iad-setup-19";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/iAd App Network/Coding Guidelines")]
	public static void iAdAppNetworkCodingGuidelines() {
		string url = "https://unionassets.com/iosnative/coding-guidelines-20";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  iCloud
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/iCloud/iCloud Setup")]
	public static void iCloudSetup() {
		string url = "https://unionassets.com/iosnative/icloud-setup-25";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/iCloud/Coding Guidelines")]
	public static void iCloudCodingGuidelines() {
		string url = "https://unionassets.com/iosnative/coding-guidelines-26";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  More Features
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/More Features/Social Sharing")]
	public static void FeaturesSocialSharing() {
		string url = "https://unionassets.com/iosnative/social-sharing-9";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/IOS Native/Documentation/More Features/Camera And Gallery")]
	public static void FeaturesCameraAndGallery() {
		string url = "https://unionassets.com/iosnative/camera-and-gallery-10";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/IOS Native/Documentation/More Features/Shared App Url")]
	public static void FeaturesSharedAppUrl() {
		string url = "https://unionassets.com/iosnative/shared-app-url-11";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/IOS Native/Documentation/More Features/Local Notifications")]
	public static void FeaturesLocalNotifications() {
		string url = "https://unionassets.com/iosnative/local-notifications-12";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/More Features/Push Notifications")]
	public static void FeaturesPushNotifications() {
		string url = "https://unionassets.com/iosnative/push-notifications-14";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/More Features/Poups and Pre-loaders")]
	public static void FeaturesPoupsPreloaders() {
		string url = "https://unionassets.com/iosnative/poups-and-pre-loaders-24";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/More Features/Native System Events")]
	public static void FeaturesNativeSystemEvents() {
		string url = "https://unionassets.com/iosnative/native-system-events-33";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/More Features/Video API")]
	public static void FeaturesVideoAPI() {
		string url = "https://unionassets.com/iosnative/video-api-73";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  PLAYMAKER
	//--------------------------------------
	
	[MenuItem("Window/IOS Native/Documentation/Playmaker/Actions List")]
	public static void PlaymakerActionsList() {
		string url = "https://unionassets.com/iosnative/actions-list-18";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Playmaker/iAd With Playmaker")]
	public static void PlaymakerIAd() {
		string url = "https://unionassets.com/iosnative/iad-with-playmaker-22";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Playmaker/IOS Native InApp Purchasing with Playmaker")]
	public static void PlaymakerInAppPurchasing() {
		string url = "https://unionassets.com/iosnative/ios-native-inapp-purchasing-with-playmaker-28";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  NOTES
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/Notes/Released Apps with the plugin")]
	public static void NotesReleasedApps() {
		string url = "https://unionassets.com/iosnative/released-apps-with-the-plugin-29";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/IOS Native/Documentation/Notes/Version 5.0")]
	public static void NotesVersion5_0() {
		string url = "https://unionassets.com/iosnative/version-5-0-30";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/IOS Native/Documentation/Notes/Version 5.1")]
	public static void NotesVersion5_1() {
		string url = "https://unionassets.com/iosnative/version-5-1-31";
		Application.OpenURL(url);
	}

	[MenuItem("Window/IOS Native/Documentation/Notes/Version 5.3")]
	public static void NotesVersion5_3() {
		string url = "https://unionassets.com/iosnative/version-5-3-32";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  TROUBLESHOOTING
	//--------------------------------------

	[MenuItem("Window/IOS Native/Documentation/Troubleshooting")]
	public static void Troubleshooting() {
		string url = "https://unionassets.com/iosnative/manual";
		Application.OpenURL(url);
	}	

	#endif

}
