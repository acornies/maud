using UnityEngine;
using System.Collections;

public class ISN_PlayerScoreLoadedResult : ISN_Result {

	
	private GCScore _score = null;


	public ISN_PlayerScoreLoadedResult():base(false) {

	}

	public ISN_PlayerScoreLoadedResult(GCScore score):base(true) {
		_score = score;
	}
	
	
	public GCScore loadedScore {
		get {
			return _score;
		}
	}
}
