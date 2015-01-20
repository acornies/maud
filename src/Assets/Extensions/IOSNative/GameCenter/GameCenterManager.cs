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



	public const string SCORE_SUMBITED       									= "score_sumbited";
	public const string GAME_CENTER_LEADER_BOARD_SCORE_LOADED  					= "game_center_leader_board_score_loaded";
	public const string GAME_CENTER_LEADER_BOARD_SCORE_LIST_LOADED  			= "game_center_leader_board_score_list_loaded";
	
	
	public const string GAME_CENTER_USER_INFO_LOADED  							= "game_center_user_info_loaded";
	public const string GAME_CENTER_VIEW_DISSMISSED  							= "game_center_view_dissmissed";
	public const string GAME_CENTER_FRIEND_LIST_LOADED  						= "game_center_friend_list_loaded";





	//Actions
	public static Action<ISN_Result> OnAuthFinished  = delegate{};

	public static Action<ISN_Result> OnScoreSubmited = delegate{};
	public static Action<ISN_PlayerScoreLoadedResult> OnPlayerScoreLoaded = delegate{};
	public static Action<ISN_Result> OnScoresListLoaded = delegate{};

	public static Action<ISN_Result> OnAchievementsReset = delegate{};
	public static Action<ISN_Result> OnAchievementsLoaded  = delegate{};
	public static Action<ISN_AcheivmentProgressResult> OnAchievementsProgress  = delegate{};


	public static Action OnGameCenterViewDissmissed  = delegate{};
	public static Action<ISN_Result> OnFriendsListLoaded = delegate{};
	public static Action<ISN_UserInfoLoadResult> OnUserInfoLoaded  = delegate{};




	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _initGamaCenter();
	
	[DllImport ("__Internal")]
	private static extern void _showLeaderBoard(string leaderBoradrId, int timeSpan);

	[DllImport ("__Internal")]
	private static extern void _reportScore (string score, string leaderBoradrId);



	[DllImport ("__Internal")]
	private static extern void _showLeaderBoards ();


	[DllImport ("__Internal")]
	private static extern void _getLeadrBoardScore (string leaderBoradrId, int timeSpan, int collection);

	[DllImport ("__Internal")]
	private static extern void _loadLeadrBoardScore (string leaderBoradrId, int timeSpan, int collection, int from, int to);
	
	[DllImport ("__Internal")]
	private static extern void _showAchievements();

	[DllImport ("__Internal")]
	private static extern void _resetAchievements();
	

	[DllImport ("__Internal")]
	private static extern void _submitAchievement(float percent, string achievementId, bool isCompleteNotification);

	[DllImport ("__Internal")]
	private static extern void _loadGCUserData(string uid);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallenge(string leaderBoradrId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueLeaderboardChallengeWithFriendsPicker(string leaderBoradrId, string message);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallenge(string leaderBoradrId, string message, string playerIds);

	[DllImport ("__Internal")]
	private static extern void _ISN_issueAchievementChallengeWithFriendsPicker(string leaderBoradrId, string message);

	[DllImport ("__Internal")]
	private static extern void _gcRetrieveFriends();
	
	#endif


	private  static bool _IsInited = false;
	private  static bool _IsPlayerAuthed = false;
	private  static bool _IsAchievementsInfoLoaded = false;



	private static List<AchievementTemplate> _achievements = new List<AchievementTemplate> ();
	private static EventDispatcherBase _dispatcher  = new EventDispatcherBase ();

	private static Dictionary<string, GCLeaderBoard> _leaderboards =  new Dictionary<string, GCLeaderBoard>();
	private static Dictionary<string, GameCenterPlayerTemplate> _players =  new Dictionary<string, GameCenterPlayerTemplate>();
	private static List<string> _friendsList = new List<string>();


	private static GameCenterPlayerTemplate _player = null;


	private const string ISN_GC_PP_KEY = "ISN_GameCenterManager";

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static void init() {

		if(_IsInited) {
			return;
		}

		_IsInited = true;


		GameObject go =  new GameObject("GameCenterManager");
		go.AddComponent<GameCenterManager>();
		DontDestroyOnLoad(go);


		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_initGamaCenter();
		#endif
			
	}

	void Awake() {
		foreach(string aId in IOSNativeSettings.Instance.RegistredAchievementsIds) {
			registerAchievement(aId);
		}
	}
	



	public static void registerAchievement(string achievemenId) {


		bool isContains = false;

		foreach(AchievementTemplate t in _achievements) {
			if (t.id.Equals (achievemenId)) {
				isContains = true;
			}
		}


		if(!isContains) {
			AchievementTemplate tpl = new AchievementTemplate ();
			tpl.id = achievemenId;
			tpl.progress = 0;
			_achievements.Add (tpl);
		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	

	public static void showLeaderBoard(string leaderBoradrId) {
		showLeaderBoard(leaderBoradrId, GCBoardTimeSpan.ALL_TIME);
	}


	public static void showLeaderBoard(string leaderBoradrId, GCBoardTimeSpan timeSpan) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showLeaderBoard(leaderBoradrId, (int) timeSpan);
		#endif
	}

	public static void showLeaderBoards() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showLeaderBoards ();
		#endif
	}
	

	public static void reportScore(long score, string leaderBoradrId) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("unity reportScore: " + leaderBoradrId);

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_reportScore(score.ToString(), leaderBoradrId);
		#endif
	}


	public static void reportScore(double score, string leaderBoradrId) {
		if(!IOSNativeSettings.Instance.DisablePluginLogs) 
			Debug.Log("unity reportScore double: " + leaderBoradrId);
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		long s = System.Convert.ToInt64(score * 100);
		_reportScore(s.ToString(), leaderBoradrId);
		#endif
	}



	public static void RetrieveFriends() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_gcRetrieveFriends();
		#endif
	}
	

	public static void loadCurrentPlayerScore(string leaderBoradrId, GCBoardTimeSpan timeSpan = GCBoardTimeSpan.ALL_TIME, GCCollectionType collection = GCCollectionType.GLOBAL)  {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_getLeadrBoardScore(leaderBoradrId, (int) timeSpan, (int) collection);
		#endif
	}
	

	private IEnumerator loadCurrentPlayerScoreLocal(string leaderBoradrId, GCBoardTimeSpan timeSpan = GCBoardTimeSpan.ALL_TIME, GCCollectionType collection = GCCollectionType.GLOBAL ) {
		yield return new WaitForSeconds(2f);
		loadCurrentPlayerScore(leaderBoradrId, timeSpan, collection);
	}


	[System.Obsolete("getScore is deprecated, use loadCurrentPlayerScore instead")]
	public static void getScore(string leaderBoradrId, GCBoardTimeSpan timeSpan = GCBoardTimeSpan.ALL_TIME, GCCollectionType collection = GCCollectionType.GLOBAL) {
		loadCurrentPlayerScore(leaderBoradrId, timeSpan, collection);
	}

	
	public static void loadScore(string leaderBoradrId, int from, int to, GCBoardTimeSpan timeSpan = GCBoardTimeSpan.ALL_TIME, GCCollectionType collection = GCCollectionType.GLOBAL) {

		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_loadLeadrBoardScore(leaderBoradrId, (int) timeSpan, (int) collection, from, to);
		#endif

	}


	public static void issueLeaderboardChallenge(string leaderBoradrId, string message, string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallenge(leaderBoradrId, message, playerId);
		#endif
	}

	public static void issueLeaderboardChallenge(string leaderBoradrId, string message, string[] playerIds) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			string ids = "";
			int len = playerIds.Length;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += playerIds[i];
			}

		_ISN_issueLeaderboardChallenge(leaderBoradrId, message, ids);
		#endif
	}


	public static void issueLeaderboardChallenge(string leaderBoradrId, string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueLeaderboardChallengeWithFriendsPicker(leaderBoradrId, message);
		#endif
	}




	public static void issueAchievementChallenge(string achievementId, string message, string playerId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueAchievementChallenge(achievementId, message, playerId);
		#endif
	}

	public static void issueAchievementChallenge(string achievementId, string message, string[] playerIds) {
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



	public static void issueAchievementChallenge(string achievementId, string message) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_issueAchievementChallengeWithFriendsPicker(achievementId, message);
		#endif
	}


	public static void showAchievements() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showAchievements();
		#endif
	}

	public static void resetAchievements() {
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


	public static void submitAchievement(float percent, string achievementId) {
		submitAchievement (percent, achievementId, true);
	}

	public static void submitAchievementNoChache(float percent, string achievementId) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_submitAchievement(percent, achievementId, false);
		#endif
	}

	public static void submitAchievement(float percent, string achievementId, bool isCompleteNotification) {

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

	public static void loadUsersData(string[] UIDs) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			//_loadGCUserData(UID);
		#endif
	}


	public static float getAchievementProgress(string id) {
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

	public static GCLeaderBoard GetLeaderBoard(string id) {
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

	public static List<AchievementTemplate> achievements {
		get {
			return _achievements;
		}
	}


	public static Dictionary<string, GameCenterPlayerTemplate> players {
		get {
			return _players;
		}
	}

	public static EventDispatcherBase dispatcher {
		get {
			return _dispatcher;
		}
	}

	public static GameCenterPlayerTemplate player {
		get {
			return _player;
		}
	}


	public static bool IsInited {
		get {
			return _IsInited;
		}
	}


	public static bool IsPlayerAuthed {
		get {
			return _IsPlayerAuthed;
		}
	}

	public static bool IsAchievementsInfoLoaded {
		get {
			return _IsAchievementsInfoLoaded;
		}
	}

	public static List<string> friendsList {
		get {
			return _friendsList;
		}
	}



	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void onLeaderBoardScoreFailed(string array) {
		ISN_PlayerScoreLoadedResult result = new ISN_PlayerScoreLoadedResult ();
		OnPlayerScoreLoaded (result);
		dispatcher.dispatch (GAME_CENTER_LEADER_BOARD_SCORE_LOADED, result);
	}


	private void onLeaderBoardScore(string array) {

		string[] data;
		data = array.Split("," [0]);

		string lbId = data[0];
		string scoreVal = data[1];
		int rank = System.Convert.ToInt32(data[2]);


		GCBoardTimeSpan timeSpan = (GCBoardTimeSpan) System.Convert.ToInt32(data[3]);
		GCCollectionType collection = (GCCollectionType) System.Convert.ToInt32(data[4]);

		GCLeaderBoard board;
		if(_leaderboards.ContainsKey(lbId)) {
			board = _leaderboards[lbId];
		} else {
			board =  new GCLeaderBoard(lbId);
			_leaderboards.Add(lbId, board);
		}


		GCScore score =  new GCScore(scoreVal, rank, timeSpan, collection, lbId, player.playerId);

		board.UpdateScore(score);
		board.UpdateCurrentPlayerRank(rank, timeSpan, collection);
	

		ISN_PlayerScoreLoadedResult result = new ISN_PlayerScoreLoadedResult (score);
		OnPlayerScoreLoaded (result);
		dispatcher.dispatch (GAME_CENTER_LEADER_BOARD_SCORE_LOADED, result);
	}

	
	public void onScoreSubmitedEvent(string array) {
		
		string[] data;
		data = array.Split("," [0]);
		
		string lbId = data[0];
		//string score =  data[1];


		StartCoroutine(loadCurrentPlayerScoreLocal(lbId));

		
		ISN_Result result = new ISN_Result (true);
		OnScoreSubmited (result);
		dispatcher.dispatch (SCORE_SUMBITED, result);
	}




	
	public void onScoreSubmitedFailed(string data) {
		ISN_Result result = new ISN_Result (false);
		OnScoreSubmited (result);
		dispatcher.dispatch (SCORE_SUMBITED, result);
		
		
	}


	private void onLeaderBoardScoreListLoaded(string array) {


		
		string[] data;
		data = array.Split("," [0]);

		string lbId = data[0];
		GCBoardTimeSpan timeSpan = (GCBoardTimeSpan) System.Convert.ToInt32(data[1]);
		GCCollectionType collection = (GCCollectionType) System.Convert.ToInt32(data[2]);

		GCLeaderBoard board;
		if(_leaderboards.ContainsKey(lbId)) {
			board = _leaderboards[lbId];
		} else {
			board =  new GCLeaderBoard(lbId);
			_leaderboards.Add(lbId, board);
		}


	
		
		
		for(int i = 3; i < data.Length; i+=3) {
			string playerId = data[i];
			string scoreVal = data[i + 1];
			int rank = System.Convert.ToInt32(data[i + 2]);

			GCScore score =  new GCScore(scoreVal, rank, timeSpan, collection, lbId, playerId);
			board.UpdateScore(score);
			if(player != null) {
				if(player.playerId.Equals(playerId)) {
					board.UpdateCurrentPlayerRank(rank, timeSpan, collection);
				}
			}
		}
		


		ISN_Result result = new ISN_Result (true);
		OnScoresListLoaded (result);
		dispatcher.dispatch (GAME_CENTER_LEADER_BOARD_SCORE_LIST_LOADED, result);


	}

	private void onLeaderBoardScoreListLoadFailed(string array) {

		ISN_Result result = new ISN_Result (false);
		OnScoresListLoaded (result);
		dispatcher.dispatch (GAME_CENTER_LEADER_BOARD_SCORE_LIST_LOADED, result);
	}



	private void onAchievementsReset(string array) {

		ISN_Result result = new ISN_Result (true);
		OnAchievementsReset (result);
		dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_RESET, result);

	}

	private void onAchievementsResetFailed(string array) {
		ISN_Result result = new ISN_Result (false);
		OnAchievementsReset (result);
		dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_RESET, result);
	}


	private void onAchievementProgressChanged(string array) {
		string[] data;
		data = array.Split("," [0]);



		AchievementTemplate tpl =  new AchievementTemplate();
		tpl.id = data [0];
		tpl.progress = System.Convert.ToSingle(data [1]) ;
		ISN_AcheivmentProgressResult result = new ISN_AcheivmentProgressResult(tpl);

		submitAchievement (tpl);

		OnAchievementsProgress(result);
		dispatcher.dispatch (GAME_CENTER_ACHIEVEMENT_PROGRESS, result);

	}


	private void onAchievementsLoaded(string array) {

		ISN_Result result = new ISN_Result (true);
		if(array.Equals(string.Empty)) {
			OnAchievementsLoaded (result);
			dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_LOADED, result);
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
		dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_LOADED, result);
	}


	private void onAchievementsLoadedFailed() {
		ISN_Result result = new ISN_Result (false);
		OnAchievementsLoaded (result);
		dispatcher.dispatch (GAME_CENTER_ACHIEVEMENTS_LOADED, result);
	}


	private void onAuthenticateLocalPlayer(string  array) {
		string[] data;
		data = array.Split("," [0]);

		_player = new GameCenterPlayerTemplate (data[0], data [1], data [2]);


		ISN_CacheManager.SendAchievementChashedRequests ();

		_IsPlayerAuthed = true;


		ISN_Result result = new ISN_Result (_IsPlayerAuthed);
		OnAuthFinished (result);
		dispatcher.dispatch (GAME_CENTER_PLAYER_AUTHENTICATED, result);



	}
	
	
	private void onAuthenticationFailed(string  array) {
		_IsPlayerAuthed = false;


		ISN_Result result = new ISN_Result (_IsPlayerAuthed);
		OnAuthFinished (result);
		dispatcher.dispatch (GAME_CENTER_PLAYER_AUTHENTICATED, result);
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

		if(p.playerId == _player.playerId) {
			_player = p;
		}




		ISN_UserInfoLoadResult result = new ISN_UserInfoLoadResult (p);
		OnUserInfoLoaded (result);
		dispatcher.dispatch (GAME_CENTER_USER_INFO_LOADED, result);
		

	}    
	
	private void onUserInfoLoadFailed(string playerId) {
		
		ISN_UserInfoLoadResult result = new ISN_UserInfoLoadResult (playerId);
		OnUserInfoLoaded (result);
		dispatcher.dispatch (GAME_CENTER_USER_INFO_LOADED, result);
	}

	private void OnGameCenterViewDismissed(string data) {
		dispatcher.dispatch(GAME_CENTER_VIEW_DISSMISSED);
		OnGameCenterViewDissmissed();
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
		dispatcher.dispatch (GAME_CENTER_FRIEND_LIST_LOADED, result);

	}

	private void onFriendListFailedToLoad(string data) {
		ISN_Result result = new ISN_Result (false);
		OnFriendsListLoaded (result);
		dispatcher.dispatch (GAME_CENTER_FRIEND_LIST_LOADED, result);
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
