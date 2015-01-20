////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class IOSStoreKitResponse  {

	public string productIdentifier;
	public InAppPurchaseState state;
	public string receipt;

	public IOSStoreKitError error;


}
