////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using UnionAssets.FLE;


public class JSHelper : MonoBehaviour {
	
	private string leaderboardId =  "your.leaderboard1.id.here";


	private string TEST_ACHIEVEMENT_1_ID = "your.achievement.id1.here";
	private string TEST_ACHIEVEMENT_2_ID = "your.achievement.id2.here";

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------




	void InitGameCneter() {


		//Achievement registration. If you will skipt this step GameCenterManager.achievements array will contain only achievements with reported progress 
		GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_1_ID);
		GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_2_ID);


		//Listen for the Game Center events
		GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_ACHIEVEMENTS_LOADED, OnAchievementsLoaded);
		GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_ACHIEVEMENT_PROGRESS, OnAchievementProgress);
		GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_ACHIEVEMENTS_RESET, OnAchievementsReset);


		GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_LEADERBOARD_SCORE_LOADED, OnLeaderboardScoreLoaded);

		GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_PLAYER_AUTHENTICATED, OnAuth);

		DontDestroyOnLoad (gameObject);

		GameCenterManager.init();
		
		Debug.Log("InitGameCenter");
	}
	

	private void SubmitScore(int val) {
		Debug.Log("SubmitScore");
		GameCenterManager.ReportScore(val, leaderboardId);
	}
	
	private void SubmitAchievement(string data) {
		
		string[] arr;
		arr = data.Split("|" [0]);
		
		float percent = System.Convert.ToSingle(arr[0]);
		string achievementId = arr[1];

		
		
		Debug.Log("SubmitAchievement: " + achievementId + "  " + percent.ToString());
		GameCenterManager.SubmitAchievement(percent, achievementId);
	}
	
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnAchievementsLoaded() {
		Debug.Log ("Achievements loaded from iOS Game Center");

		foreach(AchievementTemplate tpl in GameCenterManager.Achievements) {
			Debug.Log (tpl.id + ":  " + tpl.progress);
		}
	}

	private void OnAchievementsReset() {
		Debug.Log ("All Achievements were reset");
	}

	private void OnAchievementProgress(CEvent e) {
		Debug.Log ("OnAchievementProgress");

		AchievementTemplate tpl = e.data as AchievementTemplate;
		Debug.Log (tpl.id + ":  " + tpl.progress.ToString());
	}

	private void OnLeaderboardScoreLoaded(CEvent e) {
		ISN_PlayerScoreLoadedResult result = e.data as ISN_PlayerScoreLoadedResult;
		
		if(result.IsSucceeded) {
			GCScore score = result.loadedScore;
			IOSNativePopUpManager.showMessage("Leaderboard " + score.leaderboardId, "Score: " + score.score + "\n" + "Rank:" + score.rank);
		}
		
	}


	private void OnAuth(CEvent e) {
		ISN_Result r = e.data as ISN_Result;
		if (r.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authenticated", "ID: " + GameCenterManager.Player.PlayerId + "\n" + "Name: " + GameCenterManager.Player.DisplayName);
		}

	}
	
}
