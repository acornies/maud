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

	
	private string leaderBoardId =  "your.ios.leaderbord1.id.here";
	private string leaderBoardId2 = "your.leaderbord2.id.here";


	private string TEST_ACHIEVEMENT_1_ID = "your.achievement.id1.here";
	private string TEST_ACHIEVEMENT_2_ID = "your.achievement.id2.here";

	private static bool IsInited = false;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	


	void Awake() {
		if(!IsInited) {
			
			//Achievement registration. If you will skipt this step GameCenterManager.achievements array will contain only achievements with reported progress 
			GameCenterManager.registerAchievement (TEST_ACHIEVEMENT_1_ID);
			GameCenterManager.registerAchievement (TEST_ACHIEVEMENT_2_ID);
			
			
			//Listen for the Game Center events
			GameCenterManager.dispatcher.addEventListener (GameCenterManager.GAME_CENTER_ACHIEVEMENT_PROGRESS, OnAchievementProgress);
			GameCenterManager.dispatcher.addEventListener (GameCenterManager.GAME_CENTER_ACHIEVEMENTS_RESET, OnAchievementsReset);
			
			
			GameCenterManager.dispatcher.addEventListener (GameCenterManager.GAME_CENTER_LEADER_BOARD_SCORE_LOADED, OnLeaderBoarScoreLoaded);



			//actions use example
			GameCenterManager.OnPlayerScoreLoaded += OnPlayerScoreLoaded;
			GameCenterManager.OnAuthFinished += OnAuthFinished;

			GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;

			
			//Initializing Game Cneter class. This action will triger authentication flow
			GameCenterManager.init();
			IsInited = true;
		}


	
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	void OnGUI() {


		UpdateToStartPos();

		if(GameCenterManager.player != null) {
			GUI.Label(new Rect(100, 10, Screen.width, 40), "ID: " + GameCenterManager.player.playerId);
			GUI.Label(new Rect(100, 20, Screen.width, 40), "Name: " +  GameCenterManager.player.displayName);
			GUI.Label(new Rect(100, 30, Screen.width, 40), "Alias: " +  GameCenterManager.player.alias);

		
			//avatar loading will take a while after the user is connectd
			//so we will draw it only after instantiation to avoid null pointer check
			//if you whant to know exaxtly when the avatar is loaded, you can subscribe on 
			//GameCenterManager.OnUserInfoLoaded action  			
			//and checl for a spesific user ID
			if(GameCenterManager.player.avatar != null) {
				GUI.DrawTexture(new Rect(10, 10, 75, 75), GameCenterManager.player.avatar);
			}
		}

		StartY+= YLableStep;
		StartY+= YLableStep;
		StartY+= YLableStep;


		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "GameCneter Leaderboards", style);


		StartY+= YLableStep;
		
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leaderboards")) {
			GameCenterManager.showLeaderBoards ();
		}


		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leader Board1")) {
			GameCenterManager.showLeaderBoard(leaderBoardId);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Report Score LB 1")) {
			hiScore++;
			GameCenterManager.reportScore(9223372036854775000, leaderBoardId);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Score LB 1")) {
			GameCenterManager.loadCurrentPlayerScore(leaderBoardId);
		}


		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leader Board2")) {
			GameCenterManager.showLeaderBoard(leaderBoardId2);
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Leader Board2 Today")) {
			GameCenterManager.showLeaderBoard(leaderBoardId2, GCBoardTimeSpan.TODAY);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Report Score LB2")) {
			hiScore++;

			GameCenterManager.OnScoreSubmited += OnScoreSubmited;
			GameCenterManager.reportScore(hiScore, leaderBoardId2);
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Score LB 2")) {
			GameCenterManager.loadCurrentPlayerScore(leaderBoardId2);
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Send Challenge")) {
			GameCenterManager.issueLeaderboardChallenge(leaderBoardId2, "Here is tiny challenge for you");
		}



		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "GameCneter Achievements", style);

		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Show Achievements")) {
			GameCenterManager.showAchievements();
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Reset Achievements")) {
			GameCenterManager.resetAchievements();
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Submit Achievements1")) {
			GameCenterManager.submitAchievement(GameCenterManager.getAchievementProgress(TEST_ACHIEVEMENT_1_ID) + 2.432f, TEST_ACHIEVEMENT_1_ID);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Submit Achievements2")) {
			GameCenterManager.submitAchievement(88.66f, TEST_ACHIEVEMENT_2_ID, false);
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Send Challenge")) {
			GameCenterManager.issueAchievementChallenge(TEST_ACHIEVEMENT_1_ID, "Here is tiny challenge for you");
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
			Debug.Log ("Achievemnts was loaded from IOS Game Center");
			
			foreach(AchievementTemplate tpl in GameCenterManager.achievements) {
				Debug.Log (tpl.id + ":  " + tpl.progress);
			}
		}

	}

	private void OnAchievementsReset() {
		Debug.Log ("All  Achievemnts was reseted");
	}

	private void OnAchievementProgress(CEvent e) {
		Debug.Log ("OnAchievementProgress");

		ISN_AcheivmentProgressResult result = e.data as ISN_AcheivmentProgressResult;

		if(result.IsSucceeded) {
			AchievementTemplate tpl = result.info;
			Debug.Log (tpl.id + ":  " + tpl.progress.ToString());
		}


	}

	private void OnLeaderBoarScoreLoaded(CEvent e) {
		ISN_PlayerScoreLoadedResult result = e.data as ISN_PlayerScoreLoadedResult;

		if(result.IsSucceeded) {
			GCScore score = result.loadedScore;
			IOSNativePopUpManager.showMessage("Leader Board " + score.leaderboardId, "Score: " + score.score + "\n" + "Rank:" + score.rank);
		}

	}

	private void OnPlayerScoreLoaded (ISN_PlayerScoreLoadedResult result) {
		if(result.IsSucceeded) {
			GCScore score = result.loadedScore;
			IOSNativePopUpManager.showMessage("Leader Board " + score.leaderboardId, "Score: " + score.score + "\n" + "Rank:" + score.rank);

			Debug.Log("double score representation: " + score.GetDoubleScore());
			Debug.Log("long score representation: " + score.GetLongScore());
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

	void OnAuthFinished (ISN_Result res) {
		if (res.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Player Authed ", "ID: " + GameCenterManager.player.playerId + "\n" + "Alias: " + GameCenterManager.player.alias);
		} else {
			IOSNativePopUpManager.showMessage("Game Cneter ", "Player auntification failed");
		}
	}
	

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
