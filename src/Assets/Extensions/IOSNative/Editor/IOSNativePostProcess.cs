using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

public class IOSNativePostProcess  {

	#if UNITY_IPHONE
	[PostProcessBuild(50)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {


		string StoreKit = "StoreKit.framework";
		if(!ISDSettings.Instance.frameworks.Contains(StoreKit)) {
			ISDSettings.Instance.frameworks.Add(StoreKit);
		}


		string Accounts = "Accounts.framework";
		if(!ISDSettings.Instance.frameworks.Contains(Accounts)) {
			ISDSettings.Instance.frameworks.Add(Accounts);
		}

		string SocialF = "Social.framework";
		if(!ISDSettings.Instance.frameworks.Contains(SocialF)) {
			ISDSettings.Instance.frameworks.Add(SocialF);
		}

		string MessageUI = "MessageUI.framework";
		if(!ISDSettings.Instance.frameworks.Contains(MessageUI)) {
			ISDSettings.Instance.frameworks.Add(MessageUI);
		}


		string MediaPlayer = "MediaPlayer.framework";
		if(!ISDSettings.Instance.frameworks.Contains(MediaPlayer)) {
			ISDSettings.Instance.frameworks.Add(MediaPlayer);
		}


		string MobileCoreServices = "MobileCoreServices.framework";
		if(!ISDSettings.Instance.frameworks.Contains(MobileCoreServices)) {
			ISDSettings.Instance.frameworks.Add(MobileCoreServices);
		}


	
	}
	#endif
}
