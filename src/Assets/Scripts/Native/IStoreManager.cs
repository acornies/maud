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
namespace LegendPeak.Native
{
	public interface IStoreManager
	{
		string leaderboardHighestName { get; }
		string leaderboardTotalName { get; }
		string purchaseContinuationsIdentifier { get; }
		string purchaseMusicIdentifier { get; }
		bool authenticated { get; }
		void buyProduct(string productIdentifier);
		void restoreProducts();
		void showLeaderboards();
		void showLeaderboard(string leaderboardIdentifier);
		void submitScore(int score, string leaderboardIdentifier);
		void loadPlayerScore(string leaderboardIdentifier);
		void screenCapture();
		void socialShare();
		void rateUs(string title, string message, string rate, string remind, string decline);

		//delegate void OnTransactionComplete(IStoreResponse response);
		event EventHandler OnTransactionComplete;
		event EventHandler OnAuthenticationFinished;
		event EventHandler OnRestoreComplete;
	}
}

