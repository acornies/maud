////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class IOSSocialUseExample : MonoBehaviour {

	private GUIStyle style;
	private GUIStyle style2;

	public Texture2D textureForPost;


	void Awake() {
	

		//events use example
		IOSSocialManager.instance.addEventListener(IOSSocialManager.FACEBOOK_POST_SUCCESS, OnPostSuccses);
		IOSSocialManager.instance.addEventListener(IOSSocialManager.TWITTER_POST_SUCCESS, OnPostSuccses);


		IOSSocialManager.instance.addEventListener(IOSSocialManager.FACEBOOK_POST_FAILED, OnPostFailed);
		IOSSocialManager.instance.addEventListener(IOSSocialManager.TWITTER_POST_FAILED, OnPostFailed);

		//actions use example:
		IOSSocialManager.instance.OnMailResult += OnMailResult;


		InitStyles();
	}

	private void InitStyles () {
		style =  new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 16;
		style.fontStyle = FontStyle.BoldAndItalic;
		style.alignment = TextAnchor.UpperLeft;
		style.wordWrap = true;
		
		
		style2 =  new GUIStyle();
		style2.normal.textColor = Color.white;
		style2.fontSize = 12;
		style2.fontStyle = FontStyle.Italic;
		style2.alignment = TextAnchor.UpperLeft;
		style2.wordWrap = true;
	}


	void OnGUI() {
		
		float StartY = 20;
		float StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Twitter", style);
		
		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post")) {
			IOSSocialManager.instance.TwitterPost("Titter posting test");
		}
		
		StartX += 170;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post Screehshot")) {
			StartCoroutine(PostTwitterScreenshot());
		}
		
	
		
		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Facebook", style);


		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post")) {
			IOSSocialManager.instance.FacebookPost("Facebook posting test");
		}

		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post Screehshot")) {
			StartCoroutine(PostFBScreenshot());
		}


		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Post Image")) {
			IOSSocialManager.instance.FacebookPost("Hello world", textureForPost);
		}


		StartY+= 80;
		StartX = 10;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Native Sharing", style);
		
		
		StartY+= 40;
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Text")) {
			IOSSocialManager.instance.ShareMedia("Some text to share");
		}
		
		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Screehshot")) {
			StartCoroutine(PostScreenshot());
		}


		StartX += 170;
		
		if(GUI.Button(new Rect(StartX, StartY, 150, 50), "Send Mail")) {
			IOSSocialManager.instance.SendMail("Mail Subject", "Mail Body", "mail1@gmail.com, mail2@gmail.com", textureForPost);
		}


	}



	private IEnumerator PostScreenshot() {
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		IOSSocialManager.instance.ShareMedia("Some text to share", tex);
		
		Destroy(tex);
		
	}

	private IEnumerator PostTwitterScreenshot() {

		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		IOSSocialManager.instance.TwitterPost("My app ScreehShot", tex);
		
		Destroy(tex);
		
	}

	private IEnumerator PostFBScreenshot() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		IOSSocialManager.instance.FacebookPost("My app ScreehShot", tex);
		
		Destroy(tex);
		
	}


	private void OnPostFailed() {
		IOSNativePopUpManager.showMessage("Positng example", "Post Failed :(");
	}

	private void OnPostSuccses() {
		IOSNativePopUpManager.showMessage("Positng example", "Posy Succses!");
	}



	private void OnMailResult (ISN_Result result) {
		if(result.IsSucceeded) {
			IOSNativePopUpManager.showMessage("Positng example", "Mail Sended");
		} else {
			IOSNativePopUpManager.showMessage("Positng example", "Mail Failed :(");
		}
	}
}

