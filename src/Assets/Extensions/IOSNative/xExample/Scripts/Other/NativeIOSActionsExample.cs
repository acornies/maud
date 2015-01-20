using UnityEngine;
using System.Collections;

public class NativeIOSActionsExample : BaseIOSFeaturePreview {

	public Texture2D hello_texture;
	public Texture2D darawTexgture = null;


	void Awake() {


		IOSSharedApplication.OnUrCheckResultAction += OnUrCheckResultAction;
	}



	void OnGUI() {
		UpdateToStartPos();


		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Using Url Scheme", style);
		
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Check if FB App exists")) {
			IOSSharedApplication.instance.CheckUrl("fb://");
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open Fb Profile")) {
			IOSSharedApplication.instance.OpenUrl("fb://profile");
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open App Store")) {
			IOSSharedApplication.instance.OpenUrl("itms-apps://");
		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;



		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Video", style);
		
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Player Steamed video")) {
			IOSVideoManager.instance.PlaySteamingVideo("https://dl.dropboxusercontent.com/u/83133800/Important/Doosan/GT2100-Video.mov");
		}
		
		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Open Youtube Video")) {
			IOSVideoManager.instance.OpenYoutubeVideo("xzCEdSKMkdU");
		}

		
		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;



		
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "Camera Roll", style);
		
		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth + 10, buttonHeight), "Save Screenshot To Camera Roll")) {
			IOSCamera.instance.OnImageSaved += OnImageSaved;
			IOSCamera.instance.SaveScreenshotToCameraRoll();
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Save Texture To Camera Roll")) {
			IOSCamera.instance.OnImageSaved += OnImageSaved;
			IOSCamera.instance.SaveTextureToCameraRoll(hello_texture);
		}


		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Image From Album")) {
			IOSCamera.instance.OnImagePicked += OnImage;
			IOSCamera.instance.GetImageFromAlbum();

		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Get Image From Camera")) {
			IOSCamera.instance.OnImagePicked += OnImage;
			IOSCamera.instance.GetImageFromCamera();

		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		StartY+= YLableStep;
		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "PickedImage", style);
		StartY+= YLableStep;

		if(darawTexgture != null) {
			GUI.DrawTexture(new Rect(StartX, StartY, buttonWidth, buttonWidth), darawTexgture);
		}
	

	}


	

	private void OnImage (IOSImagePickResult result) {
		if(result.IsSucceeded) {

			//destroying old texture
			Destroy(darawTexgture);

			//applaying new texture
			darawTexgture = result.image;
			IOSMessage.Create("Success", "Image Successfully Loaded, Image size: " + result.image.width + "x" + result.image.height);
		} else {
			IOSMessage.Create("Success", "Image Load Failed");
		}

		IOSCamera.instance.OnImagePicked -= OnImage;
	}

	private void OnImageSaved (ISN_Result result) {
		IOSCamera.instance.OnImageSaved -= OnImageSaved;
		if(result.IsSucceeded) {
			IOSMessage.Create("Success", "Image Successfully saved to Camera Roll");
		} else {
			IOSMessage.Create("Success", "Image Save Failed");
		}
	}

	private void OnUrCheckResultAction (ISN_CheckUrlResult result) {

		if(result.IsSucceeded) {
			IOSMessage.Create("Url Exists", "The " + result.url + " is registred" );
		} else {
			IOSMessage.Create("Url Exists", "The " + result.url + " wasn't registred");
		}
	}
}
