////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GCLeaderBoard  {


	public ScoreCollection SocsialCollection =  new ScoreCollection();
	public ScoreCollection GlobalCollection =  new ScoreCollection();

	public Dictionary<string, int> currentPlayerRank =  new Dictionary<string, int>();

	

	private string _id;

	public GCLeaderBoard(string leaderBoardId) {
		_id = leaderBoardId;
	}




	public string id {
		get {
			return _id;
		}
	}


	public GCScore GetCurrentPlayerScore(GCBoardTimeSpan timeSpan, GCCollectionType collection) {
		string key = timeSpan.ToString() + "_" + collection.ToString();
		if(currentPlayerRank.ContainsKey(key)) {
			int rank = currentPlayerRank[key];
			return GetScore(rank, timeSpan, collection);
		} else {
			return null;
		}
		
	}
	
	public void UpdateCurrentPlayerRank(int rank, GCBoardTimeSpan timeSpan, GCCollectionType collection) {
		string key = timeSpan.ToString() + "_" + collection.ToString();
		if(currentPlayerRank.ContainsKey(key)) {
			currentPlayerRank[key] = rank;
		} else {
			currentPlayerRank.Add(key, rank);
		}
	}


	public GCScore GetScore(int rank, GCBoardTimeSpan scope, GCCollectionType collection) {

		ScoreCollection col = GlobalCollection;
		
		switch(collection) {
		case GCCollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GCCollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}
		



		Dictionary<int, GCScore> scoreDict = col.AllTimeScores;
		
		switch(scope) {
		case GCBoardTimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GCBoardTimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GCBoardTimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}



		if(scoreDict.ContainsKey(rank)) {
			return scoreDict[rank];
		} else {
			return null;
		}

	}

	public void UpdateScore(GCScore s) {

		ScoreCollection col = GlobalCollection;

		switch(s.collection) {
		case GCCollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GCCollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}




		Dictionary<int, GCScore> scoreDict = col.AllTimeScores;

		switch(s.timeSpan) {
		case GCBoardTimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GCBoardTimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GCBoardTimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}


		if(scoreDict.ContainsKey(s.GetRank())) {
			scoreDict[s.GetRank()] = s;
		} else {
			scoreDict.Add(s.GetRank(), s);
		}
	}

}

