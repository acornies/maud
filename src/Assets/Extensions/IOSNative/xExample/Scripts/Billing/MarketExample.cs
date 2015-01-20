////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarketExample : BaseIOSFeaturePreview {

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {

		//Best pertise is to init billing on app launch
		//But for this example we will use button for initialization
		//PaymentManagerExample.init();
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	void OnGUI() {




		UpdateToStartPos();
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "In-App Purchases", style);



		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Init")) {
			PaymentManagerExample.init();
		}


		if(IOSInAppPurchaseManager.instance.IsStoreLoaded) {
			GUI.enabled = true;
		} else {
			GUI.enabled = false;
		}


		StartX = XStartPos;
		StartY+= YButtonStep;

		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #1")) {
			PaymentManagerExample.buyItem(PaymentManagerExample.SMALL_PACK);

		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Perform Buy #2")) {
			PaymentManagerExample.buyItem(PaymentManagerExample.NC_PACK);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Restore Purchases")) {
			IOSInAppPurchaseManager.instance.restorePurchases();

		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Verify Last Purchases")) {
			IOSInAppPurchaseManager.instance.verifyLastPurchase(IOSInAppPurchaseManager.SANDBOX_VERIFICATION_SERVER);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Load Product View")) {
			IOSStoreProductView view =  new IOSStoreProductView("333700869");
			view.Load();
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Is Payments Enabled On device")) {
			IOSInAppPurchaseManager.instance.OnPurchasesStateSettingsLoaded += OnPurchasesStateSettingsLoaded;
			IOSInAppPurchaseManager.instance.RequestInAppSettingState();
		}
	}
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	void OnPurchasesStateSettingsLoaded (bool IsInAppPurchasesEnabled) {
		IOSInAppPurchaseManager.instance.OnPurchasesStateSettingsLoaded -= OnPurchasesStateSettingsLoaded;
		IOSNativePopUpManager.showMessage("Payments Settings State", "Is Payments Enabled: " + IOSInAppPurchaseManager.instance.IsInAppPurchasesEnabled);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------



}
