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
	public class GoogleStoreManager : IStoreManager
	{
		public GoogleStoreManager ()
		{
			ImmersiveMode.instance.EnableImmersiveMode();
		}

		public bool authenticated { get; private set;}

		public void buyProduct(string productId)
		{
			//TODO: Implement
		}

		public void showLeaderboards ()
		{

		}
		
		public void showLeaderboard(string leaderboardId)
		{

		}
		
		public void submitScore(int score, string leaderboardId)
		{

		}
		
		public void loadPlayerScore(string leaderboardId)
		{

		}

		public void screenCapture()
		{
			AndroidCamera.instance.OnImageSaved += OnImageSaved;
			AndroidCamera.instance.SaveScreenshotToGallery();
		}

		public void socialShare()
		{
			AndroidSocialGate.StartShareIntent("Share Maud", string.Format("I reached {0} platforms in Maud. #maudgame maudgame.com"));
		}

		void OnImageSaved (GallerySaveResult result) {
			AndroidCamera.instance.OnImageSaved -= OnImageSaved;
			if(result.IsSucceeded) {
				Debug.Log("Image saved to gallery \n" + "Path: " + result.imagePath);
			} else {
				Debug.Log("Image save to gallery failed");
			}
		}

		public event EventHandler OnTransactionComplete;
		public event EventHandler OnAuthenticationFinished;
	}
}

