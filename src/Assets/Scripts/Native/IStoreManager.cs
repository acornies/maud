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
		bool authenticated { get; }
		void buyProduct(string productIdentifier);
		void showLeaderboards();
		void showLeaderboard(string leaderboardIdentifier);
		void submitScore(int score, string leaderboardIdentifier);
		void loadPlayerScore(string leaderboardIdentifier);
		//delegate void OnTransactionComplete(IStoreResponse response);
		event EventHandler OnTransactionComplete;
		event EventHandler OnAuthenticationFinished;
	}
}

