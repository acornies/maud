////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class iAdEvent : MonoBehaviour {

	public const string FAIL_TO_RECEIVE_AD 			= "fail_to_receive_ad";
	public const string AD_LOADED         			= "ad_loaded";
	public const string AD_VIEW_LOADED    			= "ad_view_loaded";
	public const string AD_VIEW_ACTION_BEGIN  		= "ad_view_action_begin";
	public const string AD_VIEW_ACTION_FINISHED  	= "ad_view_action_finished";



	public const string INTERSTITIAL_DID_FAIL_WITH_ERROR 	= "interstitial_did_fail_with_error";
	public const string INTERSTITIAL_AD_WILL_LOAD         	= "interstitial_ad_will_load";
	public const string INTERSTITIAL_AD_DID_LOAD    		= "interstitial_ad_did_load";
	public const string INTERSTITIAL_AD_ACTION_DID_FINISH  	= "interstitial_ad_action_did_finish";
}

