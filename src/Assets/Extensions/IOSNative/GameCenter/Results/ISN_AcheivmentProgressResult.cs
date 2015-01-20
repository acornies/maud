using UnityEngine;
using System.Collections;

public class ISN_AcheivmentProgressResult : ISN_Result {


	private AchievementTemplate _tpl;

	public ISN_AcheivmentProgressResult(AchievementTemplate tpl):base(true) {
		_tpl = tpl;
	}


	public AchievementTemplate info {
		get {
			return _tpl;
		}
	}
}
