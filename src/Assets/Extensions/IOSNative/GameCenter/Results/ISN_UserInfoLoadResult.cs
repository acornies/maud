using UnityEngine;
using System.Collections;

public class ISN_UserInfoLoadResult : ISN_Result {

	private string _playerId;
	private GameCenterPlayerTemplate _tpl = null;
	
	
	public ISN_UserInfoLoadResult(string id):base(false) {
		_playerId = id;
	}
	
	public ISN_UserInfoLoadResult(GameCenterPlayerTemplate tpl):base(true) {
		_tpl = tpl;
	}
	
	
	
	
	public string playerId {
		get {
			return _playerId;
		}
	}	
	
	public GameCenterPlayerTemplate playerTemplate {
		get {
			return _tpl;
		}
	}
}
