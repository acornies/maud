////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using UnionAssets.FLE;
using System.Collections.Generic;

public class PaymentManagerExample {
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public const string SMALL_PACK 	=  "your.product.id1.here";
	public const string NC_PACK 	=  "your.product.id2.here";
	


	private static bool IsInited = false;
	public static void init() {


		if(!IsInited) {

			//You do not have to add products by code if you already did it in seetings guid
			//Windows -> IOS Native -> Edit Settings
			//Billing tab.
			IOSInAppPurchaseManager.instance.addProductId(SMALL_PACK);
			IOSInAppPurchaseManager.instance.addProductId(NC_PACK);
			


			//Event Use Examples
			IOSInAppPurchaseManager.instance.addEventListener(IOSInAppPurchaseManager.RESTORE_TRANSACTION_FAILED, onRestoreTransactionFailed);
			IOSInAppPurchaseManager.instance.addEventListener(IOSInAppPurchaseManager.VERIFICATION_RESPONSE, onVerificationResponce);

			IOSInAppPurchaseManager.instance.OnStoreKitInitComplete += OnStoreKitInitComplete;


			//Action Use Examples
			IOSInAppPurchaseManager.instance.OnTransactionComplete += OnTransactionComplete;


			IsInited = true;

		} 

		IOSInAppPurchaseManager.instance.loadStore();


	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	public static void buyItem(string productId) {
		IOSInAppPurchaseManager.instance.buyProduct(productId);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------


	private static void UnlockProducts(string productIdentifier) {
		switch(productIdentifier) {
		case SMALL_PACK:
			//code for adding small game money amount here
			break;
		case NC_PACK:
			//code for unlocking cool item here
			break;
			
		}
	}

	private static void OnTransactionComplete (IOSStoreKitResponse responce) {

		Debug.Log("OnTransactionComplete: " + responce.productIdentifier);
		Debug.Log("OnTransactionComplete: state: " + responce.state);

		switch(responce.state) {
		case InAppPurchaseState.Purchased:
		case InAppPurchaseState.Restored:
			//Our product been succsesly purchased or restored
			//So we need to provide content to our user depends on productIdentifier
			UnlockProducts(responce.productIdentifier);
			break;
		case InAppPurchaseState.Deferred:
			//iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
			//You should update your UI to reflect this deferred state, and expect another Transaction Complete  to be called again with a new transaction state 
			//reflecting the parent’s decision or after the transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.
			break;
		case InAppPurchaseState.Failed:
			//Our purchase flow is failed.
			//We can unlock intrefase and repor user that the purchase is failed. 
			Debug.Log("Transaction failed with error, code: " + responce.error.code);
			Debug.Log("Transaction failed with error, description: " + responce.error.description);


			break;
		}

		if(responce.state == InAppPurchaseState.Failed) {
			IOSNativePopUpManager.showMessage("Transaction Failed", "Error code: " + responce.error.code + "\n" + "Error description:" + responce.error.description);
		} else {
			IOSNativePopUpManager.showMessage("Store Kit Response", "product " + responce.productIdentifier + " state: " + responce.state.ToString());
		}

	}
 
	private static void onRestoreTransactionFailed() {
		IOSNativePopUpManager.showMessage("Fail", "Restore Failed");
	}
	

	private static void onVerificationResponce(CEvent e) {
		IOSStoreKitVerificationResponse responce =  e.data as IOSStoreKitVerificationResponse;

		IOSNativePopUpManager.showMessage("Verification", "Transaction verification status: " + responce.status.ToString());

		Debug.Log("ORIGINAL JSON ON: " + responce.originalJSON);
	}

	private static void OnStoreKitInitComplete(ISN_Result result) {
		if(result.IsSucceeded) {
			IOSNativePopUpManager.showMessage("StoreKit Init Succeeded", "Available products cound: " + IOSInAppPurchaseManager.instance.products.Count.ToString());
		} else {
			IOSNativePopUpManager.showMessage("StoreKit Init Failed",  "Error code: " + result.error.code + "\n" + "Error description:" + result.error.description);
		}
	}

	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
