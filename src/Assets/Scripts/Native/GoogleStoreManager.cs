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
	public class GoogleStoreManager : IStoreManager
	{
		public GoogleStoreManager ()
		{
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

		public event EventHandler OnTransactionComplete;
		public event EventHandler OnAuthenticationFinished;
	}
}

