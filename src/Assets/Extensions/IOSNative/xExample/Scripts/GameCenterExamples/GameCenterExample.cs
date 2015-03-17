////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;

public class GameCenterExample : BaseIOSFeaturePreview {
	
	private int hiScore = 200;
	
	
	private string leaderBoardId =  "your.ios.leaderbord1.id";
	private string leaderBoardId2 = "your.ios.leaderbord2.id";
	
	
	
	private string TEST_ACHIEVEMENT_1_ID = "your.achievement.id1.here";
	private string TEST_ACHIEVEMENT_2_ID = "your.achievement.id2.here";
	
	
	private static bool IsInitialized = false;
	private static long LB2BestScores = 0;
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	
	
	
	
	void Awake() {
		if(!IsInitialized) {
			
			//Achievement registration. If you skip this step GameCenterManager.achievements array will contain only achievements with reported progress 
			GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_1_ID);
			GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_2_ID);
			
			
			//Listen for the Game Center events
			GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_ACHIEVEMENT_PROGRESS, OnAchievementProgress);
			GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_ACHIEVEMENTS_RESET, OnAchievementsReset);
			
			
			GameCenterManager.Dispatcher.addEventListener (GameCenterManager.GAME_CENTER_LEADERBOARD_SCORE_LOADED, OnLeaderboardScoreLoaded);
			
			
			
			//actions use example
			GameCenterManager.OnPlayerScoreLoaded += OnPlayerScoreLoaded;
			GameCenterManager.OnAuthFinished += OnAuthFinished;
			
			GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;
			
			
			//Initializing Game Center class. This action will trigger authentication flow
			GameCenterManager.init();
			IsInitialized = true;
		}
		
		
		
	}
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	void OnGUI() {
		

		UpdateToStartPos();
		
		if(GameCenterManager.Player != null) {
			GUI.Label(new Rect(100, 10, Screen.width, 40), "ID: " + GameCenterManager.Player.PlayerId);
			GUI.Label(new Rect(100, 20, Screen.width, 40), "Name: " +  GameCenterManager.Player.DisplayName);
			GUI.Label(new Rect(100, 30, Screen.width, 40), "Alias: " +  GameCenterManager.Player.Alias);
			
			
			//avatar loading will take a while after the user is connectd
			//so we will draw it only after instantiation to avoid null pointer check
			//if you whant to know exaxtly when the avatar is loaded, you can subscribe on 
			//GameCenterManager.OnUserInfoLoaded action  			
			//and checl for a spesific user ID
			if(GameCenterManager.Player.Avatar != null) {
				GUI.DrawTexture(new Rect(10, 10, 75, 75), GameCenterManager.Player.Avatar);
			}
		}
		
		StartY+= YLableStep;
		StartY+= YLableStep;
		StartY+= YLableStep;
		
		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Game Center Leaderboards", style);
		

		StartY+= YLableStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leaderboards")) {
			GameCenterManager.ShowLeaderboards ();
		}
		
		
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leaderboard 1")) {
			GameCenterManager.ShowLeaderboard(leaderBoardId);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Report Score LB 1")) {
			hiScore++;
			GameCenterManager.ReportScore(hiScore, leaderBoardId);
			
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Score LB 1")) {
			GameCenterManager.LoadCurrentPlayerScore(leaderBoardId);
		}
		
		
		StartX = XStartPos;
		StartY+= YButtonStep;
		
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Leaderboard #2, user best score: " + LB2BestScores.ToString(), style);
		
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leader Board2")) {
			GameCenterManager.ShowLeaderboard(leaderBoardId2);

		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leaderboard 2 Today")) {
			GameCenterManager.ShowLeaderboard(leaderBoardId2, GCBoardTimeSpan.TODAY);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Report Score LB2")) {
			hiScore++;
			
			GameCenterManager.OnScoreSubmitted += OnScoreSubmitted;
			GameCenterManager.ReportScore(hiScore, leaderBoardId2);
		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Score LB 2")) {
			GameCenterManager.LoadCurrentPlayerScore(leaderBoardId2);
		}
		
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Send Challenge")) {
			GameCenterManager.IssueLeaderboardChallenge(leaderBoardId2, "Here's a tiny challenge for you");
		}
		
		
		
		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Game Center Achievements", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Achievements")) {
			GameCenterManager.ShowAchievements();
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Reset Achievements")) {
			GameCenterManager.ResetAchievements();
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Submit Achievements1")) {
			GameCenterManager.SubmitAchievement(GameCenterManager.GetAchievementProgress(TEST_ACHIEVEMENT_1_ID) + 2.432f, TEST_ACHIEVEMENT_1_ID);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Submit Achievements2")) {
			GameCenterManager.SubmitAchievement(88.66f, TEST_ACHIEVEMENT_2_ID, false);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Send Challenge")) {
			GameCenterManager.IssueAchievementChallenge(TEST_ACHIEVEMENT_1_ID, "Here's a tiny challenge for you");
		}
		
		
		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "More", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Retrieve Signature")) {
			GameCenterManager.RetrievePlayerSignature();
			GameCenterManager.OnPlayerSignatureRetrieveResult += OnPlayerSignatureRetrieveResult;
		}
		
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private void OnAchievementsLoaded(ISN_Result result) {
		
		Debug.Log("OnAchievementsLoaded");
		Debug.Log(result.IsSucceeded);
		
		if(result.IsSucceeded) {
			Debug.Log ("Achievements were loaded from iOS Game Center");
			
			foreach(AchievementTemplate tpl in GameCenterManager.Achievements) {
				Debug.Log (tpl.id + ":  " + tpl.progress);
			}
		}
		
	}
	
	private void OnAchievementsReset() {
		Debug.Log ("All Achievements were reset");
	}
	
	private void OnAchievementProgress(CEvent e) {
		Debug.Log ("OnAchievementProgress");
		
		ISN_AchievementProgressResult result = e.data as ISN_AchievementProgressResult;
		
		if(result.IsSucceeded) {
			AchievementTemplate tpl = result.info;
			Debug.Log (tpl.id + ":  " + tpl.progress.ToString());
		}
		
		
	}
	
	private void OnLeaderboardScoreLoaded(CEvent e) {
		ISN_PlayerScoreLoadedResult result = e.data as ISN_PlayerScoreLoadedResult;
		
		if(result.IsSucceeded) {
			GCScore score = result.loadedScore;
			IOSNativePopUpManager.showMessage("Leaderboard " + score.leaderboardId, "Score: " + score.score + "\n" + "Rank:" + score.rank);
		}
		
	}
	
	private void OnPlayerScoreLoaded (ISN_PlayerScoreLoadedResult result) {
		if(result.IsSucceeded) {
			GCScore score = result.loadedScore;
			IOSNativePopUpManager.showMessage("Leaderboard " + score.leaderboardId, "Score: " + score.score + "\n" + "Rank:" + score.rank);
			
			Debug.Log("double score representation: " + score.GetDoubleScore());
			Debug.Log("long score representation: " + score.GetLongScore());
			
			if(score.leaderboardId.Equals(leaderBoardId2)) {
				Debug.Log("Updating leaderboard 2 score");
				LB2BestScores = score.GetLongScore();
				
			}
		}
	}
	
	private void OnScoreSubmitted (ISN_Result result) {
		GameCenterManager.OnScoreSubmitted -= OnScoreSubmitted;
		
		if(result.IsSucceeded)  {
			Debug.Log("Score Submitted");
		} else {
			Debug.Log("Score Submit Failed");
		}
	}
	
	
	void OnAuthFinished (ISN_Result res) {
		if (res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authed ", "ID: " + GameCenterManager.Player.PlayerId + "\n" + "Alias: " + GameCenterManager.Player.Alias);
			GameCenterManager.LoadCurrentPlayerScore(leaderBoardId2);
		} else {
			IOSNativePopUpManager.showMessage("Game Center ", "Player authentication failed");
		}
	}
	
	
	void OnPlayerSignatureRetrieveResult (ISN_PlayerSignatureResult result) {
		Debug.Log("OnPlayerSignatureRetrieveResult");
		
		if(result.IsSucceeded) {
			
			Debug.Log("PublicKeyUrl: " + result.PublicKeyUrl);
			Debug.Log("Signature: " + result.Signature);
			Debug.Log("Salt: " + result.Salt);
			Debug.Log("Timestamp: " + result.Timestamp);
			
		} else {
			Debug.Log("Error code: " + result.error.code);
			Debug.Log("Error description: " + result.error.description);
		}
		
		GameCenterManager.OnPlayerSignatureRetrieveResult -= OnPlayerSignatureRetrieveResult;
	}
	
	
	
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------
	
	
}
