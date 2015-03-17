using UnityEngine;
using System.Collections;

public class ISN_AchievementProgressResult : ISN_Result {


	private AchievementTemplate _tpl;

	public ISN_AchievementProgressResult(AchievementTemplate tpl):base(true) {
		_tpl = tpl;
	}


	public AchievementTemplate info {
		get {
			return _tpl;
		}
	}
}
