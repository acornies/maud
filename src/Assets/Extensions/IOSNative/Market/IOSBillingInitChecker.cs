using UnityEngine;
using System.Collections;

public class IOSBillingInitChecker 
{
	public delegate void BilliginitListner();

	BilliginitListner _listner;


	public IOSBillingInitChecker(BilliginitListner listner) {
		_listner = listner;

		if(IOSInAppPurchaseManager.instance.IsStoreLoaded) {
			_listner();
		} else {

			IOSInAppPurchaseManager.instance.addEventListener(IOSInAppPurchaseManager.STORE_KIT_INITIALIZED, OnStoreKitInit);
			if(!IOSInAppPurchaseManager.instance.IsWaitingLoadResult) {
				IOSInAppPurchaseManager.instance.loadStore();
			}
		}
	}

	private void OnStoreKitInit() {
		IOSInAppPurchaseManager.instance.removeEventListener(IOSInAppPurchaseManager.STORE_KIT_INITIALIZED, OnStoreKitInit);
		_listner();
	}

}

