//#define SA_DEBUG_MODE
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class GameCenterManager : MonoBehaviour {



	//Events

	public const string GAME_CENTER_ACHIEVEMENTS_RESET        	 				= "game_center_achievements_reset";
	public const string GAME_CENTER_ACHIEVEMENTS_LOADED        					= "game_center_achievements_loaded";
	public const string GAME_CENTER_ACHIEVEMENT_PROGRESS  						= "game_center_achievement_progress";
	public const string GAME_CENTER_PLAYER_AUTHENTICATED       					= "game_center_player_authenticated";



	public const string SCORE_SUMBITTED       									= "score_sumbitted";
	public const string GAME_CENTER_LEADERBOARD_SCORE_LOADED  					= "game_center_leaderboard_score_loaded";
	public const string GAME_CENTER_LEADERBOARD_SCORE_LIST_LOADED  			    = "game_center_leaderboard_score_list_loaded";
	
	
	public const string GAME_CENTER_USER_INFO_LOADED  							= "game_center_user_info_loaded";
	public const string GAME_CENTER_VIEW_DISMISSED  							= "game_center_view_dismissed";
	public const string GAME_CENTER_FRIEND_LIST_LOADED  						= "game_center_friend_list_loaded";





	//Actions
	public static Action<ISN_Result> OnAuthFinished  = delegate{};

	public static Action<ISN_Result> OnScoreSubmitted = delegate{};
	public static Action<ISN_PlayerScoreLoadedResult> OnPlayerScoreLoaded = delegate{};
	public static Action<ISN_Result> OnScoresListLoaded = delegate{};

	public static Action<ISN_Result> OnAchievementsReset = delegate{};
	public static Action<ISN_Result> OnAchievementsLoaded  = delegate{};
	public static Action<ISN_AchievementProgressResult> OnAchievementsProgress  = delegate{};


	public static Action OnGameCenterViewDismissedAction = delegate{};
	public static Action<ISN_Result> OnFriendsListLoaded = delegate{};
	public static Action<ISN_UserInfoLoadResult> OnUserInfoLoaded  = delegate{};
	public static Action<ISN_PlayerSignatureResult> OnPlayerSignatureRetrieveResult = delegate{};




	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _initGameCenter();
	
	[DllImport ("__Internal")]
	private static extern void _showLeaderboard(string leaderboardId, int timeSpan);

	[DllImport ("__Internal")]
	private static extern void _reportScore (string score, string leaderboardId);



	[DllImport ("__Internal")]
	private static extern void _showLeaderboards ();


	[DllImport ("__Internal")]
	private static extern void _getLeaderboardScore (string leaderboardId, int timeSpan, int collection);

	[DllImport ("__Internal")]
	private static extern void _loadLeaderboardScore (string leaderboardId, int timeSpan, int collection, int from, int to);
	
	[DllImport ("__Internal")]
	private static extern void _showAchievements();

	[DllImport ("__Internal")]
	private static extern void _resetAchievements();
	

	[DllImport ("__Internal")]
	private static extern void _submitAchievement(float percent, string achievementId, bool isCompleteNotification);

	[DllImport ("__Internal")]
	private static extern void _loadGCUserData(string uid);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallenge(string leaderboardId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallengeWithFriendsPicker(string leaderboardId, string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallenge(string leaderboardId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallengeWithFriendsPicker(string leaderboardId, string message);

	[DllImport ("__Internal")]
	private static extern void _gcRetrieveFriends();


	[DllImport ("__Internal")]
	private static extern void _ISN_getSignature();
	
	#endif


	private  static bool _IsInitialized = false;
	private  static bool _IsPlayerAuthenticated = false;
	private  static bool _IsAchievementsInfoLoaded = false;



	private static List<AchievementTemplate> _achievements = new List<AchievementTemplate> ();
	private static EventDispatcherBase _dispatcher  = new EventDispatcherBase ();

	private static Dictionary<string, GCLeaderboard> _leaderboards =  new Dictionary<string, GCLeaderboard>();
	private static Dictionary<string, GameCenterPlayerTemplate> _players =  new Dictionary<string, GameCenterPlayerTemplate>();
	private static List<string> _friendsList = new List<string>();


	private static GameCenterPlayerTemplate _player = null;


	private const string ISN_GC_PP_KEY = "ISN_GameCenterManager";

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static void init() {

		if(_IsInitialized) {
			return;
		}

		_IsInitialized = true;


		GameObject go =  new GameObject("GameCenterManager");
		go.AddComponent<GameCenterManager>();
		DontDestroyOnLoad(go);


		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_initGameCenter();
		#endif
			
	}

	void Awake() {
		foreach(string aId in IOSNativeSettings.Instance.RegisteredAchievementsIds) {
			RegisterAchievement(aId);
		}
	}


	public static void RetrievePlayerSignature() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_getSignature();
		#endif
	}



	public static void RegisterAchievement(string achievementId) {


		bool isContains = false;

		foreach(AchievementTemplate t in _achievements) {
			if (t.id.Equals (achievementId)) {
				isContains = true;
			}
		}


		if(!isContains) {
			AchievementTemplate tpl = new AchievementTemplate ();
			tpl.id = achievementId;
			tpl.progress = 0;
			_achievements.Add (tpl);
		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	

	public static void ShowLeaderboard(string leaderboardId) {
		ShowLeaderboard(leaderboardId, GCBoardTimeSpan.ALL_TIME);
	}


	public static void ShowLeaderboard(string leaderboardId, GCBoardTimeSpan timeSpan) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showLeaderboard(leaderboardId, (int) timeSpan);
		#endif
	}

	public static void ShowLeaderboards() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showLeaderboards ();
		#endif
	}
	

	public static void ReportScore(long score, string leaderboardId) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("unity reportScore: " + leaderboardId);

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_reportScore(score.ToString(), leaderboardId);
		#endif
	}


	public static void ReportScore(double score, string leaderboardId) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("unity reportScore double: " + leaderboardId);
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		long s = System.Convert.ToInt64(score * 100);
		_reportScore(s.ToString(), leaderboardId);
		#endif
	}



	public static void RetrieveFriends() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_gcRetrieveFriends();
		#endif
	}
	

	public static void LoadCurrentPlayerScore(string leaderboardId, GCBoardTimeSpan timeSpan = GCBoardTimeSpan.ALL_TIME, GCCollectionType collection = GCCollectionType.GLOBAL)  {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_getLeaderboardScore(leaderboardId, (int) timeSpan, (int) collection);
		#endif
	}
	

	private IEnumerator LoadCurrentPlayerScoreLocal(string leaderboardId, GCBoardTimeSpan timeSpan = GCBoardTimeSpan.ALL_TIME, GCCollectionType collection = GCCollectionType.GLOBAL ) {
		yield return new WaitForSeconds(4f);
		LoadCurrentPlayerScoreLocal(leaderboardId, timeSpan, collection);
	}



	
	public static void LoadScore(string leaderboardId, int from, int to, GCBoardTimeSpan timeSpan = GCBoardTimeSpan.ALL_TIME, GCCollectionType collection = GCCollectionType.GLOBAL) {

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_loadLeaderboardScore(leaderboardId, (int) timeSpan, (int) collection, from, to);
		#endif

	}


	public static void IssueLeaderboardChallenge(string leaderboardId, string message, string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallenge(leaderboardId, message, playerId);
		#endif
	}

	public static void IssueLeaderboardChallenge(string leaderboardId, string message, string[] playerIds) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			string ids = "";
			int len = playerIds.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += playerIds[i];
			}

		_ISN_issueLeaderboardChallenge(leaderboardId, message, ids);
		#endif
	}


	public static void IssueLeaderboardChallenge(string leaderboardId, string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallengeWithFriendsPicker(leaderboardId, message);
		#endif
	}




	public static void IssueAchievementChallenge(string achievementId, string message, string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueAchievementChallenge(achievementId, message, playerId);
		#endif
	}

	public static void IssueAchievementChallenge(string achievementId, string message, string[] playerIds) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			string ids = "";
			int len = playerIds.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += playerIds[i];
			}
			
		_ISN_issueAchievementChallenge(achievementId, message, ids);
		#endif
	}



	public static void IssueAchievementChallenge(string achievementId, string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueAchievementChallengeWithFriendsPicker(achievementId, message);
		#endif
	}


	public static void ShowAchievements() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showAchievements();
		#endif
	}

	public static void ResetAchievements() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_resetAchievements();

			foreach(AchievementTemplate tpl in _achievements) {
				tpl.progress = 0f;
			}
		#endif

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			ResetStoredProgress();
		}
	}


	public static void SubmitAchievement(float percent, string achievementId) {
		SubmitAchievement (percent, achievementId, true);
	}

	public static void SubmitAchievementNoCache(float percent, string achievementId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_submitAchievement(percent, achievementId, false);
		#endif
	}

	public static void SubmitAchievement(float percent, string achievementId, bool isCompleteNotification) {

		if(Application.internetReachability == NetworkReachability.NotReachable) {
			ISN_CacheManager.SaveAchievementRequest(achievementId, percent);
		}


		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			SaveAchievementProgress(achievementId, percent);
		}


		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_submitAchievement(percent, achievementId, isCompleteNotification);
		#endif
	}

	public static void LoadUsersData(string[] UIDs) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			//_loadGCUserData(UID);
		#endif
	}


	public static float GetAchievementProgress(string id) {
		float progress = 0f;

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			progress = GetStoredAchievementProgress(id);
		} else {
			foreach(AchievementTemplate tpl in _achievements) {
				if(tpl.id == id) {
					return tpl.progress;
				}
			}
		}

		return progress;
	}

	public static GCLeaderboard GetLeaderboard(string id) {
		if(_leaderboards.ContainsKey(id)) {
			return _leaderboards[id];
		} else {
			return null;
		}
	}


	public static GameCenterPlayerTemplate GetPlayerById(string playerID) {
		if(_players.ContainsKey(playerID)) {
			return _players[playerID];
		} else {
			return null;
		}
	}
	

	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public static List<AchievementTemplate> Achievements {
		get {
			return _achievements;
		}
	}


	public static Dictionary<string, GameCenterPlayerTemplate> Players {
		get {
			return _players;
		}
	}

	public static EventDispatcherBase Dispatcher {
		get {
			return _dispatcher;
		}
	}

	public static GameCenterPlayerTemplate Player {
		get {
			return _player;
		}
	}


	public static bool IsInitialized {
		get {
			return _IsInitialized;
		}
	}


	public static bool IsPlayerAuthenticated {
		get {
			return _IsPlayerAuthenticated;
		}
	}

	public static bool IsAchievementsInfoLoaded {
		get {
			return _IsAchievementsInfoLoaded;
		}
	}

	public static List<string> FriendsList {
		get {
			return _friendsList;
		}
	}



	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void onLeaderboardScoreFailed(string array) {
		ISN_PlayerScoreLoadedResult result = new ISN_PlayerScoreLoadedResult ();
		OnPlayerScoreLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_LEADERBOARD_SCORE_LOADED, result);
	}


	private void onLeaderboardScore(string array) {

		string[] data;
		data = array.Split("," [0]);

		string lbId = data[0];
		string scoreVal = data[1];
		int rank = System.Convert.ToInt32(data[2]);


		GCBoardTimeSpan timeSpan = (GCBoardTimeSpan) System.Convert.ToInt32(data[3]);
		GCCollectionType collection = (GCCollectionType) System.Convert.ToInt32(data[4]);

		GCLeaderboard board;
		if(_leaderboards.ContainsKey(lbId)) {
			board = _leaderboards[lbId];
		} else {
			board =  new GCLeaderboard(lbId);
			_leaderboards.Add(lbId, board);
		}


		GCScore score =  new GCScore(scoreVal, rank, timeSpan, collection, lbId, Player.PlayerId);

		board.UpdateScore(score);
		board.UpdateCurrentPlayerRank(rank, timeSpan, collection);
	

		ISN_PlayerScoreLoadedResult result = new ISN_PlayerScoreLoadedResult (score);
		OnPlayerScoreLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_LEADERBOARD_SCORE_LOADED, result);
	}

	
	public void onScoreSubmittedEvent(string array) {
		
		string[] data;
		data = array.Split("," [0]);
		
		string lbId = data[0];
		//string score =  data[1];


		StartCoroutine(LoadCurrentPlayerScoreLocal(lbId));

		
		ISN_Result result = new ISN_Result (true);
		OnScoreSubmitted (result);
		Dispatcher.dispatch (SCORE_SUMBITTED, result);
	}




	
	public void onScoreSubmittedFailed(string data) {
		ISN_Result result = new ISN_Result (false);
		OnScoreSubmitted (result);
		Dispatcher.dispatch (SCORE_SUMBITTED, result);
		
		
	}


	private void onLeaderboardScoreListLoaded(string array) {


		
		string[] data;
		data = array.Split("," [0]);

		string lbId = data[0];
		GCBoardTimeSpan timeSpan = (GCBoardTimeSpan) System.Convert.ToInt32(data[1]);
		GCCollectionType collection = (GCCollectionType) System.Convert.ToInt32(data[2]);

		GCLeaderboard board;
		if(_leaderboards.ContainsKey(lbId)) {
			board = _leaderboards[lbId];
		} else {
			board =  new GCLeaderboard(lbId);
			_leaderboards.Add(lbId, board);
		}


	
		
		
		for(int i = 3; i < data.Length; i+=3) {
			string playerId = data[i];
			string scoreVal = data[i + 1];
			int rank = System.Convert.ToInt32(data[i + 2]);

			GCScore score =  new GCScore(scoreVal, rank, timeSpan, collection, lbId, playerId);
			board.UpdateScore(score);
			if(Player != null) {
				if(Player.PlayerId.Equals(playerId)) {
					board.UpdateCurrentPlayerRank(rank, timeSpan, collection);
				}
			}
		}
		


		ISN_Result result = new ISN_Result (true);
		OnScoresListLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_LEADERBOARD_SCORE_LIST_LOADED, result);


	}

	private void onLeaderboardScoreListLoadFailed(string array) {

		ISN_Result result = new ISN_Result (false);
		OnScoresListLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_LEADERBOARD_SCORE_LIST_LOADED, result);
	}



	private void onAchievementsReset(string array) {

		ISN_Result result = new ISN_Result (true);
		OnAchievementsReset (result);
		Dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_RESET, result);

	}

	private void onAchievementsResetFailed(string array) {
		ISN_Result result = new ISN_Result (false);
		OnAchievementsReset (result);
		Dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_RESET, result);
	}


	private void onAchievementProgressChanged(string array) {
		string[] data;
		data = array.Split("," [0]);



		AchievementTemplate tpl =  new AchievementTemplate();
		tpl.id = data [0];
		tpl.progress = System.Convert.ToSingle(data [1]) ;
		ISN_AchievementProgressResult result = new ISN_AchievementProgressResult(tpl);

		submitAchievement (tpl);

		OnAchievementsProgress(result);
		Dispatcher.dispatch (GAME_CENTER_ACHIEVEMENT_PROGRESS, result);

	}


	private void onAchievementsLoaded(string array) {

		ISN_Result result = new ISN_Result (true);
		if(array.Equals(string.Empty)) {
			OnAchievementsLoaded (result);
			Dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_LOADED, result);
			return;
		}

		string[] data;
		data = array.Split("," [0]);


		for(int i = 0; i < data.Length; i+=2) {
			AchievementTemplate tpl =  new AchievementTemplate();
			tpl.id 				= data[i];
			tpl.progress 		= System.Convert.ToSingle(data[i + 1]);
			submitAchievement (tpl);
		}

		_IsAchievementsInfoLoaded = true;
		OnAchievementsLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_LOADED, result);
	}


	private void onAchievementsLoadedFailed() {
		ISN_Result result = new ISN_Result (false);
		OnAchievementsLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_LOADED, result);
	}


	private void onAuthenticateLocalPlayer(string  array) {
		string[] data;
		data = array.Split("," [0]);

		_player = new GameCenterPlayerTemplate (data[0], data [1], data [2]);


		ISN_CacheManager.SendAchievementCachedRequest();

		_IsPlayerAuthenticated = true;


		ISN_Result result = new ISN_Result (_IsPlayerAuthenticated);
		OnAuthFinished (result);
		Dispatcher.dispatch (GAME_CENTER_PLAYER_AUTHENTICATED, result);



	}
	
	
	private void onAuthenticationFailed(string  array) {
		_IsPlayerAuthenticated = false;


		ISN_Result result = new ISN_Result (_IsPlayerAuthenticated);
		OnAuthFinished (result);
		Dispatcher.dispatch (GAME_CENTER_PLAYER_AUTHENTICATED, result);
	}


	private void onUserInfoLoaded(string array) {

		string[] data;
		data = array.Split("," [0]);

		string playerId = data[0];
		string displayName = data[3];
		string alias = data[2];
		string avatar = data[1];

		GameCenterPlayerTemplate p =  new GameCenterPlayerTemplate(playerId, displayName, alias);
		p.SetAvatar(avatar);


		if(_players.ContainsKey(playerId)) {
			_players[playerId] = p;
		} else {
			_players.Add(playerId, p);
		}

		if(p.PlayerId == _player.PlayerId) {
			_player = p;
		}




		ISN_UserInfoLoadResult result = new ISN_UserInfoLoadResult (p);
		OnUserInfoLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_USER_INFO_LOADED, result);
		

	}    
	
	private void onUserInfoLoadFailed(string playerId) {
		
		ISN_UserInfoLoadResult result = new ISN_UserInfoLoadResult (playerId);
		OnUserInfoLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_USER_INFO_LOADED, result);
	}

	private void OnGameCenterViewDismissed(string data) {
		Dispatcher.dispatch(GAME_CENTER_VIEW_DISMISSED);
		OnGameCenterViewDismissedAction();
	}

	private void onFriendListLoaded(string data) {


		string[] fl;
		fl = data.Split("|" [0]);

		for(int i = 0; i < fl.Length; i++) {
			_friendsList.Add(fl[i]);
		}

		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("Friends list loaded, total friends: " + _friendsList.Count);


		ISN_Result result = new ISN_Result (true);
		OnFriendsListLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_FRIEND_LIST_LOADED, result);

	}

	private void onFriendListFailedToLoad(string data) {
		ISN_Result result = new ISN_Result (false);
		OnFriendsListLoaded (result);
		Dispatcher.dispatch (GAME_CENTER_FRIEND_LIST_LOADED, result);
	}



	
	private void VerificationSignatureRetrieveFailed(string array) {

		string[] data;
		data = array.Split("|" [0]);

		ISN_Error error =  new ISN_Error();
		error.code = System.Convert.ToInt32(data[0]);
		error.description = data[1];

		ISN_PlayerSignatureResult res =  new ISN_PlayerSignatureResult(error);
		OnPlayerSignatureRetrieveResult(res);

	}

	private void VerificationSignatureRetrieved(string array) {
		string[] data;
		data = array.Split("|" [0]);

		ISN_PlayerSignatureResult res =  new ISN_PlayerSignatureResult(data[0], data[1], data[2], data[3]);
		OnPlayerSignatureRetrieveResult(res);
	}



	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void submitAchievement(AchievementTemplate tpl) {
		bool isContains = false;
		foreach(AchievementTemplate t in _achievements) {
			if (t.id.Equals (tpl.id)) {
				isContains = true;
				t.progress = tpl.progress;
			}
		}

		if(IOSNativeSettings.Instance.UsePPForAchievements) {
			SaveAchievementProgress(tpl.id, tpl.progress);
		}

		if(!isContains) {
			_achievements.Add (tpl);
		}
	}


	private static void ResetStoredProgress() {
		foreach(AchievementTemplate t in _achievements) {
			PlayerPrefs.DeleteKey(ISN_GC_PP_KEY + t.id);
		}
	}

	private static void SaveAchievementProgress(string achievementId, float progress) {

		float currentProgress =  GetStoredAchievementProgress(achievementId);
		if(progress > currentProgress) {
			PlayerPrefs.SetFloat(ISN_GC_PP_KEY + achievementId, progress);
		}
	}

	private static float GetStoredAchievementProgress(string achievementId) {
		float v = 0f;
		if(PlayerPrefs.HasKey(ISN_GC_PP_KEY + achievementId)) {
			v = PlayerPrefs.GetFloat(ISN_GC_PP_KEY + achievementId);
		} 

		return v;
	}
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
