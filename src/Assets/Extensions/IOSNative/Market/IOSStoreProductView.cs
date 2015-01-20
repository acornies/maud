////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////
/// 
using UnityEngine;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


public class IOSStoreProductView : EventDispatcherBase {

	public const string PRODUCT_VIEW_LOADED 				= "product_view_loaded";
	public const string PRODUCT_VIEW_LOAD_FAILED 			= "product_view_load_failed";
	public const string PRODUCT_VIEW_APPEARED 				= "product_view_appeared";
	public const string PRODUCT_VIEW_DISMISSED 				= "product_view_dismissed";

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _createProductView(int viewId, string productsId);
	
	[DllImport ("__Internal")]
	private static extern void _showProductView(int viewId);
	#endif


	private List<string> _ids =  new List<string>();

	private int _id;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	public IOSStoreProductView() {
		foreach(string pid in IOSNativeSettings.Instance.DefaultStoreProductsView) {
			addProductId(pid);
		}

		IOSInAppPurchaseManager.instance.RegisterProductView(this);
	}

	public IOSStoreProductView(params string[] ids) {
		foreach(string pid in ids) {
			addProductId(pid);
		}

		IOSInAppPurchaseManager.instance.RegisterProductView(this);
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public void addProductId(string productId) {
		if(_ids.Contains(productId)) {
			return;
		}
		
		_ids.Add(productId);
	}

	

	public void Load() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			string ids = "";
			int len = _ids.Count;
			for(int i = 0; i < len; i++) {
				if(i != 0) {
					ids += ",";
				}
				
				ids += _ids[i];
			}

			_createProductView(id, ids);
		#endif
	}

	public void Show() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_showProductView(id);
		#endif
	}

	
	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public int id {
		get {
			return _id;
		}
	}


	//--------------------------------------
	// EVENTS
	//--------------------------------------

	public void OnProductViewAppeard() {
		dispatch(PRODUCT_VIEW_APPEARED);
	}

	public void OnProductViewDismissed() {
		dispatch(PRODUCT_VIEW_DISMISSED);
	}

	public void OnContentLoaded() {
		Show();
		dispatch(PRODUCT_VIEW_LOADED);
	}

	public void OnContentLoadFailed() {
		dispatch(PRODUCT_VIEW_LOAD_FAILED);
	}

	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------

	public void SetId(int viewId) {
		_id = viewId;
	}



}
