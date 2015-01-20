////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class GCScore  {
	

	private int _rank;
	private string _score;

	private string _playerId;
	private string _leaderboardId;

	private GCCollectionType _collection;
	private GCBoardTimeSpan _timeSpan;


	public GCScore(string vScore, int vRank, GCBoardTimeSpan vTimeSpan, GCCollectionType sCollection, string lid, string pid) {
		_score = vScore;
		_rank = vRank;
		
		_playerId = pid;
		_leaderboardId = lid;
		
		
		_timeSpan  = vTimeSpan;
		_collection = sCollection;
		
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	public double GetDoubleScore() {
		return GetLongScore () / 100f;
	}
	
	public long GetLongScore() {
		return System.Convert.ToInt64 (_score);
	}
	
	
	public int GetRank() {
		return rank;
	}


	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public int rank {
		get {
			return _rank;
		}
	}

	public string score {
		get {
			return _score;
		}
	}
	
	public string playerId {
		get {
			return _playerId;
		}
	}
	
	public string leaderboardId {
		get {
			return _leaderboardId;
		}
	}
	
	
	public GCCollectionType collection {
		get {
			return _collection;
		}
	}
	
	
	public GCBoardTimeSpan timeSpan {
		get {
			return _timeSpan;
		}
	}

}

