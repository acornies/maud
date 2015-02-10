// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;
namespace LegendPeak.Native
{
	public class AppleStoreManager : IStoreManager
	{
		public AppleStoreManager ()
		{
			GameCenterManager.OnAuthFinished += OnAuthFinished;
			GameCenterManager.init();

			IOSInAppPurchaseManager.instance.OnStoreKitInitComplete += OnStoreKitInitComplete;
			IOSInAppPurchaseManager.instance.OnTransactionComplete += OnAppleTransactionComplete;
			
			IOSInAppPurchaseManager.instance.loadStore();
		}

		public bool authenticated { get; private set;}

		public void buyProduct(string productId)
		{
			IOSInAppPurchaseManager.instance.buyProduct (productId);
		}

		public void showLeaderboards ()
		{
			if (!authenticated) return;
			GameCenterManager.showLeaderBoards ();
		}

		public void showLeaderboard(string leaderboardId)
		{
			if (!authenticated) return;
			GameCenterManager.showLeaderBoard (leaderboardId);
		}

		public void submitScore(int score, string leaderboardId)
		{
			if (!authenticated) return;
			GameCenterManager.OnScoreSubmited += OnScoreSubmited;
			GameCenterManager.reportScore (score, leaderboardId);
		}

		public void loadPlayerScore(string leaderboardId)
		{
			if (!authenticated) return;
			GameCenterManager.loadCurrentPlayerScore (leaderboardId);
		}

		public void screenCaptureAndShare()
		{
			IOSCamera.instance.OnImageSaved += OnImageSaved;
			IOSCamera.instance.SaveScreenshotToCameraRoll();		
		}

		private void OnImageSaved (ISN_Result result) {
			IOSCamera.instance.OnImageSaved -= OnImageSaved;
			if(result.IsSucceeded) {
				//IOSMessage.Create("Success", "Image successfully saved to Camera Roll");
				IOSCamera.instance.OnImagePicked += OnImage;
				IOSCamera.instance.GetImageFromAlbum();
			} else {
				IOSMessage.Create("Failed", "Image Save Failed");
			}
		}

		private void OnImage (IOSImagePickResult result) {
			if(result.IsSucceeded) 
			{
				//drawTexgture = result.image;
				Debug.Log("Launch social share.");
				IOSSocialManager.instance.ShareMedia(string.Format("I reached {0} platforms in MAUD! #maudgame maudgame.com", 
				                                                   GameController.Instance.highestPoint), result.image);
			}
			IOSCamera.instance.OnImagePicked -= OnImage;
		}

		public event EventHandler OnTransactionComplete;
		public event EventHandler OnAuthenticationFinished;

		void OnAuthFinished(ISN_Result res)
		{
			if (res.IsSucceeded) 
			{
				authenticated = true;

				//IOSNativePopUpManager.showMessage("Player Authored ", "ID: " + GameCenterManager.player.playerId + "\n" + "Alias: " + GameCenterManager.player.alias);
			} 
			else 
			{
				IOSNativePopUpManager.showMessage("Game Center", "Not logged in.");
			}
		}

		private static void OnStoreKitInitComplete (ISN_Result result) {
			IOSInAppPurchaseManager.instance.OnStoreKitInitComplete -= OnStoreKitInitComplete;
			
			if(result.IsSucceeded) {
				Debug.Log("Inited successfully, Avaliable products count: " + IOSInAppPurchaseManager.instance.products.Count.ToString());
			} else {
				Debug.Log("StoreKit Init Failed.  Error code: " + result.error.code + "\n" + "Error description:" + result.error.description);
			}
		}

		private void OnScoreSubmited (ISN_Result result) {
			GameCenterManager.OnScoreSubmited -= OnScoreSubmited;
			if(result.IsSucceeded)  {
				Debug.Log("Score Submited");
			} else {
				Debug.Log("Score Submit Failed");
			}
		}

		private void OnAppleTransactionComplete (IOSStoreKitResponse response) {

			var storeResponse = new AppleStoreResponse () { productId = response.productIdentifier };
			Debug.Log("OnTransactionComplete: " + response.productIdentifier);
			Debug.Log("OnTransactionComplete: state: " + response.state);
			
			switch(response.state) {
				case InAppPurchaseState.Purchased:
				case InAppPurchaseState.Restored:
					//Our product been succsesly purchased or restored
					//So we need to provide content to our user 
					//depends on productIdentifier
					//UnlockProducts(response.productIdentifier);
					storeResponse.status = StoreResponseStatus.Success;
					//storeResponse.productId = response.productIdentifier;
					break;
				case InAppPurchaseState.Deferred:
					//iOS 8 introduces Ask to Buy, which lets 
					//parents approve any purchases initiated by children
					//You should update your UI to reflect this 
					//deferred state, and expect another Transaction 
					//Complete  to be called again with a new transaction state 
					//reflecting the parent's decision or after the 
					//transaction times out. Avoid blocking your UI 
					//or gameplay while waiting for the transaction to be updated.
					storeResponse.status = StoreResponseStatus.Deferred;
					storeResponse.message = "Your purchase is waiting for approval from a parent or guardian.";
					IOSNativePopUpManager.showMessage("Purchase Requested", storeResponse.message);
					break;
				case InAppPurchaseState.Failed:
					//Our purchase flow is failed.
					//We can unlock interface and report user that the purchase is failed. 
					Debug.Log("Transaction failed with error, code: " + response.error.code);
					Debug.Log("Transaction failed with error, description: " + response.error.description);
					storeResponse.status = StoreResponseStatus.Failed;
					storeResponse.message = response.error.description;
					IOSNativePopUpManager.showMessage("Purchase Failed", storeResponse.message);
					break;
			}

			if (OnTransactionComplete != null)
			{
				OnTransactionComplete (this, storeResponse);
			}
		}
	}
}

