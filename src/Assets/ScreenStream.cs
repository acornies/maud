using UnityEngine;


public class ScreenStream: MonoBehaviour
{
	public GUISkin skin = null;
	public Texture logo = null;

	Texture2D screen = null;
	bool synced = false;

	byte[] image;
	int width;
	int height;


	void OnGUI()
	{
		if (!synced)
		{
			if (SystemInfo.supportsGyroscope) Input.gyro.enabled = false;
			GUI.DrawTexture(new Rect(0,0, logo.width, logo.height), logo, ScaleMode.ScaleToFit);
			GUI.skin = skin;
			GUI.Label(new Rect(280.0f,  40.0f, 400, 40), "Connect this device with");
			GUI.Label(new Rect(280.0f,  70.0f, 400, 40), "USB cable to your computer and");
			GUI.Label(new Rect(280.0f, 100.0f, 400, 40), "play the game in editor");
			GUI.Label(new Rect(280.0f, 150.0f, 400, 40), "Select what device to use in:");
			GUI.Label(new Rect(280.0f, 180.0f, 400, 40), "Edit->Project Settings->Editor");
		}

		if (synced && (screen != null))
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), screen);
	}


	void LateUpdate()
	{
		Profiler.BeginSample("ScreenStream.LateUpdate");

		if (screen == null || screen.width != width || screen.height != height)
			screen = new Texture2D(width, height, TextureFormat.RGB24, false);

		Profiler.BeginSample("LoadImage");
		if ((image != null) && screen.LoadImage(image))
			synced = true;
		image = null;
		Profiler.EndSample();

		Profiler.EndSample();
	}


	public void OnDisconnect()
	{
		synced = false;
		image = null;
	}


	public void UpdateScreen(byte[] data, int width, int height)
	{
		// Loading texture takes a lot of time, so we postpone it and do it in
		// LateUpdate(), in case we receive several images during single frame.
		this.image = data;
		this.width = width;
		this.height = height;
	}
}
