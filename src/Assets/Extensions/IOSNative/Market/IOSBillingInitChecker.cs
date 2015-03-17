using UnityEngine;
using System.Collections;

public class IOSBillingInitChecker 
{
	public delegate void BillingInitListener();

	BillingInitListener _listener;


	public IOSBillingInitChecker(BillingInitListener listener) {
		_listener = listener;

		if(IOSInAppPurchaseManager.instance.IsStoreLoaded) {
			_listener();
		} else {

			IOSInAppPurchaseManager.instance.addEventListener(IOSInAppPurchaseManager.STORE_KIT_INITIALIZED, OnStoreKitInit);
			if(!IOSInAppPurchaseManager.instance.IsWaitingLoadResult) {
				IOSInAppPurchaseManager.instance.loadStore();
			}
		}
	}

	private void OnStoreKitInit() {
		IOSInAppPurchaseManager.instance.removeEventListener(IOSInAppPurchaseManager.STORE_KIT_INITIALIZED, OnStoreKitInit);
		_listener();
	}

}

